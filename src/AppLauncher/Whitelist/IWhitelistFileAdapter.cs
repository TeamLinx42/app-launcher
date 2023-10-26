namespace AppLauncher.Whitelist;

internal interface IWhitelistFileAdapter
{
    public bool WhitelistExists(WhitelistFilePath whitelistFilePath);

    public Whitelist ReadWhitelist(WhitelistFilePath whitelistFilePath);
}