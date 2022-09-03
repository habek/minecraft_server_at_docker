using MinecraftServerManager;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Minecraft.Users;
using web_api.Hubs;

namespace web_api.Background
{
	public class BedrockService : IHostedService
	{
		private readonly SettingsModel _settings;
		ServersManager _serversManager;
		private CancellationTokenSource _refreshServerListCts = new CancellationTokenSource();

		public BedrockService(SettingsModel settings, ServersManager serversManager)
		{
			_settings = settings;
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
			_settings.Save();
			return Task.CompletedTask;
		}
	}
}
