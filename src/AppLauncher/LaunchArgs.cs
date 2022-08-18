namespace AppLauncher;

internal record LaunchArgs(bool IsValid, int ExitCode, LaunchApplication LaunchApplication)
{
    public LaunchArgs(bool isValid, int exitCode)
        : this(isValid, exitCode, LaunchApplication.Undefined)
    {
    }
}