﻿using System;

namespace AppLauncher;

public partial class App
{
    public App()
    {
        Startup += (_, eventArgs) =>
        {
            var launchArgs = eventArgs.Validate(WindowsEventLog.LogEvent);
            if (!launchArgs.IsValid)
            {
                WindowsEventLog.LogEvent("Invalid arguments".CreateMessage(eventArgs.Args));
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

                case LaunchCommandType.Undefined:
                case LaunchCommandType.Unknown:
                default:
                    WindowsEventLog.LogEvent($"Unknown command: {launchArgs.LaunchApplication}");
                    throw new ArgumentOutOfRangeException(nameof(launchArgs.LaunchApplication.Type), launchArgs.LaunchApplication.Type, "Unhandled command.");
            }
        };
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

        mainWindow.Show();
    }
}