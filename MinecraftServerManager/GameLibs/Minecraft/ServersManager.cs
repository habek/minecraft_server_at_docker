using MinecraftServerManager.Communication.Docker;
using MinecraftServerManager.Minecraft.Users;
using System.Collections.Concurrent;

namespace MinecraftServerManager.Minecraft
{
	public class ServersManager
	{
		private DockerHost _dockerHost;
		private ConcurrentDictionary<string, MinecraftServer> _minecraftServers = new ConcurrentDictionary<string, MinecraftServer>();
		private readonly MinecraftUsersManager _minecraftUsersManager;

		public ServersManager(MinecraftUsersManager minecraftUsersManager, SettingsModel settings)
		{
			_dockerHost = new DockerHost($"tcp://{settings.DockerHost}:2375");
			_minecraftUsersManager = minecraftUsersManager;
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
					server = new MinecraftServer(_minecraftUsersManager, _dockerHost, container);
					_minecraftServers[container.ID] = server;
				}
			}
			OnServerListChanged?.Invoke();
		}
		public IList<MinecraftServer> Servers => _minecraftServers.Values.ToList();

		public Action? OnServerListChanged { get; set; }
	}
}
