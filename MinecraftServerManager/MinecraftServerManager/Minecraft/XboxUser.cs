using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class XboxUser
	{
		public string UserName { get; set; } = "";
		public string Xuid { get; set; } = "";
		public override string ToString()
		{
			return $"{UserName}, xuid: {Xuid}";
		}
	}
}
