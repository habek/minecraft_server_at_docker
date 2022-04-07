using MinecraftServerManager;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Minecraft.Users;
using MinecraftServerManager.Windows;
using Newtonsoft.Json;
using Serilog;

public class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		var settingsPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath ?? Environment.CurrentDirectory)!, "Settings.json");
		try
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.WriteTo.File("logFile.log")
				.WriteTo.Debug()
				.CreateLogger();
			Console.WriteLine("Welcome in Minecraft manager");
			Log.Information("###### Start ######");
			try
			{
				if (File.Exists(settingsPath))
				{
					var json = File.ReadAllText(settingsPath);
					Settings = JsonConvert.DeserializeObject<SettingsModel>(json);
				}
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Settings load error");
			}
			var mainWindow = new MainWindow(new ServersManager(CancellationToken.None));
			Application.Run(mainWindow);
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
		finally
		{
			try
			{
				Settings.KnownUsers.Clear();
				Settings.KnownUsers.AddRange(MinecraftUsersManager.Instance.GetAllUsers().Where(user => user.HasXuid()));
				File.WriteAllText(settingsPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
			}
			catch (Exception ex)
			{
				Log.Warning(ex, "Settings save error");
			}
		}
		Log.CloseAndFlush();
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Log.Error("Unhandled exception: " + e.ExceptionObject);
		Log.CloseAndFlush();
	}
	public static SettingsModel Settings { get; set; } = new SettingsModel();
}