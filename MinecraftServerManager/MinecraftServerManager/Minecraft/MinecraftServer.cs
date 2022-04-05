using Docker.DotNet;
using Docker.DotNet.Models;
using MinecraftServerManager.Communication.Docker;
using Serilog;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class MinecraftServer
	{
		private readonly List<string> _logs = new List<string>();
		MemoryStream _logBuffer = new MemoryStream();
		private readonly DockerClient _dockerClient;
		private readonly string _containerId;

		public MinecraftServer(DockerHost dockerHost, ContainerListResponse container)
		{
			_dockerClient = dockerHost.DockerClient;
			_containerId = container.ID;
			Name = container.Names.FirstOrDefault() ?? container.ID;
			Id = container.ID;
			State = container.State;
			Status = container.Status;
			_ = ConnectAsync(CancellationToken.None);
		}

		public string State { get; private set; } = "Unknown";
		public string Status { get; private set; } = "Unknown";
		public string Name { get; private set; } = "Unknown";
		public string Id { get; internal set; } = "Unknown";
		public IEnumerable<object> Logs => _logs;
		public Action<MinecraftServer, string>? OnLogAppend;
		private bool _ttyEnabled;

		public override string ToString()
		{
			return Name;
		}

		public async Task ConnectAsync(CancellationToken cancellationToken)
		{
			var inspectResponse = await _dockerClient.Containers.InspectContainerAsync(_containerId, cancellationToken);
			_ttyEnabled = inspectResponse.Config.Tty;
			var stream = await _dockerClient.Containers.GetContainerLogsAsync(_containerId, _ttyEnabled, new ContainerLogsParameters { Timestamps = true, Follow = true, ShowStderr = true, ShowStdout = true, }, cancellationToken);
			byte[] buffer = new byte[1024];

			while (!cancellationToken.IsCancellationRequested)
			{
				try
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
				}
				catch (Exception ex)
				{
					Log.Error(ex.Message);
				}
			}
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
						if (OnLogAppend != null)
						{
							OnLogAppend(this, line);
						}
						if (_logs.Count > 10000)
						{
							_logs.RemoveRange(0, _logs.Count - 10000);
						}
						_logBuffer.Position = 0;
						_logBuffer.SetLength(0);
					}
					else
					{
						_logBuffer.WriteByte(c);
					}
				}
			}
		}

		internal async Task<string> LoadPropertiesFile()
		{
			return Encoding.UTF8.GetString(await GetFile("/bedrock/server.properties")).ReplaceLineEndings();
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
					ExtractionOptions opt = new ExtractionOptions
					{
						ExtractFullPath = true,
						Overwrite = true
					};
					var memoryStream = new MemoryStream();
					reader.WriteEntryTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
			throw new FileNotFoundException(parameters.Path);
		}
	}
}
