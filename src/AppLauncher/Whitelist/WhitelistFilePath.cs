namespace AppLauncher.Whitelist;

public record WhitelistFilePath(string? Path)
{
    public bool IsConfigured => !string.IsNullOrEmpty(Path);
};