using System.IO;

namespace AppLauncher.Whitelist;

internal class WhitelistFileAdapter : IWhitelistFileAdapter
{
    public bool WhitelistExists(string pathToWhitelist) => File.Exists(pathToWhitelist);

    public Whitelist ReadWhitelist(string pathToWhitelist) => new(File.ReadAllLines(pathToWhitelist));
}