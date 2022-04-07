using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft.Users
{
	public class MinecraftUser
	{
		public string UserName { get; set; } = "";
		public string Xuid { get; set; } = "";
		public override string ToString()
		{
			return $"{UserName}, xuid: {Xuid}";
		}

		internal bool HasXuid()
		{
			return Xuid != null && Xuid.Length > 0;
		}
	}
}
