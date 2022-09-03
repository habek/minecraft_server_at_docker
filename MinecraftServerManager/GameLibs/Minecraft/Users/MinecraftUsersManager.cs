using System.Collections.Concurrent;

namespace MinecraftServerManager.Minecraft.Users
{
	public class MinecraftUsersManager
	{
		private ConcurrentDictionary<string, MinecraftUser> _allUsers = new();
		private readonly SettingsModel _settings;

		public MinecraftUsersManager(SettingsModel settings)
		{
			foreach (var user in settings.KnownUsers)
			{
				_allUsers.TryAdd(user.UserName, user);
			}
			_settings = settings;
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
			_settings.UpdateUsers(GetAllUsers().Where(user => user.HasXuid()));
			_settings.Save();
			return true;
		}

		public IEnumerable<MinecraftUser> GetAllUsers()
		{
			return _allUsers.Values;
		}

		internal MinecraftUser GetXboxUserByXuid(string xuid)
		{
			var existingUser = _allUsers.FirstOrDefault(keyValue => keyValue.Value.Xuid == xuid).Value;
			if (existingUser != null)
			{
				return existingUser;
			}
			return GetXboxUser(xuid);
		}
	}
}
