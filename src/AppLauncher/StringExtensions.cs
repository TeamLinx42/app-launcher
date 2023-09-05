using System.Collections.Generic;
using System.Linq;

namespace AppLauncher;

internal static class StringExtensions
{
    public static string CreateMessage(this string context, string arg) => $"{context}: {arg}";

    public static string CreateMessage(this string context, IEnumerable<string> args) => $"{context}:".Join(args);

    public static string Join(this string text, string argsContext, IEnumerable<string> args)
    {
        var argsArray = args.ToArray();
        return argsArray.Any()
            ? $"{text} {argsContext}{string.Join(' ', argsArray)}"
            : text;
    }

    public static string Join(this string text, IEnumerable<string> args) => text.Join(string.Empty, args);
}