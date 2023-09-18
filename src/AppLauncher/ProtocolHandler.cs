using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace AppLauncher;

internal static class ProtocolHandler
{
    private const string UrlAllowListEntryIndex = "99";
    private const string AppRegPath = "SOFTWARE\\AppLauncher";
    private const string WhitelistFilePathRegKey = "WhitelistFilePath";

    private static IEnumerable<(string, string?, string )> ProtocolRegValues(string protocolName, string appLauncherLocation)
    {
        yield return ($@"{protocolName}", null, $"{protocolName}:// protocol");
        yield return ($@"{protocolName}", "URL Protocol", "");
        yield return ($@"{protocolName}\DefaultIcon", null, $"{appLauncherLocation},1");
        yield return ($@"{protocolName}\shell\open\command", null, $"{appLauncherLocation} \"%1\"");
    }

    private static (string Path, string Key, string Value) EdgeRegValue(string protocolName) =>
        (@"SOFTWARE\Policies\Microsoft\Edge\URLAllowlist", UrlAllowListEntryIndex, $"{protocolName}://*");

    private static (string Path, string Key, string Value) ChromeRegValue(string protocolName) =>
        (@"SOFTWARE\Policies\Google\Chrome\URLAllowlist", UrlAllowListEntryIndex, $"{protocolName}://*");

    //private static string AllowedOriginsValue(string protocolName) => "[{\"allowed_origins\":[\"*\"],\"protocol\":\"" + protocolName + "\"}]";

    public static void Register(string protocolName, string whitelistFilePath, Action<string> logEvent)
    {
        var appLauncherLocation = Process.ExeFileName ?? throw new InvalidOperationException("Unable to get AppLauncher location");

        AddProtocolSettings(protocolName, logEvent, appLauncherLocation);
        AddBrowserSettings(EdgeRegValue(protocolName), "Edge", logEvent);
        AddBrowserSettings(ChromeRegValue(protocolName), "Chrome", logEvent);
        AddWhitelistSettings(whitelistFilePath, logEvent);
    }

    public static void UnRegister(string protocolName, Action<string> logEvent)
    {
        RemoveProtocolSettings(protocolName, logEvent);
        RemoveBrowserSettings(EdgeRegValue(protocolName), "Edge", logEvent);
        RemoveBrowserSettings(ChromeRegValue(protocolName), "Chrome", logEvent);
        RemoveWhitelistSettings(logEvent);
    }

    public static void Launch(LaunchApplication launchApplication, Action<string> logEvent)
    {
        var processStartParams = new ProcessStartParams(launchApplication.Command);
        Process.Run(processStartParams, launchApplication.Args, logEvent);
    }

    public static string? GetWhitelistFilePath() => GetRegistryValue(Registry.LocalMachine.Name, AppRegPath, WhitelistFilePathRegKey, string.Empty);

    private static void AddWhitelistSettings(string whitelistFilePath, Action<string> logEvent)
    {
        if (string.IsNullOrEmpty(whitelistFilePath)) return;

        SetRegistryKey(Registry.LocalMachine.Name, AppRegPath, WhitelistFilePathRegKey, whitelistFilePath);
        logEvent($"Whitelist '{whitelistFilePath}' registered.");
    }

    private static void RemoveWhitelistSettings(Action<string> logEvent)
    {
        Registry.LocalMachine.DeleteSubKeyTree(AppRegPath);
        logEvent("Whitelist unregistered.");
    }

    private static void AddProtocolSettings(string protocolName, Action<string> logEvent, string appLauncherLocation)
    {
        foreach (var (path, key, value) in ProtocolRegValues(protocolName, appLauncherLocation))
            SetRegistryKey(Registry.ClassesRoot.Name, path, key, value);
        logEvent($"Protocol '{protocolName}' registered.");
    }

    private static void AddBrowserSettings((string Path, string Key, string Value) regValue, string browser, Action<string> logEvent)
    {
        SetRegistryKey(Registry.LocalMachine.Name, regValue.Path, regValue.Key, regValue.Value);
        logEvent($"Browser settings for '{browser}' registered.");
    }

    private static void RemoveProtocolSettings(string protocolName, Action<string> logEvent)
    {
        Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
        logEvent($"Protocol '{protocolName}' unregistered.");
    }

    private static void RemoveBrowserSettings((string Path, string Key, string Value) regValue, string browser, Action<string> logEvent)
    {
        var subKey = Registry.LocalMachine.OpenSubKey(regValue.Path, true);
        subKey?.DeleteValue(regValue.Key, false);
        logEvent($"Browser settings for '{browser}' unregistered.");
    }

    private static void SetRegistryKey(string baseKey, string path, string? key, string value)
    {
        Registry.SetValue($"{baseKey}\\{path}", key, value);
    }

    private static string? GetRegistryValue(string baseKey, string path, string? key, string defaultValue) => (string?)Registry.GetValue($"{baseKey}\\{path}", key, defaultValue);
}