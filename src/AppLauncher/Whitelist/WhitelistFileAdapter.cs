using System;
using System.IO;

namespace AppLauncher.Whitelist;

internal class WhitelistFileAdapter : IWhitelistFileAdapter
{
    public string[] ReadWhitelist(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException();

        return File.ReadAllLines(filePath);
    }

    public bool WhitelistExists(string? filePath) => File.Exists(filePath);
}