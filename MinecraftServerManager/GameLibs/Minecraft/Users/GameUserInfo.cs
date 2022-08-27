using MinecraftServerManager.Minecraft.Users;

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
