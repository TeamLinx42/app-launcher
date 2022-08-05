using System;

namespace AppLauncher;

public partial class App
{
    public App()
    {
        Startup += (_, eventArgs) =>
        {
            try
            {
                ValidateAndHandleCommand(eventArgs);
            }
            catch (Exception exception)
            {
                WindowsEventLog.LogEvent($"Error: {exception}");
                throw;
            }
        };
    }
}