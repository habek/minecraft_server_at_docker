using MinecraftServerManager.Communication.Docker;
using System.Collections.Concurrent;

namespace MinecraftServerManager.Minecraft
{
	public class ServersManager
	{
		private DockerHost _dockerHost;
		private ConcurrentDictionary<string, MinecraftServer> _minecraftServers = new ConcurrentDictionary<string, MinecraftServer>();

		public ServersManager()
		{
			_dockerHost = new DockerHost("tcp://192.168.200.11:2375");
		}

		public async Task RefreshServersList()
		{
			await _dockerHost.RefreshContainerList();
			foreach (var container in _dockerHost.GetContainerList())
			{
				if (!_minecraftServers.TryGetValue(container.ID, out MinecraftServer? server))
				{
					server = new MinecraftServer(container.ID);
					_minecraftServers.TryAdd(container.ID, server);
				}
				server.RefreshState(container);
			}
		}
		public IList<MinecraftServer> Servers => _minecraftServers.Values.ToList();

		internal void OnStdOut(string id, Action<string> p)
		{
			_dockerHost.OnStdOut(id, p, CancellationToken.None);
		}
	}
}
