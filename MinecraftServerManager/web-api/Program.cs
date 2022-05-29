using MinecraftServerManager;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Minecraft.Users;
using Newtonsoft.Json;
using Serilog;
using Serilog.AspNetCore;
using System.Reflection;
using web_api.Background;
using web_api.Hubs;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("logFile.log")
	.WriteTo.Debug()
	.CreateLogger();
Console.WriteLine("Welcome in Minecraft manager");

var settingsPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "../Settings.json");
SettingsModel settings = new();
if (!File.Exists(settingsPath))
{
	settingsPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "../../data/Settings.json");
}
if (File.Exists(settingsPath))
{
	var json = File.ReadAllText(settingsPath);
	Log.Logger.Information($"Loading settings from '{settingsPath}'");
	settings = JsonConvert.DeserializeObject<SettingsModel>(json) ?? settings;
}
else
{
	throw new Exception($"Setting file not found '{settingsPath}'");
}

settings.SettingsFilePath = settingsPath;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Log.Logger);

// Add services to the container.
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<MinecraftUsersManager>();
builder.Services.AddSingleton<ServersManager>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR()
	.AddNewtonsoftJsonProtocol(options =>
	{
		options.PayloadSerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
	});

builder.Services.AddHostedService<BedrockService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger(c =>
	{
		c.RouteTemplate = "api/swagger/{documentname}/swagger.json";
	});
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "My Cool API V1");
		c.RoutePrefix = "api/swagger";
	});
}

app.UseAuthorization();
app.MapHub<EventsHub>("/api/events");
app.MapControllers();

app.Run();
