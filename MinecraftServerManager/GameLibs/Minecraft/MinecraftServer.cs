using Docker.DotNet;
using Docker.DotNet.Models;
using GameLibs.Minecraft.Users;
using MinecraftServerManager.Communication.Docker;
using MinecraftServerManager.Minecraft.Users;
using Newtonsoft.Json;
using Serilog;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class MinecraftServer
	{
		public const string PropertiesPathOnContainer = "/bedrock/server.properties";
		public const string PermissionsPathOnContainer = "/bedrock/permissions.json";
		public const string worldFolderOnContainer = "/bedrock/worlds";
		private readonly List<string> _logs = new List<string>();
		private CancellationTokenSource _logsTaskCancelationTokenSource;
		private readonly MemoryStream _logBuffer = new MemoryStream();
		private ConcurrentDictionary<string, MinecraftUser> _connectedUsers = new();
		private readonly DockerClient _dockerClient;
		private string _containerId;
		private readonly ConcurrentDictionary<string, Permission> _permissions = new ConcurrentDictionary<string, Permission>();

		private readonly MinecraftUsersManager _minecraftUsersManager;

		public string State { get; private set; } = "Unknown";
		public string Status { get; private set; } = "Unknown";
		public string Id  {get;private set;}
		public IEnumerable<string> Logs => _logs;

		public static string LineSeparator { get => "\n"; }

		public MinecraftServer(MinecraftUsersManager minecraftUsersManager, DockerHost dockerHost, ContainerListResponse container)
		{
			Id = GetServerIdFromContainer(container);
			_dockerClient = dockerHost.DockerClient;
			_containerId = container.ID;
			State = container.State;
			Status = container.Status;
			_logsTaskCancelationTokenSource = new CancellationTokenSource();
			_ = ConnectAsync(_logsTaskCancelationTokenSource.Token);
			_minecraftUsersManager = minecraftUsersManager;
		}

		public void UpdateContainer(ContainerListResponse container)
		{
			if(_containerId == container.ID)
			{
				return;
			}
			_containerId = container.ID;
			_logsTaskCancelationTokenSource.Cancel();
			_logsTaskCancelationTokenSource = new CancellationTokenSource();
			_ = ConnectAsync(_logsTaskCancelationTokenSource.Token);
		}

		public static string GetServerIdFromContainer(ContainerListResponse container)
		{
			return container.Names.FirstOrDefault() ?? container.ID;
		}

		[Flags]
		public enum ChangedData
		{
			Users = 1,
			State,
			Configuration
		}

		public class LogAppendEventArgs
		{
			public MinecraftServer MinecraftServer { get; set; }
			public string Line { get; set; }

			public LogAppendEventArgs(MinecraftServer minecraftServer, string line)
			{
				MinecraftServer = minecraftServer;
				Line = line;
			}
		}

		public Action<MinecraftServer, ChangedData>? OnDataChanged;
		public EventHandler<LogAppendEventArgs>? OnLogAppend;
		private bool _isRunning;
		private bool _ttyEnabled;
		private string? _timestamp;
		private string? _minecraftVersion;

		public override string ToString()
		{
			var s = Id.TrimStart('/');
			if (!_connectedUsers.IsEmpty)
			{
				s += $" ({_connectedUsers.Count})";
			}
			return s;
		}

		public IReadOnlyList<MinecraftUser> ConnectedUsers => _connectedUsers.Values.ToList();

		public string? MinecraftVersion { get => _minecraftVersion; }

		public async Task RefreshContainerState(CancellationToken cancellationToken)
		{
			try
			{
				var inspectResponse = await _dockerClient.Containers.InspectContainerAsync(_containerId, cancellationToken);
				_isRunning = inspectResponse.State.Running == true;
				_ttyEnabled = inspectResponse.Config.Tty;
				SetState(inspectResponse.State.Status);
			}
			catch (Exception ex)
			{
				if (ex is DockerContainerNotFoundException containerNotFoundException)
				{
					SetState("Not found");
					_isRunning = false;
				}
				throw;
			}
		}

		public async Task ConnectAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					await RefreshContainerState(cancellationToken);
					string? since = null;
					if (_timestamp != null)
					{
						var linuxTimestamp = DateTimeOffset.Parse(_timestamp);
						since = linuxTimestamp.ToUnixTimeSeconds().ToString();
					}
					var stream = await _dockerClient.Containers.GetContainerLogsAsync(_containerId, _ttyEnabled, new ContainerLogsParameters { Timestamps = true, Follow = true, ShowStderr = true, ShowStdout = true, Since = since }, cancellationToken);
					byte[] buffer = new byte[1024];

					while (!cancellationToken.IsCancellationRequested)
					{
						var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
						if (readResult.Count > 0)
						{
							AppendDataToLog(buffer, readResult);
						}
						else
						{
							await Task.Delay(100, cancellationToken);
						}
						if (readResult.EOF)
						{
							do
							{
								await Task.Delay(1000, cancellationToken);
								await RefreshContainerState(cancellationToken);
							} while (!_isRunning);
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message);
					await Task.Delay(1000, cancellationToken);
				}
			}
		}

		private void SetState(string newState)
		{
			if (newState == State)
			{
				return;
			}
			State = newState;
			OnDataChanged?.Invoke(this, ChangedData.State);
		}

		public async Task Start()
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;
			await _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters { }, cancellationToken);
			do
			{
				await RefreshContainerState(cancellationToken);
			} while (!_isRunning);
		}

		public async Task Stop()
		{
			AppendActionLineToLog("Stopping...");
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(10000);
			try
			{
				await SendCommand("stop", cancellationTokenSource.Token);
				do
				{
					await Task.Delay(100, cancellationTokenSource.Token);
					await RefreshContainerState(cancellationTokenSource.Token);
				} while (_isRunning);
				AppendActionLineToLog("Stopped gracefully");
				return;
			}
			catch (Exception ex)
			{
				AppendActionLineToLog("Stop server error: " + ex.Message);
				Log.Error(ex, "Stop server error");
			}
			finally
			{
				cancellationTokenSource.Dispose();
			}
			using var cancellationTokenSource2 = new CancellationTokenSource();
			{
				AppendActionLineToLog("Stopping container");
				int timeout = 10000;
				cancellationTokenSource2.CancelAfter(timeout);
				try
				{
					await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters { WaitBeforeKillSeconds = (uint)(timeout - 5000) }, cancellationTokenSource2.Token);
					do
					{
						await Task.Delay(100, cancellationTokenSource2.Token);
						await RefreshContainerState(cancellationTokenSource2.Token);
					} while (_isRunning);
					AppendActionLineToLog("Stopped (forced)");
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Stop container error");
					AppendActionLineToLog("Stop container error: " + ex.Message);
				}
			}
		}

		public async void Restore(string fileName)
		{
			AppendActionLineToLog("Starting restore...");
			try
			{

				var newArchive = new MemoryStream();
				using (var writer = WriterFactory.Open(newArchive, ArchiveType.Tar, CompressionType.GZip))
				{
					using (var reader = ReaderFactory.Open(File.OpenRead(fileName)))
					{
						while (reader.MoveToNextEntry())
						{
							if (!reader.Entry.IsDirectory)
							{
								var memoryStream = new MemoryStream();
								reader.WriteEntryTo(memoryStream);
								var destPath = reader.Entry.Key;
								const string worlds = "worlds/";
								if (destPath.StartsWith(worlds))
								{
									destPath = destPath[worlds.Length..];
								}
								memoryStream.Position = 0;
								writer.Write(destPath, memoryStream, reader.Entry.LastModifiedTime);
							}
						}
					}
				}
				await Stop();
				try
				{


					using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
					cancellationTokenSource.CancelAfter(60000);
					var cancellationToken = cancellationTokenSource.Token;
					newArchive.Position = 0;
					await _dockerClient.Containers.ExtractArchiveToContainerAsync(_containerId, new ContainerPathStatParameters { Path = worldFolderOnContainer }, newArchive);
				}
				finally
				{
					try
					{
						await Start();
					}
					catch (Exception)
					{

					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Restore backup failed");
				AppendActionLineToLog("Restore backup failed: " + ex.Message);
			}
		}

		public async Task Backup(string destinationPath)
		{
			AppendActionLineToLog("Starting backup...");
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(10000);
			var cancellationToken = cancellationTokenSource.Token;
			try
			{
				using (var stream = await Attach())
				{
					await stream.WriteLineWithVerify("save hold");
					try
					{
						await stream.ReadLine();//Saving...
						await stream.WriteLineWithVerify("save query");
						int maxRetries = 5;
						int i = 0;
						while (true)
						{
							var line = await stream.ReadLine();
							if (line.StartsWith("Data saved."))
							{
								break;
							}
							if (i++ >= maxRetries)
							{
								throw new Exception("Wait for saving data failed");
							}
						}
						var filesList = await stream.ReadLine();
						var filesForBackup = filesList.Split(", ").Select(path => path.Split(":")[0]).ToList();
						await CreateBackup(filesForBackup, destinationPath);
						AppendActionLineToLog("Files: " + filesList);
					}
					finally
					{
						await stream.WriteLine("save resume");
					}
					AppendActionLineToLog("Backup saved to " + destinationPath);
				}
			}
			catch (Exception ex)
			{
				AppendActionLineToLog("Backup error: " + ex.Message);
				Log.Error(ex, "Backup error");
			}
		}

		private async Task CreateBackup(List<string> filesForBackup, string destinationPath)
		{
			string? destDir = Path.GetDirectoryName(destinationPath);
			if (destDir != null)
			{
				Directory.CreateDirectory(destDir);
			}
			var tarStream = new MemoryStream();
			using (var writer = WriterFactory.Open(tarStream, ArchiveType.Tar, CompressionType.GZip))
			{
				foreach (var relativePath in filesForBackup)
				{
					var fileData = await GetFile($"{worldFolderOnContainer}/{relativePath}");
					writer.Write($"worlds/{relativePath}", new MemoryStream(fileData.Content), fileData.ModificationTime);
				}
			}
			await File.WriteAllBytesAsync(destinationPath, tarStream.ToArray());
		}

		private async Task UpdateAllData()
		{
			await UpdateUsersList();
			await UpdatePermissions();
		}

		public async Task SendCommand(string command, CancellationToken cancellationToken)
		{
			using (var stream = await Attach())
			{
				await stream.WriteLine(command);
			};
			//var stream = await _dockerClient.Containers.AttachContainerAsync(_containerId, _ttyEnabled, new ContainerAttachParameters { Stream = true, Stdout = true, Stderr = true, Stdin = true }, cancellationToken).ConfigureAwait(false);
			//try
			//{
			//	byte[] buffer = new byte[1024];
			//	var data = Encoding.UTF8.GetBytes(command);
			//	await stream.WriteAsync(data, 0, data.Length, cancellationToken);
			//	//while (!cancellationToken.IsCancellationRequested)
			//	//{
			//	//	var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);
			//	//	if (readResult.Count > 0)
			//	//	{
			//	//		AppendDataToLog(buffer, readResult);
			//	//	}
			//	//	else
			//	//	{
			//	//		await Task.Delay(100, cancellationToken);
			//	//	}
			//	//}
			//}
			//catch (Exception ex)
			//{
			//	Log.Error(ex.Message);
			//}
		}

		public Permission? GetUserPermission(MinecraftUser user)
		{
			if (_permissions.TryGetValue(user.Xuid, out Permission? permission))
			{
				return permission;
			}
			return null;
		}

		public async Task<List<Permission>> UpdatePermissions()
		{
			try
			{
				var json = await LoadTextFile(PermissionsPathOnContainer);
				var permissions = JsonConvert.DeserializeObject<List<Permission>>(json);
				foreach (var permission in permissions)
				{
					if (permission.Xuid != null)
					{
						_permissions[permission.Xuid] = permission;
					}
				}
				OnDataChanged?.Invoke(this, ChangedData.Users);
				return permissions;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Updating permissions error");
			}
			return new List<Permission>();
		}

		public async Task<CommandStream> Attach()
		{
			CancellationTokenSource cancellationTokenSource = new();
			cancellationTokenSource.CancelAfter(5000);
			var stream = await _dockerClient.Containers.AttachContainerAsync(_containerId, _ttyEnabled, new ContainerAttachParameters { Stream = true, Stdout = true, Stderr = true, Stdin = true }, cancellationTokenSource.Token).ConfigureAwait(false);
			return new CommandStream(stream);
		}

		public async Task UpdateUsersList()
		{
			try
			{
				var users = new List<MinecraftUser>();
				using (var stream = await Attach())
				{
					await stream.WriteLineWithVerify("list");
					var line = await stream.ReadLine();
					if (ConsoleDataParser.IsOnlineInformationDataParsed(line, out int _numberOfPlayers, out int _maxNumberOfPlayers))
					{
						for (int i = 0; i < _numberOfPlayers; i++)
						{
							var userName = await stream.ReadLine();
							users.Add(_minecraftUsersManager.GetXboxUser(userName));
						}
					}
				}
				foreach (var userName in _connectedUsers.Keys)
				{
					if (!users.Any(u => u.UserName == userName))
					{
						_connectedUsers.TryRemove(userName, out _);
					}
				}
				foreach (var user in users)
				{
					_connectedUsers[user.UserName] = user;
				}
				OnDataChanged?.Invoke(this, ChangedData.Users);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error listing users");
			}
		}

		private void AppendDataToLog(byte[] buffer, MultiplexedStream.ReadResult readResult)
		{
			lock (_logBuffer)
			{
				for (int i = 0; i < readResult.Count; i++)
				{
					byte c = buffer[i];
					if (c == '\n')
					{
						var line = Encoding.UTF8.GetString(_logBuffer.ToArray());
						_logs.Add(line);
						if (line.Length > 30)
						{
							_timestamp = line.Substring(0, 30);
						}
						AppendLineToLog(line);
						_logBuffer.Position = 0;
						_logBuffer.SetLength(0);
					}
					else if (c != '\r')
					{
						_logBuffer.WriteByte(c);
					}
				}
			}
		}

		private void AppendActionLineToLog(string message)
		{
			AppendLineToLog($"**** {message} ****");
		}

		private void AppendLineToLog(string line)
		{
			//if (line.EndsWith("Server started."))
			//{
			//	_ = UpdateAllData();
			//}
			//else 
			if (line.Contains("Version"))
			{
				if (ConsoleDataParser.IsVersionInformation(line, out string? version))
				{
					_minecraftVersion = version;
					OnDataChanged?.Invoke(this, ChangedData.Configuration);
				}
			}
			else if (line.Contains("connected: "))
			{
				if (ConsoleDataParser.IsUserConnectedInformation(line, out string? userName, out string? xuid) && !string.IsNullOrEmpty(xuid) && !string.IsNullOrEmpty(userName))
				{
					bool userChanged = _minecraftUsersManager.UpdateUser(userName, xuid);
					if (_connectedUsers.TryAdd(userName, _minecraftUsersManager.GetXboxUser(userName)) || userChanged)
					{
						OnDataChanged?.Invoke(this, ChangedData.Users);
					}
				}
				else if (ConsoleDataParser.IsUserDisconnectedInformation(line, out userName, out xuid) && !string.IsNullOrEmpty(xuid) && !string.IsNullOrEmpty(userName))
				{
					_minecraftUsersManager.UpdateUser(userName, xuid);
					_connectedUsers.TryRemove(userName, out MinecraftUser _);
					OnDataChanged?.Invoke(this, ChangedData.Users);
				}
			}
			OnLogAppend?.Invoke(this, new LogAppendEventArgs(this, line));

			if (_logs.Count > 10000)
			{
				_logs.RemoveRange(0, _logs.Count - 10000);
			}
		}

		public async Task<string> LoadTextFile(string pathInContainer)
		{
			return Encoding.UTF8.GetString((await GetFile(pathInContainer)).Content).ReplaceLineEndings();
		}

		public async Task SaveTextFile(string pathInContainer, string content)
		{
			content = content.ReplaceLineEndings(LineSeparator);
			await SaveFile(pathInContainer, Encoding.UTF8.GetBytes(content));
		}

		private async Task SaveFile(string filePathOnContainer, byte[] content)
		{
			var tarStream = new MemoryStream();
			using (var writer = WriterFactory.Open(tarStream, ArchiveType.Tar, CompressionType.None))
			{
				writer.Write(Path.GetFileName(filePathOnContainer), new MemoryStream(content));
			}
			tarStream.Position = 0;
			await _dockerClient.Containers.ExtractArchiveToContainerAsync(_containerId, new ContainerPathStatParameters { Path = Path.GetDirectoryName(filePathOnContainer)?.Replace(Path.DirectorySeparatorChar, '/') }, tarStream);
		}

		public class FileData
		{
			public byte[] Content { get; set; } = Array.Empty<byte>();
			public DateTime? ModificationTime { get; set; }
		}

		internal async Task<FileData> GetFile(string filePathOnContainer)
		{
			GetArchiveFromContainerParameters parameters = new GetArchiveFromContainerParameters { Path = filePathOnContainer };
			var response = await _dockerClient.Containers.GetArchiveFromContainerAsync(_containerId, parameters, false);
			var reader = ReaderFactory.Open(response.Stream);
			while (reader.MoveToNextEntry())
			{
				if (!reader.Entry.IsDirectory)
				{
					var memoryStream = new MemoryStream();
					reader.WriteEntryTo(memoryStream);
					return new FileData { Content = memoryStream.ToArray(), ModificationTime = reader.Entry.LastModifiedTime };
				}
			}
			throw new FileNotFoundException(parameters.Path);
		}

		public async Task<IEnumerable<GameUserInfo>> GetUserInfos()
		{
			var permissions = await UpdatePermissions();
			var connectedUsers = ConnectedUsers;
			var xuids = permissions.Select(permission => permission.Xuid).Concat(connectedUsers.Select(user => user.Xuid)).Distinct();
			var userInfos = new List<GameUserInfo>();
			foreach (var xuid in xuids)
			{
				if (xuid == null)
				{
					continue;
				}
				var user = connectedUsers.FirstOrDefault(user => user.Xuid == xuid);
				if (user == null)
				{
					user = new MinecraftUser { UserName = xuid, Xuid = xuid };
				}
				var userInfo = new GameUserInfo(user);
				var permission = permissions.FirstOrDefault(permission => permission.Xuid == xuid);
				if (permission != null)
				{
					userInfo.Permission = permission.PermissionName;
				}
				userInfos.Add(userInfo);
			}
			return userInfos;
		}
	}
}
