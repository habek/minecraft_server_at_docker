using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class ConsoleDataParser
	{
		public static bool IsOnlineInformationDataParsed(string line, out int numberOfPlayers, out int maxNumberOfPlayers)
		{
			var match = Regex.Match(line, @"There are (\d+)\/(\d+) players online");
			if (match.Success)
			{
				numberOfPlayers = int.Parse(match.Groups[1].Value);
				maxNumberOfPlayers = int.Parse(match.Groups[2].Value);
				return true;
			}
			numberOfPlayers = 0;
			maxNumberOfPlayers = 0;
			return false;
		}

		public static bool IsUserConnectedInformation(string line, out string? userName, out string? xuid)
		{
			var match = Regex.Match(line, @"Player connected: ([a-zA-Z0-9_]+), xuid: (\d+)");
			if (match.Success)
			{
				userName = match.Groups[1].Value;
				xuid = match.Groups[2].Value;
				return true;
			}
			userName = null;
			xuid = null;
			return false;
		}
		//[2022-04-10 22:13:56:634 INFO] Player disconnected: HabekTheOne, xuid: 2535405401068560

		public static bool IsUserDisconnectedInformation(string line, out string? userName, out string? xuid)
		{
			var match = Regex.Match(line, @"Player disconnected: ([a-zA-Z0-9_]+), xuid: (\d+)");
			if (match.Success)
			{
				userName = match.Groups[1].Value;
				xuid = match.Groups[2].Value;
				return true;
			}
			userName = null;
			xuid = null;
			return false;
		}

		internal static bool IsVersionInformation(string line, out string? version)
		{
			var match = Regex.Match(line, @"Version (\d+\.\d+\.\d+\.\d+)");
			if (match.Success)
			{
				version = match.Groups[1].Value;
				return true;
			}
			version = null;
			return false;
		}
	}
}
