using MinecraftServerManager.Minecraft.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibs.Minecraft.Users
{
	public class GameUserInfo
	{
		public GameUserInfo(MinecraftUser minecraftUser)
		{
			User = minecraftUser;
		}

		public bool IsConnected { get; set; }
		public MinecraftUser User { get; set; }
		public string? Permission { get; set; }
	}
}
