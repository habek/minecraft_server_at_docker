using MinecraftServerManager.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinecraftServerManager.Windows
{
	public partial class PropertiesFileEditor : Form
	{
		private readonly MinecraftServer _minecraftServer;
		private readonly string _filePath;

		public PropertiesFileEditor(MinecraftServer minecraftServer, string filePath)
		{
			InitializeComponent();
			_minecraftServer = minecraftServer;
			_filePath = filePath;
			Text = $"Server: {minecraftServer.Name}, file: {_filePath}";
			Task.Run(LoadFile);
		}

		private async Task LoadFile()
		{
			var fileContent = await _minecraftServer.LoadTextFile(_filePath);
			Invoke(() => textBox1.Text = fileContent);
		}

		private async void btnSave_Click(object sender, EventArgs e)
		{
			await _minecraftServer.SaveTextFile(_filePath, textBox1.Text);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
