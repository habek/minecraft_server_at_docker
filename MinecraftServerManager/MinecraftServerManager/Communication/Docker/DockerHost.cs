using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Communication.Docker
{
	internal class DockerHost
	{
		DockerClient _client;
		private IList<ContainerListResponse> _containers = new List<ContainerListResponse>();

		public DockerHost(string uri)
		{
			_client = new DockerClientConfiguration(new Uri(uri)).CreateClient();
		}

		public async Task RefreshContainerList()
		{
			_containers = await _client.Containers.ListContainersAsync(new ContainersListParameters()
			{
				Limit = 100,
			});
		}

		public IList<ContainerListResponse> GetContainerList()
		{
			return _containers;
		}

		public void OnStdOut(string id, Action<string> action, CancellationToken cancellationToken)
		{
			_client.Containers.AttachContainerAsync(id, true, new ContainerAttachParameters { Stream = true, Stdout=true, Stderr=true }, cancellationToken).ContinueWith(async t =>
			 {
				 byte[] buffer = new byte[1024];
				 var stream = await t;
				 while (!cancellationToken.IsCancellationRequested)
				 {
					 var readResult = await stream.ReadOutputAsync(buffer, 0, buffer.Length, cancellationToken);
					 if (readResult.Count > 0)
					 {
						 action(new string( Encoding.Default.GetString(buffer, 0, readResult.Count).Where(c=>c>=32).ToArray()));
					 }
				 }
			 });
		}
	}
}
