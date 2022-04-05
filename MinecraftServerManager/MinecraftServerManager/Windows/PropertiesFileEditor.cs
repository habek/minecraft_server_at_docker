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

		public PropertiesFileEditor(MinecraftServer minecraftServer)
		{
			InitializeComponent();
			_minecraftServer = minecraftServer;
			Text = minecraftServer.Name;
			Task.Run(LoadPropertiesFile);
		}

		private async Task LoadPropertiesFile()
		{
			var fileContent = await _minecraftServer.LoadPropertiesFile();
			Invoke(() => textBox1.Text = fileContent);
		}
	}
}
