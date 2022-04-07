using MinecraftServerManager.Communication.Docker;
using MinecraftServerManager.Minecraft.Users;
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
			_dockerHost = new DockerHost($"tcp://{Program.Settings.DockerHost}:2375");
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
					server = new MinecraftServer(MinecraftUsersManager.Instance, _dockerHost, container);
					_minecraftServers[container.ID] = server;
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
