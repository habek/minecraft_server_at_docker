using MinecraftServerManager;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Minecraft.Users;
using web_api.Hubs;

namespace web_api.Background
{
	public class BedrockService : IHostedService
	{
		private readonly SettingsModel _settings;
		private readonly MinecraftUsersManager _minecraftUsersManager;
		ServersManager _serversManager;

		public BedrockService(SettingsModel settings, MinecraftUsersManager minecraftUsersManager, ServersManager serversManager)
		{
			_settings = settings;
			_minecraftUsersManager = minecraftUsersManager;
			_serversManager = serversManager;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _serversManager.RefreshServersList();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_settings.KnownUsers.Clear();
			_settings.KnownUsers.AddRange(_minecraftUsersManager.GetAllUsers().Where(user => user.HasXuid()));
			_settings.Save();
			return Task.CompletedTask;
		}
	}
}
