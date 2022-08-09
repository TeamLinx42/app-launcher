using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace AppLauncher;

internal static class ProtocolHandler
{
    private const string UrlAllowListEntryIndex = "99";

    private static IEnumerable<(string, string?, string )> ProtocolRegValues(string protocolName, string appLauncherLocation)
    {
        yield return ($@"{protocolName}", null, $"{protocolName}:// protocol");
        yield return ($@"{protocolName}", "URL Protocol", "");
        yield return ($@"{protocolName}\DefaultIcon", null, $"{appLauncherLocation},1");
        yield return ($@"{protocolName}\shell\open\command", null, $"{appLauncherLocation} \"%1\"");
    }

    private static (string Path, string Key, string Value) EdgeRegValue(string protocolName) =>
        (@"SOFTWARE\Policies\Microsoft\Edge", "AutoLaunchProtocolsFromOrigins", AllowedOriginsValue(protocolName));

    private static (string Path, string Key, string Value) ChromeRegValue(string protocolName) =>
        (@"SOFTWARE\Policies\Google\Chrome\URLAllowlist", UrlAllowListEntryIndex, $"{protocolName}://");

    private static string AllowedOriginsValue(string protocolName) => "[{\"allowed_origins\":[\"*\"],\"protocol\":\"" + protocolName + "\"}]";

    public static void Register(string protocolName, Action<string> logEvent)
    {
        var appLauncherLocation = Process.ExeFileName;
        if (appLauncherLocation == null) throw new InvalidOperationException("Unable to get AppLauncher location");

        foreach (var (path, key, value) in ProtocolRegValues(protocolName, appLauncherLocation))
            SetRegistryKey(Registry.ClassesRoot.Name, path, key, value);
        logEvent($"Protocol '{protocolName}' registered.");

        var regValue = EdgeRegValue(protocolName);
        SetRegistryKey(Registry.LocalMachine.Name, regValue.Path, regValue.Key, regValue.Value);

        regValue = ChromeRegValue(protocolName);
        SetRegistryKey(Registry.LocalMachine.Name, regValue.Path, regValue.Key, regValue.Value);
    }

    public static void UnRegister(string protocolName, Action<string> logEvent)
    {
        Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
        logEvent($"Protocol '{protocolName}' unregistered.");

        var regValue = EdgeRegValue(protocolName);
        var subKey = Registry.LocalMachine.OpenSubKey(regValue.Path, true);
        subKey?.DeleteValue(regValue.Key, false);

        regValue = ChromeRegValue(protocolName);
        subKey = Registry.LocalMachine.OpenSubKey(regValue.Path, true);
        subKey?.DeleteValue(regValue.Key, false);
    }

    public static void Launch(LaunchApplication launchApplication, Action<string> logEvent)
    {
        var processStartParams = new ProcessStartParams(launchApplication.Command);
        Process.Run(processStartParams, launchApplication.Args, logEvent);
    }

    private static void SetRegistryKey(string baseKey, string path, string? key, string value)
    {
        Registry.SetValue($"{baseKey}\\{path}", key, value);
    }
}