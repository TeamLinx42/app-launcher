using System;
using System.Windows;

namespace AppLauncher;

public partial class App
{
    private static readonly Version Version = new(0, 1, 0);

    private void ValidateAndHandleCommand(StartupEventArgs eventArgs)
    {
        WindowsEventLog.LogEvent($"{nameof(AppLauncher)} V{Version} launched with args: {string.Join(',', eventArgs.Args)}");
        var launchArgs = eventArgs.Validate(WindowsEventLog.LogEvent);
        if (!launchArgs.IsValid)
        {
            WindowsEventLog.LogError("Invalid command".CreateMessage(eventArgs.Args));
            Shutdown(launchArgs.ExitCode);
            return;
        }

        if (launchArgs.IsAdministrativeCommand())
            if (!Process.IsRunningAsAdministrator())
            {
                WindowsEventLog.LogEvent("Restarting launcher with admin privileges...");
                Process.RunAsAdministrator(eventArgs.Args);
                Shutdown();
                return;
            }

        WindowsEventLog.LogEvent($"Executing command: {launchArgs.LaunchApplication}");
        switch (launchArgs.LaunchApplication.Type)
        {
            case LaunchCommandType.Register:
                WindowsEventLog.CreateEventSource();
                ProtocolHandler.Register(launchArgs.LaunchApplication.Command, WindowsEventLog.LogEvent);
                Shutdown();
                break;

            case LaunchCommandType.UnRegister:
                ProtocolHandler.UnRegister(launchArgs.LaunchApplication.Command, WindowsEventLog.LogEvent);
                Shutdown();
                break;

            case LaunchCommandType.Test:
                ShowWindow(launchArgs);
                break;

            case LaunchCommandType.Launch:
                ProtocolHandler.Launch(launchArgs.LaunchApplication, WindowsEventLog.LogEvent);
                Shutdown(launchArgs.ExitCode);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(launchArgs.LaunchApplication.Type), launchArgs.LaunchApplication.Type, "Unhandled command.");
        }
    }

    private static void ShowWindow(LaunchArgs launchArgs)
    {
        var mainWindow = new MainWindow
        {
            Command =
            {
                Text = launchArgs.LaunchApplication.Command
            }
        };

        if (launchArgs.LaunchApplication.Args != null)
            mainWindow.Parameters.Text = string.Join(' ', launchArgs.LaunchApplication.Args);
        mainWindow.LabelVersion.Content = $"v{Version}";
        mainWindow.Show();
    }
}