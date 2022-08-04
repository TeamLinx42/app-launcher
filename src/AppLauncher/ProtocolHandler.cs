using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace AppLauncher;

internal static class ProtocolHandler
{
    private static IEnumerable<(string, string?, string)> RegValues(string protocolName, string appLauncherLocation)
    {
        yield return ($@"HKEY_CLASSES_ROOT\{protocolName}", null, $"{protocolName}:// protocol");
        yield return ($@"HKEY_CLASSES_ROOT\{protocolName}", "URL Protocol", "");
        yield return ($@"HKEY_CLASSES_ROOT\{protocolName}\DefaultIcon", null, $"{appLauncherLocation},1");
        yield return ($@"HKEY_CLASSES_ROOT\{protocolName}\shell\open\command", null, $"{appLauncherLocation} \"%1\"");
    }

    public static void Register(IEnumerable<string> launchArgs)
    {
        var appLauncherLocation = Process.ExeFileName;
        if (appLauncherLocation == null) throw new InvalidOperationException("Unable to get AppLauncher location");

        var protocolName = launchArgs.Single();

        foreach (var (path, key, value) in RegValues(protocolName, appLauncherLocation)) SetRegistryKey(path, key, value);
    }

    public static void UnRegister(IEnumerable<string> launchArgs)
    {
        var protocolName = launchArgs.Single();
        DeleteRegistryTree(protocolName);
    }

    private static void DeleteRegistryTree(string protocolName)
    {
        Registry.ClassesRoot.DeleteSubKeyTree(protocolName, false);
    }

    private static void SetRegistryKey(string path, string? key, string value)
    {
        Registry.SetValue(path, key, value);
    }
}