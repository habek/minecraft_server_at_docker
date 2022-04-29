using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft.Users
{
	public class MinecraftUsersManager
	{
		private static MinecraftUsersManager? _instance;
		private readonly SettingsModel _settings;
		private ConcurrentDictionary<string, MinecraftUser> _allUsers = new();

		public MinecraftUsersManager(SettingsModel settings)
		{
			foreach (var user in settings.KnownUsers)
			{
				_allUsers.TryAdd(user.UserName, user);
			}
			_settings = settings;
		}

		//public static MinecraftUsersManager Instance
		//{
		//	get
		//	{
		//		if (_instance == null)
		//		{
		//			_instance = new MinecraftUsersManager(_settings.KnownUsers);
		//		}
		//		return _instance;
		//	}
		//}

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
