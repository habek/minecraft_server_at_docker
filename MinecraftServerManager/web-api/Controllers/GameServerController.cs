using GameLibs.Minecraft.Users;
using Microsoft.AspNetCore.Mvc;
using MinecraftServerManager.Minecraft;
using System.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace web_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GameServerController : ControllerBase
	{
		private readonly ServersManager _serversManager;

		public GameServerController(ServersManager serversManager)
		{
			_serversManager = serversManager;
		}
		// GET: api/<GameServerController>
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return _serversManager.Servers.Select(x => x.Name);
		}

		// GET api/<GameServerController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		private MinecraftServer? GetServer(string serverName)
		{
			serverName = HttpUtility.UrlDecode(serverName);
			return _serversManager.GetMinecraftServer(serverName);
		}

		[HttpGet("Logs/{serverName}")]
		public ActionResult<IEnumerable<string>> GetLogs(string serverName)
		{
			var server = GetServer(serverName);
			if (server == null)
			{
				return NotFound();
			}
			return Ok(server.Logs);
		}

		[HttpGet("Users/{serverName}")]
		public async Task<ActionResult<IEnumerable<GameUserInfo>>> GetUsers(string serverName)
		{
			var server = GetServer(serverName);
			if (server == null)
			{
				return NotFound();
			}
			return Ok(await server.GetUserInfos());
		}


		// POST api/<GameServerController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<GameServerController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<GameServerController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
