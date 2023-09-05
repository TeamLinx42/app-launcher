using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AppLauncher;

internal static class ProtocolDecoder
{
    private const string UriProtocolPattern = "^(([^:/?#]+)://)";

    public static bool IsLaunchProtocol(string arg)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        return Regex.IsMatch(arg, UriProtocolPattern);
    }

    public static LaunchApplication Decode(string protocol)
    {
        if (string.IsNullOrEmpty(protocol)) throw new ArgumentNullException(nameof(protocol));

        var encodedProtocol = StripProtocol(protocol);
        if (string.IsNullOrEmpty(encodedProtocol)) throw new InvalidOperationException("Protocol Uri must not be empty.");

        return DecodeApplication(encodedProtocol);
    }

    internal static string StripProtocol(string arg)
    {
        var splitUri = Regex.Split(arg, UriProtocolPattern);
        var strippedUri = splitUri[3];
        return strippedUri;
    }

    internal static LaunchApplication DecodeApplication(string encodedProtocol)
    {
        var decodedProtocol = DecodeProtocol(encodedProtocol);
        var (path, args) = DecodeArgs(decodedProtocol);
        return new LaunchApplication(LaunchCommandType.Launch, path, args);
    }

    internal static string DecodeProtocol(string encodedProtocol)
    {
        var decodedProtocol = HttpUtility.UrlDecode(encodedProtocol);
        return TrimDoubleQuotes(decodedProtocol);
    }

    internal static (string Path, IEnumerable<string> Args) DecodeArgs(string decodedProtocol)
    {
        var split = decodedProtocol.Split("?");
        var path = split[0].TrimEnd('/').TrimEnd('\\');
        var args = split.Skip(1);
        return (path, args);
    }

    private static string TrimDoubleQuotes(string text) => text.TrimStart('\"').TrimEnd('\"');
}