using System;
using System.IO;

namespace AppLauncher.Whitelist;

internal class WhitelistFileAdapter : IWhitelistFileAdapter
{
    public bool WhitelistExists(WhitelistFilePath whitelistFilePath) => File.Exists(whitelistFilePath.Path);

    public Whitelist ReadWhitelist(WhitelistFilePath whitelistFilePath)
    {
        if (whitelistFilePath.Path == null || !whitelistFilePath.IsConfigured)
            throw new InvalidOperationException("Path to whitelist not configured.");

        return new Whitelist(File.ReadAllLines(whitelistFilePath.Path));
    }
}