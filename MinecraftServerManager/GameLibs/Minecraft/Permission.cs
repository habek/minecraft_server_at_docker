using Newtonsoft.Json;

namespace MinecraftServerManager.Minecraft
{

	public class Permission
	{
		//		[{
		//"permission": "operator",
		//"xuid": "2535418563391161"
		//},{
		//"permission": "operator",
		//"xuid": "2535405401068560"
		//}]

		[JsonProperty("permission")]
		public string? PermissionName { get; set; }
		public string? Xuid { get; set; }

	}
}
