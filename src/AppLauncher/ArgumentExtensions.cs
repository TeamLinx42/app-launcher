using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppLauncher.Whitelist;

namespace AppLauncher;

internal static class ArgumentExtensions
{
    public static LaunchArgs Validate(this StartupEventArgs startupEventArgs, Action<string> logEvent)
    {
        var args = startupEventArgs.Args;
        if (args.Length < 1) return new LaunchArgs(false, ExitCode.MissingArgs);

        if (IsKnownCommand(args, "register"))
            return args.Length == 2
                ? new LaunchArgs(true, ExitCode.Success, LaunchApplication.Register(args[1]))
                : new LaunchArgs(false, ExitCode.MissingProtocolNameArg);

        if (IsKnownCommand(args, "unregister"))
            return args.Length == 2
                ? new LaunchArgs(true, ExitCode.Success, LaunchApplication.UnRegister(args[1]))
                : new LaunchArgs(false, ExitCode.MissingProtocolNameArg);

        if (IsKnownCommand(args, "test")) return new LaunchArgs(true, ExitCode.Success, LaunchApplication.Test(args.Skip(1)));

        if (IsLaunchProtocol(args))
        {
            if (args.Length > 1)
                return new LaunchArgs(false, ExitCode.InvalidArgCount);

            logEvent("Protocol to decode".CreateMessage(args[0]));
            var launchApplication = ProtocolDecoder.Decode(args[0]);
            logEvent($"Decoded protocol: {launchApplication}");

            if (!IsWhitelistedApp(launchApplication))
            {
                logEvent("App is not whitelisted".CreateMessage(launchApplication.Command));
                return new LaunchArgs(false, ExitCode.NotInWhitelist);
            }

            return new LaunchArgs(true, ExitCode.Success, launchApplication);
        }

        return new LaunchArgs(false, ExitCode.UnknownCommand);
    }

    private static bool IsWhitelistedApp(LaunchApplication launchApplication) => WhitelistProvider.IsValid(launchApplication, GetWhitelist());

    private static Whitelist.Whitelist GetWhitelist()
    {
        IWhitelistFileAdapter whitelistFileAdapter = new WhitelistFileAdapter();
        var pathToWhitelist = ProtocolHandler.GetWhitelistFilePath();
        return WhitelistProvider.ReadWhitelist(pathToWhitelist, whitelistFileAdapter);
    }

    public static bool IsAdministrativeCommand(this LaunchArgs launchArgs) => launchArgs.LaunchApplication.Type is LaunchCommandType.Register or LaunchCommandType.UnRegister;

    private static bool IsKnownCommand(IReadOnlyList<string> args, string command) => args[0].Equals(command, StringComparison.OrdinalIgnoreCase);

    private static bool IsLaunchProtocol(IReadOnlyList<string> args) => ProtocolDecoder.IsLaunchProtocol(args[0]);

    private static class ExitCode
    {
        public const int Success = 0;
        public const int MissingArgs = 1;
        public const int MissingProtocolNameArg = 2;
        public const int InvalidArgCount = 3;
        public const int NotInWhitelist = 4;
        public const int UnknownCommand = 100;
    }
}