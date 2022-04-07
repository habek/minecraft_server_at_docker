using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class MinecraftUsersManager
	{
		private static MinecraftUsersManager? _instance;
		ConcurrentDictionary<string, XboxUser> _allUsers = new();

		public MinecraftUsersManager(List<XboxUser> knownUsers)
		{
			foreach (var user in knownUsers)
			{
				_allUsers.TryAdd(user.UserName, user);
			}
		}

		public static MinecraftUsersManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MinecraftUsersManager(Program.Settings.KnownUsers);
				}
				return _instance;
			}
		}

		public XboxUser GetXboxUser(string userName)
		{
			if (_allUsers.TryGetValue(userName, out XboxUser? user))
			{
				return user;
			}
			user = new XboxUser { UserName = userName };
			_allUsers.TryAdd(userName, user);
			return _allUsers[userName];
		}

		public bool UpdateUser(string userName, string xuid)
		{
			var user = GetXboxUser(userName);
			if(user.Xuid == xuid)
			{
				return false;
			}
			user.Xuid = xuid;
			return true;
		}

		internal IEnumerable<XboxUser> GetAllUsers()
		{
			return _allUsers.Values;
		}
	}
}
