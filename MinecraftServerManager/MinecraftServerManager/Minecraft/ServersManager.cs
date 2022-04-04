using MinecraftServerManager.Communication.Docker;
using System.Collections.Concurrent;

namespace MinecraftServerManager.Minecraft
{
	public class ServersManager
	{
		private DockerHost _dockerHost;
		private ConcurrentDictionary<string, MinecraftServer> _minecraftServers = new ConcurrentDictionary<string, MinecraftServer>();
		private readonly CancellationToken _cancellationToken;

		public ServersManager(CancellationToken cancellationToken)
		{
			_dockerHost = new DockerHost("tcp://192.168.200.11:2375");
			_cancellationToken = cancellationToken;
		}

		public async Task RefreshServersList()
		{
			await _dockerHost.RefreshContainerList();
			foreach (var container in _dockerHost.GetContainerList())
			{
				if (!_minecraftServers.TryGetValue(container.ID, out MinecraftServer? server))
				{
					if (!container.Names.Any(n => n.Contains("test")))
					{
						//continue;
					}
					server = new MinecraftServer(_dockerHost, container.ID);
					if(_minecraftServers.TryAdd(container.ID, server))
					{
						server.Connect(_cancellationToken);
						server.RefreshState(container);
					}
				}
			}
		}
		public IList<MinecraftServer> Servers => _minecraftServers.Values.ToList();

		internal void OnStdOut(string id, Action<string> p)
		{
			_dockerHost.OnStdOut(id, p, _cancellationToken);
		}
	}
}
