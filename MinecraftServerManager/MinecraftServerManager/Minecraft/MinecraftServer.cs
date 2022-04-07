using Docker.DotNet;
using Docker.DotNet.Models;
using MinecraftServerManager.Communication.Docker;
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
		private const string PropertiesPathOnContainer = "/bedrock/server.properties";
		private readonly List<string> _logs = new List<string>();
		MemoryStream _logBuffer = new MemoryStream();
		private readonly DockerClient _dockerClient;
		private readonly string _containerId;
		private readonly MinecraftUsersManager _minecraftUsersManager;

		public MinecraftServer(MinecraftUsersManager minecraftUsersManager, DockerHost dockerHost, ContainerListResponse container)
		{
			_dockerClient = dockerHost.DockerClient;
			_containerId = container.ID;
			Name = container.Names.FirstOrDefault() ?? container.ID;
			Id = container.ID;
			State = container.State;
			Status = container.Status;
			_ = ConnectAsync(CancellationToken.None);
			_minecraftUsersManager = minecraftUsersManager;
		}

		public string State { get; private set; } = "Unknown";
		public string Status { get; private set; } = "Unknown";
		public string Name { get; private set; } = "Unknown";
		public string Id { get; internal set; } = "Unknown";
		public IEnumerable<object> Logs => _logs;
		private ConcurrentDictionary<string, XboxUser> _connectedUsers = new();

		public static string LineSeparator { get => "\n"; }


		public enum ChangedData { Users }
		public Action<MinecraftServer, ChangedData>? OnDataChanged;
		public Action<MinecraftServer, string>? OnLogAppend;
		private bool _ttyEnabled;
		private string? _timestamp;

		public override string ToString()
		{
			return Name;
		}

		public IReadOnlyList<XboxUser> ConnectedUsers => _connectedUsers.Values.ToList();

		public async Task ConnectAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					var inspectResponse = await _dockerClient.Containers.InspectContainerAsync(_containerId, cancellationToken);
					_ttyEnabled = inspectResponse.Config.Tty;
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
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message);
				}
			}
		}

		private async Task UpdateAllData()
		{
			await UpdateUsersList();
		}

		public async Task SendCommand(string command, CancellationToken cancellationToken)
		{
			var stream = await _dockerClient.Containers.AttachContainerAsync(_containerId, _ttyEnabled, new ContainerAttachParameters { Stream = true, Stdout = true, Stderr = true, Stdin = true }, cancellationToken).ConfigureAwait(false);
			try
			{
				byte[] buffer = new byte[1024];
				var data = Encoding.UTF8.GetBytes(command);
				await stream.WriteAsync(data, 0, data.Length, cancellationToken);
				//while (!cancellationToken.IsCancellationRequested)
				//{
				//	var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);
				//	if (readResult.Count > 0)
				//	{
				//		AppendDataToLog(buffer, readResult);
				//	}
				//	else
				//	{
				//		await Task.Delay(100, cancellationToken);
				//	}
				//}
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
			}
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
				var users = new List<XboxUser>();
				using (var stream = await Attach())
				{
					await stream.WriteLine("list");
					var line = await stream.ReadLine();
					if (line != "list")
					{
						throw new Exception("List users failure");
					}
					line = await stream.ReadLine();
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
			if (line.EndsWith("Server started."))
			{
				_ = UpdateAllData();
			}
			else if (ConsoleDataParser.IsUserInformation(line, out string? userName, out string? xuid) && !string.IsNullOrEmpty(xuid) && !string.IsNullOrEmpty(userName))
			{
				bool userChanged = _minecraftUsersManager.UpdateUser(userName, xuid);
				if (_connectedUsers.TryAdd(userName, _minecraftUsersManager.GetXboxUser(userName)) || userChanged)
				{
					OnDataChanged?.Invoke(this, ChangedData.Users);
				}
			}
			if (OnLogAppend != null)
			{
				OnLogAppend(this, line);
			}
			if (_logs.Count > 10000)
			{
				_logs.RemoveRange(0, _logs.Count - 10000);
			}
		}

		internal async Task<string> LoadPropertiesFile()
		{
			return Encoding.UTF8.GetString(await GetFile(PropertiesPathOnContainer)).ReplaceLineEndings();
		}

		internal async Task SavePropertiesFile(string text)
		{
			var content = text.ReplaceLineEndings(LineSeparator);
			await SaveFile(PropertiesPathOnContainer, Encoding.UTF8.GetBytes(content));
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

		internal async Task<byte[]> GetFile(string filePathOnContainer)
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
					return memoryStream.ToArray();
				}
			}
			throw new FileNotFoundException(parameters.Path);
		}

		public async Task Restart()
		{
			AppendActionLineToLog("Restarting...");
			await _dockerClient.Containers.RestartContainerAsync(_containerId, new ContainerRestartParameters());
			AppendActionLineToLog("Restarted");
		}
	}
}
