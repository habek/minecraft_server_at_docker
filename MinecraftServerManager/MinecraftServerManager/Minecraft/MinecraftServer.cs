using Docker.DotNet;
using Docker.DotNet.Models;
using MinecraftServerManager.Communication.Docker;
using Serilog;
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
		private readonly string _iD;

		public MinecraftServer(DockerHost dockerHost, string iD)
		{
			_dockerClient = dockerHost.DockerClient;
			_iD = iD;
		}

		public string State { get; private set; } = "Unknown";
		public string Status { get; private set; } = "Unknown";
		public string Name { get; private set; } = "Unknown";
		public string Id { get; internal set; } = "Unknown";
		public IEnumerable<object> Logs => _logs;
		public Action<MinecraftServer, string>? OnLogAppend;

		public override string ToString()
		{
			return Name;
		}

		internal void RefreshState(ContainerListResponse container)
		{
			Name = container.Names.FirstOrDefault() ?? container.ID;
			Id = container.ID;
			State = container.State;
			Status = container.Status;
		}

		public void Connect(CancellationToken cancellationToken)
		{
			_dockerClient.Containers.GetContainerLogsAsync(_iD, true, new ContainerLogsParameters { Timestamps = true, Follow = true, ShowStderr = true, ShowStdout = true, }, cancellationToken).ContinueWith(async (t) =>
			   {
				   var stream = await t;
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
			   });
		}

		public async Task SendCommand(string command, CancellationToken cancellationToken)
		{
			var stream = await _dockerClient.Containers.AttachContainerAsync(_iD, true, new ContainerAttachParameters { Stream = true, Stdout = true, Stderr = true, Stdin = true }, cancellationToken).ConfigureAwait(false);
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
	}
}
