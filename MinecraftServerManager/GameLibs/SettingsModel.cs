using MinecraftServerManager.Minecraft.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager
{
	public class SettingsModel
	{
		private const string _notSet = "";

		public string DockerHost { get; set; } = _notSet;

		public List<MinecraftUser> KnownUsers { get; } = new List<MinecraftUser>();
		public string BackupFolder { get; set; } = "";
		public string SettingsFilePath { get; set; } = "";

		public void Save()
		{
			File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public string GetBackupFilePath(string name)
		{
			return Path.Join(BackupFolder, name, $"backup-{DateTime.Now.ToString("yy-dd-MM-HH-mm-ss")}.tar.gz");
		}
	}
}
