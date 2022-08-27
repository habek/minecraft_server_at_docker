using System.Collections.Concurrent;

namespace MinecraftServerManager.Minecraft.Users
{
	public class MinecraftUsersManager
	{
		private ConcurrentDictionary<string, MinecraftUser> _allUsers = new();

		public MinecraftUsersManager(SettingsModel settings)
		{
			foreach (var user in settings.KnownUsers)
			{
				_allUsers.TryAdd(user.UserName, user);
			}
		}

		public MinecraftUser GetXboxUser(string userName)
		{
			if (_allUsers.TryGetValue(userName, out MinecraftUser? user))
			{
				return user;
			}
			user = new MinecraftUser { UserName = userName };
			_allUsers.TryAdd(userName, user);
			return _allUsers[userName];
		}

		public bool UpdateUser(string userName, string xuid)
		{
			var user = GetXboxUser(userName);
			if (user.Xuid == xuid)
			{
				return false;
			}
			user.Xuid = xuid;
			return true;
		}

		public IEnumerable<MinecraftUser> GetAllUsers()
		{
			return _allUsers.Values;
		}
	}
}
