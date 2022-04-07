using MinecraftServerManager.Minecraft;
using NUnit.Framework;

namespace MinecraftServerManagerTests
{
	public class ConsoleDataParserTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[TestCase("There are 1/10 players online:", 1,10)]
		[TestCase("There are 0/5 players online:", 0, 5)]
		public void OnlinePlayersExpressionTest(string line, int numberOfPlayers, int maxNumberOfPlayers)
		{
			var isMatch = ConsoleDataParser.IsOnlineInformationDataParsed(line, out int players, out int max);
			Assert.IsTrue(isMatch);
			Assert.AreEqual(numberOfPlayers, players);
			Assert.AreEqual(maxNumberOfPlayers, max);
		}
	}
}