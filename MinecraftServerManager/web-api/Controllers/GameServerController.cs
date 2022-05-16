using GameLibs.Minecraft.Users;
using Microsoft.AspNetCore.Mvc;
using MinecraftServerManager;
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
		private readonly SettingsModel _settings;
		private readonly ILogger<GameServerController> _logger;

		public GameServerController(ServersManager serversManager, SettingsModel settings, ILogger<GameServerController> logger)
		{
			_serversManager = serversManager;
			_settings = settings;
			_logger = logger;
		}
		// GET: api/<GameServerController>
		[HttpGet]
		public IEnumerable<string> Get()
		{
			return _serversManager.Servers.Select(x => x.Id);
		}

		// GET api/<GameServerController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		private MinecraftServer? GetServer(string serverId)
		{
			serverId = HttpUtility.UrlDecode(serverId);
			return _serversManager.GetMinecraftServer(serverId);
		}

		[HttpGet("Logs/{serverId}")]
		public ActionResult<IEnumerable<string>> GetLogs(string serverId)
		{
			var server = GetServer(serverId);
			if (server == null)
			{
				return NotFound();
			}
			return Ok(server.Logs);
		}

		[HttpGet("Users/{serverId}")]
		public async Task<ActionResult<IEnumerable<GameUserInfo>>> GetUsers(string serverId)
		{
			var server = GetServer(serverId);
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

		[HttpGet("Actions/Backup/{serverId}")]
		public async Task<ActionResult> Backup(string serverId)
		{
			var server = GetServer(serverId);
			if (server == null)
			{
				return NotFound();
			}
			await server.Backup(_settings.GetBackupFilePath(server.Id));
			return Ok();
		}
		[HttpGet("{serverId}/restore/{backupName}")]
		public async Task<ActionResult> Restore(string serverId, string backupName)
		{
			var server = GetServer(serverId);
			if (server == null)
			{
				return NotFound("Unknown server");
			}
			await server.Restore(_settings.GetBackupFilePath(server.Id, backupName));
			return Ok();
		}

		public class BackupInfo
		{
			public string? Name { get; set; }
			public int Size { get; set; }
		}
		[HttpGet("{serverId}/backups")]
		public ActionResult<List<BackupInfo>> ListBackups(string serverId)
		{
			var server = GetServer(serverId);
			if (server == null)
			{
				return NotFound("Unknown server");
			}
			var folder = _settings.GetBackupFolder(server.Id);
			if (!Directory.Exists(folder))
			{
				return new List<BackupInfo>();
			}
			var files = Directory.EnumerateFiles(folder);
			return files.Select(f =>
			{
				var fileInfo = new FileInfo(f);
				return new BackupInfo
				{
					Name = fileInfo.Name,
					Size = (int)fileInfo.Length
				};
			}).ToList();
		}
	}
}
