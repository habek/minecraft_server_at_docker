﻿using Microsoft.AspNetCore.SignalR;
using MinecraftServerManager.Minecraft;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using web_api.Background;
using static MinecraftServerManager.Minecraft.MinecraftServer;

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
				_ = SendServerListUpdate();
			};
		}

		private async Task SendServerListUpdate()
		{
			await Clients.All.SendAsync("ServerListChanged", _serversManager.Servers.Select(s => s.Name).OrderBy(name => name), CancellationToken.None);
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			await Clients.Caller.SendAsync("ServerListChanged", _serversManager.Servers.Select(s => s.Name).OrderBy(name => name), CancellationToken.None);
		}

		public async IAsyncEnumerable<string> ReadConsole(string serverName, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var server = _serversManager.GetMinecraftServer(serverName);
			if (server == null)
			{
				yield break;
			}
			var lines = new ConcurrentQueue<string>();
			void handler(object? server, LogAppendEventArgs e)
			{
				lines.Enqueue(e.Line);
			}
			server.OnLogAppend += handler;

			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (lines.TryDequeue(out string? line))
					{
						yield return line;
					}
					else
					{
						await Task.Delay(20, cancellationToken);
					}
				}
			}
			finally
			{
				server.OnLogAppend -= handler;
			}
		}
	}
}
