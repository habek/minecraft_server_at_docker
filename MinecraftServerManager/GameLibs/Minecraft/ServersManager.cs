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
				if (container.Image.Contains("tchorwat/bedrock-in-docker") || (container.Image.Contains("habek/minecraft_server_at_docker") && !container.Image.Contains("manager")))
				{
					var serverId = MinecraftServer.GetServerIdFromContainer(container);

					if (!_minecraftServers.TryGetValue(serverId, out MinecraftServer? server))
					{
						server = new MinecraftServer(_minecraftUsersManager, _dockerHost, container);
						_minecraftServers[serverId] = server;
					}
					else
					{
						server.UpdateContainer(container);
					}
				}
			}
			OnServerListChanged?.Invoke();
		}
		public IList<MinecraftServer> Servers => _minecraftServers.Values.ToList();

		public MinecraftServer? GetMinecraftServer(string serverId)
		{
			_minecraftServers.TryGetValue(serverId, out MinecraftServer? server);
			return server;
		}

		public Action? OnServerListChanged { get; set; }
	}
}
