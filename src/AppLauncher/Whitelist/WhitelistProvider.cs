using System;
using System.IO;
using System.Linq;

namespace AppLauncher.Whitelist;

internal class WhitelistProvider
{
    public static bool IsValid(LaunchApplication launchApplication, Whitelist whitelist) =>
        Contains(launchApplication.Command, whitelist) ||
        ContainsFullPath(launchApplication.Command, whitelist);

    public static Whitelist ReadWhitelist(string? pathToWhitelist, IWhitelistFileAdapter whitelistFileAdapter)
    {
        if (string.IsNullOrEmpty(pathToWhitelist))
            return new Whitelist(Array.Empty<string>());

        if (!whitelistFileAdapter.WhitelistExists(pathToWhitelist))
            throw new FileNotFoundException("Registered whitelist not found.", pathToWhitelist);

        return whitelistFileAdapter.ReadWhitelist(pathToWhitelist);
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