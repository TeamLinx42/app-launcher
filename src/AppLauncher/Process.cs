using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;

namespace AppLauncher;

internal static class Process
{
    public static string? ExeFileName => System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;

    public static bool IsRunningAsAdministrator()
    {
        var windowsIdentity = WindowsIdentity.GetCurrent();

        var windowsPrincipal = new WindowsPrincipal(windowsIdentity);

        return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void RunAsAdministrator(IEnumerable<string> args)
    {
        // Setting up start info of the new process of the same application
        var processStartInfo = new ProcessStartInfo
        {
            FileName = ExeFileName,
            // Using operating shell and setting the ProcessStartInfo.Verb to “runas” will let it run as admin
            UseShellExecute = true,
            Verb = "runas"
        };

        foreach (var arg in args) processStartInfo.ArgumentList.Add(arg);

        System.Diagnostics.Process.Start(processStartInfo);
    }

    public static void Run(ProcessStartParams processStartParams, IEnumerable<string>? args, Action<string> logEvent)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = $"\"{processStartParams.PathToApplication}\"",
            CreateNoWindow = processStartParams.CreateNoWindow,
            WindowStyle = processStartParams.ProcessWindowStyle
        };

        if (args != null)
            foreach (var arg in args)
                processStartInfo.ArgumentList.Add(arg);

        var message = "Launching".CreateMessage(processStartInfo.FileName).Join(processStartInfo.ArgumentList);
        logEvent(message);

        System.Diagnostics.Process.Start(processStartInfo);
    }
}