// See https://aka.ms/new-console-template for more information
using MinecraftServerManager;
using MinecraftServerManager.Communication.Docker;
using MinecraftServerManager.Minecraft;
using MinecraftServerManager.Windows;
using Serilog;

public class Program
{
	public static void Main(string[] args)
	{
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		try
		{
			Console.WriteLine("Hello, World!");
			var mainWindow = new MainWindow(new ServersManager());
			Application.Run(mainWindow);
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
		Log.CloseAndFlush();
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Log.Error("Unhandled exception: " + e.ExceptionObject);
		Log.CloseAndFlush();
	}
}