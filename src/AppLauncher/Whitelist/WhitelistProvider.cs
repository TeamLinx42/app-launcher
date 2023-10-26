using System;
using System.IO;
using System.Linq;

namespace AppLauncher.Whitelist;

internal static class WhitelistProvider
{
    public static bool IsValid(LaunchApplication launchApplication, Whitelist whitelist) =>
        Contains(launchApplication.Command, whitelist) ||
        ContainsFullPath(launchApplication.Command, whitelist);

    public static Whitelist ReadWhitelist(WhitelistFilePath whitelistFilePath, IWhitelistFileAdapter whitelistFileAdapter)
    {
        if (!whitelistFilePath.IsConfigured)
            throw new ArgumentNullException(nameof(whitelistFilePath));

        if (!whitelistFileAdapter.WhitelistExists(whitelistFilePath))
            throw new FileNotFoundException("Registered whitelist not found.", whitelistFilePath.Path);

        return whitelistFileAdapter.ReadWhitelist(whitelistFilePath);
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