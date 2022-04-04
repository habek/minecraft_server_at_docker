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
		StringBuilder _logLineBuffer = new StringBuilder();
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
			_dockerClient.Containers.GetContainerLogsAsync(_iD, true, new ContainerLogsParameters { Timestamps = true, Follow = true, ShowStderr = true, ShowStdout = true }, cancellationToken).ContinueWith(async (t) =>
			   {
				   var stream = await t;
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
						//await Task.Delay(100, cancellationToken);
					}
				   }
			   });
			_dockerClient.Containers.AttachContainerAsync(_iD, true, new ContainerAttachParameters { Stream = true, Stdout = true, Stderr = true, Stdin = true }, cancellationToken).ContinueWith(async t =>
			  {
				  byte[] buffer = new byte[1024];
				  var stream = await t;
				  var data = Encoding.UTF8.GetBytes("list\n");
				  try
				  {
					  await stream.WriteAsync(data, 0, data.Length, cancellationToken);
				  }
				  catch (Exception ex)
				  {
					  Log.Error(ex.Message);
				  }
				  while (!cancellationToken.IsCancellationRequested)
				  {
					  var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);
					  if (readResult.Count > 0)
					  {
						  AppendDataToLog(buffer, readResult);
					  }
					  else
					  {
						  //await Task.Delay(100, cancellationToken);
					  }
				  }
			  });
		}

		private void AppendDataToLog(byte[] buffer, MultiplexedStream.ReadResult readResult)
		{
			for (int i = 0; i < readResult.Count; i++)
			{
				char c = (char)buffer[i];
				if (c == '\n')
				{
					_logLineBuffer.Remove(0, 1);
					_logs.Add(_logLineBuffer.ToString());
					if (OnLogAppend != null)
					{
						OnLogAppend(this, _logLineBuffer.ToString());
					}
					if (_logs.Count > 10000)
					{
						_logs.RemoveRange(0, _logs.Count - 10000);
					}
					_logLineBuffer.Length = 0;
				}
				else if (c > 31)
				{
					_logLineBuffer.Append(c);
				}
			}
		}
	}
}
