using Microsoft.AspNetCore.SignalR;
using MinecraftServerManager.Minecraft;
using web_api.Background;

namespace web_api.Hubs
{
	public class EventsHub : Hub
	{
		private readonly ServersManager _serversManager;

		public EventsHub(ServersManager serversManager)
		{
			_serversManager = serversManager;
			serversManager.OnServerListChanged = () =>
			{
				Clients.All.SendAsync("ServerListChanged", _serversManager.Servers.Select(s => s.Id));
			};
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			await Clients.Caller.SendAsync("ServerListChanged", _serversManager.Servers.Select(s => s.Id));
		}
	}
}
