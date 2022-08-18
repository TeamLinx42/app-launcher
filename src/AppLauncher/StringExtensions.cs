using System.Collections.Generic;

namespace AppLauncher;

internal static class StringExtensions
{
    public static string CreateMessage(this string context, string arg)
    {
        return $"{context}: {arg}";
    }

    public static string CreateMessage(this string context, IEnumerable<string> args)
    {
        return $"{context}:".Join(args);
    }

    public static string Join(this string text, IEnumerable<string> args)
    {
        return $"{text} {string.Join(' ', args)}";
    }
}