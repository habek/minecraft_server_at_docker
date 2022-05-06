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
		private CancellationTokenSource _refreshServerListCts = new CancellationTokenSource();

		public BedrockService(SettingsModel settings, MinecraftUsersManager minecraftUsersManager, ServersManager serversManager)
		{
			_settings = settings;
			_minecraftUsersManager = minecraftUsersManager;
			_serversManager = serversManager;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Task.Run(async () =>
			{
				while (!_refreshServerListCts.IsCancellationRequested)
				{
					await _serversManager.RefreshServersList();
					await Task.Delay(60000, _refreshServerListCts.Token);
				}
			});
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_refreshServerListCts.Cancel();
			_settings.KnownUsers.Clear();
			_settings.KnownUsers.AddRange(_minecraftUsersManager.GetAllUsers().Where(user => user.HasXuid()));
			_settings.Save();
			return Task.CompletedTask;
		}
	}
}
