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

		public bool HasXuid()
		{
			return Xuid != null && Xuid.Length > 0;
		}
	}
}
