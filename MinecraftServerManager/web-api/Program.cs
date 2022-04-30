using MinecraftServerManager;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Minecraft.Users;
using Newtonsoft.Json;
using web_api.Background;
using web_api.Hubs;

var settingsPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath ?? Environment.CurrentDirectory)!, "Settings.json");
SettingsModel settings = new();
if (File.Exists(settingsPath))
{
	var json = File.ReadAllText(settingsPath);
	settings = JsonConvert.DeserializeObject<SettingsModel>(json) ?? settings;
}
settings.SettingsFilePath = settingsPath;

var builder = WebApplication.CreateBuilder(args);

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
