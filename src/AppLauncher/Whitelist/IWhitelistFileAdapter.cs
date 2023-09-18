namespace AppLauncher.Whitelist;

internal interface IWhitelistFileAdapter
{
    public bool WhitelistExists(string pathToWhitelist);

    public Whitelist ReadWhitelist(string pathToWhitelist);
}