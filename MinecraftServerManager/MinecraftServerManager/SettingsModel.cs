using MinecraftServerManager.Minecraft.Users;
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
	}
}
