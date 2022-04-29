using MinecraftServerManager.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MinecraftServerManager.Minecraft.MinecraftServer;
using static System.Windows.Forms.ListViewItem;

namespace MinecraftServerManager.Windows
{
	public partial class MainWindow : Form
	{
		private readonly ServersManager _serversManager;
		private string? _currentServerId;
		private ConfigurationWindow? _configurationWindow;

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
				var stateSubitem = new ListViewSubItem();
				var serverItem = new ListViewItem { Tag = server };
				serverItem.SubItems.Add(stateSubitem);

				UpdateServerListViewItem(serverItem);
				lvServers.Items.Add(serverItem);
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

		private void UpdateServerListViewItem(ListViewItem server)
		{
			MinecraftServer minecraftServer = (MinecraftServer)server.Tag;
			server.Text = minecraftServer.ToString();
			server.SubItems[1].Text = minecraftServer.State;
		}

		private void OnDataChanged(MinecraftServer minecraftServer, ChangedData changedData)
		{
			if ((changedData & (ChangedData.Users | ChangedData.State)) != 0)
			{
				foreach (ListViewItem server in lvServers.Items)
				{
					if (server.Tag == minecraftServer)
					{
						UpdateServerListViewItem(server);
					}
				}
			}
			if (minecraftServer.Id != _currentServerId)
			{
				return;
			}
			switch (changedData)
			{
				case ChangedData.Users: UpdateUserList(minecraftServer); break;
				case ChangedData.Configuration: UpdateConfiguration(minecraftServer); break;
			}
		}

		private void UpdateConfiguration(MinecraftServer minecraftServer)
		{
			tbVersion.Text = minecraftServer.MinecraftVersion;
		}

		private void UpdateUserList(MinecraftServer minecraftServer)
		{
			lvUsers.BeginUpdate();
			lvUsers.Items.Clear();
			foreach (var user in minecraftServer.ConnectedUsers)
			{
				var item = new ListViewItem(user.UserName)
				{
					Tag = user
				};
				var xuidSubItem = new ListViewSubItem(item, user.Xuid);
				var permissionSubitem = new ListViewSubItem(item, minecraftServer.GetUserPermission(user)?.PermissionName);
				item.SubItems.Add(xuidSubItem);
				item.SubItems.Add(permissionSubitem);
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
			CurrentServer?.UpdatePermissions();
			UpdateConfiguration(minecraftServer);
		}

		private MinecraftServer? CurrentServer => _serversManager.Servers.FirstOrDefault(s => s.Id == _currentServerId);

		private void BtnSendCommand_Click(object sender, EventArgs e)
		{
			CurrentServer?.SendCommand(cmbCommandToSend.Text, CancellationToken.None);
		}

		private void BtnRestart_Click(object sender, EventArgs e)
		{
			CurrentServer?.Stop();
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

		private void BtnServerStart_Click(object sender, EventArgs e)
		{
			CurrentServer?.Start();
		}

		private void BtnBackup_Click(object sender, EventArgs e)
		{
			CurrentServer?.Backup(Program.Settings.GetBackupFilePath(CurrentServer.Name));
		}

		OpenFileDialog? openFileDialog;

		private void BtnRestore_Click(object sender, EventArgs e)
		{
			openFileDialog ??= new OpenFileDialog();
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
			{
				return;
			}
			CurrentServer?.Restore(openFileDialog.FileName);
		}

		private void BtnSettings_Click(object sender, EventArgs e)
		{
			_configurationWindow ??= new ConfigurationWindow();
			_configurationWindow.ShowDialog();
		}
	}
}
