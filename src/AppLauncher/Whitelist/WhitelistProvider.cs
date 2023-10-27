using System;
using System.IO;
using System.Linq;

namespace AppLauncher.Whitelist;

internal static class WhitelistProvider
{
    public static bool IsValid(LaunchApplication launchApplication, Whitelist whitelist) =>
        !whitelist.IsConfigured ||
        Contains(launchApplication.Command, whitelist) ||
        ContainsFullPath(launchApplication.Command, whitelist);

    public static Whitelist ReadWhitelist(WhitelistFilePath whitelistFilePath, IWhitelistFileAdapter whitelistFileAdapter)
    {
        if (!whitelistFilePath.IsConfigured)
            return Whitelist.NotConfigured;

        if (!whitelistFileAdapter.WhitelistExists(whitelistFilePath.Path))
            throw new FileNotFoundException("Registered whitelist not found.", whitelistFilePath.Path);

        return new Whitelist(whitelistFileAdapter.ReadWhitelist(whitelistFilePath.Path));
    }

    private static bool Contains(string command, Whitelist whitelist) =>
        whitelist.Entries
            .Contains(command, StringComparer.OrdinalIgnoreCase);

    private static bool ContainsFullPath(string command, Whitelist whitelist)
    {
        var commandFullPath = Path.GetFullPath(command);
        return whitelist.Entries
            .Select(Path.GetFullPath)
            .Contains(commandFullPath, StringComparer.OrdinalIgnoreCase);
    }
}