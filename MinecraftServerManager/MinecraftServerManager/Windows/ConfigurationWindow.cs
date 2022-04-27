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
	public partial class ConfigurationWindow : Form
	{
		public ConfigurationWindow()
		{
			InitializeComponent();
		}

		private void TbBackupFolder_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				tbBackupFolder.Text = Program.Settings.BackupFolder;
				tbDockerHostAddress.Text = Program.Settings.DockerHost;
			}
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			Program.Settings.BackupFolder = tbBackupFolder.Text;
			Program.Settings.DockerHost = tbDockerHostAddress.Text;
			Program.Settings.Save();
			DialogResult = DialogResult.OK;
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
