using System;

namespace AppLauncher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        Startup += (_, eventArgs) =>
        {
            var launchArgs = eventArgs.Validate();
            if (!launchArgs.IsValid)
            {
                Shutdown(launchArgs.ExitCode);
                return;
            }

            if (launchArgs.IsAdministrativeCommand())
                if (!Process.IsRunningAsAdministrator())
                {
                    Process.RunAsAdministrator(eventArgs.Args);
                    Shutdown();
                    return;
                }

            switch (launchArgs.LaunchCommand)
            {
                case LaunchCommand.Register:
                    ProtocolHandler.Register(launchArgs.Parameters);
                    Shutdown();
                    break;

                case LaunchCommand.UnRegister:
                    ProtocolHandler.UnRegister(launchArgs.Parameters);
                    Shutdown();
                    break;

                case LaunchCommand.Unknown:
                    Shutdown(launchArgs.ExitCode);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(launchArgs.LaunchCommand), launchArgs.LaunchCommand, "Unhandled command.");
            }
        };
    }
}