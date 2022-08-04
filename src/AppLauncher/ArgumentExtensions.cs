using System;
using System.Collections.Generic;
using System.Windows;

namespace AppLauncher;

internal static class ArgumentExtensions
{
    public static LaunchArgs Validate(this StartupEventArgs startupEventArgs)
    {
        var args = startupEventArgs.Args;
        if (args.Length < 1) return new LaunchArgs(false, ExitCode.MissingArgs);

        if (IsKnownCommand(args, "register"))
            return args.Length == 2
                ? new LaunchArgs(true, 1, LaunchCommand.Register, args[1])
                : new LaunchArgs(false, ExitCode.MissingProtocolNameArg);

        if (IsKnownCommand(args, "unregister"))
            return args.Length == 2
                ? new LaunchArgs(true, 1, LaunchCommand.UnRegister, args[1])
                : new LaunchArgs(false, ExitCode.MissingProtocolNameArg);

        return new LaunchArgs(false, ExitCode.UnknownCommand, LaunchCommand.Unknown);
    }

    public static bool IsAdministrativeCommand(this LaunchArgs launchArgs)
    {
        return launchArgs.LaunchCommand is LaunchCommand.Register or LaunchCommand.UnRegister;
    }

    private static bool IsKnownCommand(IReadOnlyList<string> args, string command)
    {
        return args[0].Equals(command, StringComparison.OrdinalIgnoreCase);
    }

    private static class ExitCode
    {
        public const int MissingArgs = 1;
        public const int MissingProtocolNameArg = 2;
        public const int UnknownCommand = 100;
    }
}