using System;
using System.Collections.Generic;

namespace AppLauncher;

internal record LaunchArgs(bool IsValid, int ExitCode, LaunchCommand LaunchCommand, IEnumerable<string> Parameters)
{
    public LaunchArgs(bool isValid, int exitCode)
        : this(isValid, exitCode, LaunchCommand.Undefined, Array.Empty<string>())
    {
    }

    public LaunchArgs(bool isValid, int exitCode, LaunchCommand launchCommand, string parameter)
        : this(isValid, exitCode, launchCommand, new[] { parameter })
    {
    }

    public LaunchArgs(bool isValid, int exitCode, LaunchCommand launchCommand)
        : this(isValid, exitCode, launchCommand, Array.Empty<string>())
    {
    }
}