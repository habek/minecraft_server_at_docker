﻿using System;
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

		public static bool IsUserInformation(string line, out string? userName, out string? xuid)
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

	}
}
