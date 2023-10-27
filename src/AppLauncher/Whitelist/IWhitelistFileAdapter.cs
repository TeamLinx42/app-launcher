namespace AppLauncher.Whitelist;

internal interface IWhitelistFileAdapter
{
    public bool WhitelistExists(string? filePath);

    public string[] ReadWhitelist(string? filePath);
}