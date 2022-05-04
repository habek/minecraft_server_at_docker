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
		[HttpGet("Logs/{serverName}")]
		public ActionResult<IEnumerable<string>> Get(string serverName)
		{
			serverName = HttpUtility.UrlDecode(serverName);
			var server = _serversManager.GetMinecraftServer(serverName);
			if (server == null)
			{
				return NotFound();
			}
			return Ok(server.Logs);
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
