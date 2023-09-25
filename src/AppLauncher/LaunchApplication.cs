using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppLauncher;

internal record LaunchApplication(LaunchCommandType Type, string Command, IEnumerable<string>? Args = default)
{
    public static readonly LaunchApplication Undefined = new(LaunchCommandType.Undefined, string.Empty);

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static LaunchApplication Register(string command) => new(LaunchCommandType.Register, command);

    public static LaunchApplication Register(string command, string pathToWhitelist) => new(LaunchCommandType.Register, command, new[] { pathToWhitelist });

    public static LaunchApplication UnRegister(string command) => new(LaunchCommandType.UnRegister, command);

    public static LaunchApplication Test(IEnumerable<string> args) => new(LaunchCommandType.Test, "test", args);

    public static LaunchApplication Launch(string command, IEnumerable<string>? args) => new(LaunchCommandType.Launch, command, args);

    public override string ToString() => JsonSerializer.Serialize(this, _jsonSerializerOptions);
}