using MinecraftServerManager.Minecraft.Users;
using Newtonsoft.Json;
using System.Runtime;

namespace MinecraftServerManager
{
	public class SettingsModel
	{
		private const string _notSet = "";

		public string DockerHost { get; set; } = _notSet;

		public List<MinecraftUser> KnownUsers { get; private set; } = new List<MinecraftUser>();
		public string BackupFolder { get; set; } = "";
		public string SettingsFilePath { get; set; } = "";

		public void Save()
		{
			File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public string GetBackupFolder(string serverId)
		{
			return Path.Join(BackupFolder, serverId);
		}

		public string GetBackupFilePath(string serverId, string backupName)
		{
			return Path.Join(GetBackupFolder(serverId), backupName);
		}
		public string GetBackupFilePath(string serverId)
		{
			return Path.Join(GetBackupFolder(serverId), $"backup-{DateTime.Now.ToString("yy-MM-dd-HH-mm-ss")}.tar.gz");
		}

		internal void UpdateUsers(IEnumerable<MinecraftUser> enumerable)
		{
			KnownUsers = enumerable.ToList();
		}
	}
}
