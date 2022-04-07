﻿using MinecraftServerManager.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MinecraftServerManager.Minecraft.MinecraftServer;

namespace MinecraftServerManager.Windows
{
	public partial class MainWindow : Form
	{
		private readonly ServersManager _serversManager;
		private string? _currentServerId;

		public MainWindow(ServersManager serversManager)
		{
			InitializeComponent();
			_serversManager = serversManager;
		}

		private void MainWindow_Shown(object sender, EventArgs e)
		{
			_serversManager.RefreshServersList().ContinueWith(t => Invoke(ServerListChanged));
		}

		private void ServerListChanged()
		{
			lvServers.Items.Clear();
			foreach (var server in _serversManager.Servers)
			{
				lvServers.Items.Add(new ListViewItem { Text = server.ToString(), Tag = server });
				server.OnLogAppend = (server, line) =>
				{
					if (IsDisposed)
					{
						return;
					};
					Invoke(OnLogAppend, server, line);
				};
				server.OnDataChanged = (server, changedData) =>
				{
					if (IsDisposed)
					{
						return;
					};
					Invoke(OnDataChanged, server, changedData);
				};
			}
			Task.Delay(5000).ContinueWith(t =>
			{
				Invoke(() =>
				{
					if (lvServers.Items.Count > 0)
					{
						if (lvServers.SelectedItems.Count == 0)
						{
							lvServers.Items[0].Selected = true;
						}
					}
				});
			});
		}

		private void OnDataChanged(MinecraftServer minecraftServer, ChangedData changedData)
		{
			if (minecraftServer.Id != _currentServerId)
			{
				return;
			}
			switch (changedData)
			{
				case ChangedData.Users: UpdateUserList(minecraftServer); break;
			}
		}

		private void UpdateUserList(MinecraftServer minecraftServer)
		{
			lvUsers.BeginUpdate();
			lvUsers.Items.Clear();
			foreach (var user in minecraftServer.ConnectedUsers)
			{
				var item = new ListViewItem(user.ToString())
				{
					Tag = user
				};
				lvUsers.Items.Add(item);
			}
			lvUsers.EndUpdate();
		}

		private void OnLogAppend(MinecraftServer minecraftServer, string lineContent)
		{
			if (minecraftServer.Id != _currentServerId)
			{
				return;
			}
			listBox1.Items.Add(lineContent);
			listBox1.TopIndex = listBox1.Items.Count - 1;
		}

		private void LvServers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvServers.SelectedItems.Count == 0)
			{
				return;
			}
			var minecraftServer = lvServers.SelectedItems[0].Tag as MinecraftServer;
			if (minecraftServer == null)
			{
				return;
			}
			if (_currentServerId == minecraftServer.Id)
			{
				return;
			}

			_currentServerId = minecraftServer.Id;

			listBox1.BeginUpdate();
			listBox1.Items.Clear();
			foreach (var line in minecraftServer.Logs)
			{
				listBox1.Items.Add(line);
			}
			listBox1.TopIndex = listBox1.Items.Count - 1;
			listBox1.EndUpdate();
			UpdateUserList(minecraftServer);
		}

		MinecraftServer? CurrentServer => _serversManager.Servers.FirstOrDefault(s => s.Id == _currentServerId);

		private void BtnSendCommand_Click(object sender, EventArgs e)
		{
			CurrentServer?.SendCommand(cmbCommandToSend.Text + "\n", CancellationToken.None);
		}

		private void BtnRestart_Click(object sender, EventArgs e)
		{
			CurrentServer?.Restart();
		}

		private void BtnListUsers_Click(object sender, EventArgs e)
		{
			CurrentServer?.UpdateUsersList();
		}

		private void BtnOpenProperties_Click(object sender, EventArgs e)
		{
			EditFile(PropertiesPathOnContainer);
		}

		private void BtnEditPermissions_Click(object sender, EventArgs e)
		{
			EditFile(PermissionsPathOnContainer);
		}

		private void EditFile(string pathInContainer)
		{
			var minecraftServer = CurrentServer;
			if (minecraftServer == null)
			{
				return;
			}
			var window = new PropertiesFileEditor(minecraftServer, pathInContainer);
			window.Show(this);
		}

	}
}
