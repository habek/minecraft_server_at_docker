using MinecraftServerManager.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Windows
{
	public partial class MainWindow : Form
	{
		private readonly ServersManager _serversManager;
		private string? _selectedServerId;

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
				server.OnLogAppend = (server, line) => Invoke(OnLogAppend, server, line);
			}
		}

		private void OnLogAppend(MinecraftServer minecraftServer, string lineContent)
		{
			if (minecraftServer.Id != _selectedServerId)
			{
				return;
			}
			listBox1.Items.Add(lineContent);
			listBox1.TopIndex = listBox1.Items.Count - 1;
		}

		private void lvServers_SelectedIndexChanged(object sender, EventArgs e)
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
			if (_selectedServerId == minecraftServer.Id)
			{
				return;
			}
			_selectedServerId = minecraftServer.Id;
			listBox1.BeginUpdate();
			listBox1.Items.Clear();
			foreach (var line in minecraftServer.Logs)
			{
				listBox1.Items.Add(line);
			}
			listBox1.TopIndex = listBox1.Items.Count - 1;
			listBox1.EndUpdate();
			//_serversManager.OnStdOut(_selectedServerId, (string stdOut) => Invoke(OnStdOut, stdOut));
		}
		private void OnStdOut(string text)
		{
			listBox1.Items.Add(text);
			if (listBox1.Items.Count > 1000)
			{
				listBox1.Items.RemoveAt(0);
			}
		}
	}
}
