using FluentAssertions;

namespace AppLauncher.Tests;

public class ProtocolDecoderTests
{
    [Fact]
    public void IsLaunchProtocol_Null_Exception()
    {
        var actual = () => ProtocolDecoder.IsLaunchProtocol(null!);

        actual.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("x")]
    [InlineData("xyz")]
    [InlineData(":")]
    [InlineData("xyz:")]
    [InlineData("123:")]
    [InlineData("//")]
    [InlineData("://")]
    [InlineData("//abc")]
    [InlineData("://abc")]
    public void IsLaunchProtocol_NotAProtocol_False(string arg)
    {
        var actual = ProtocolDecoder.IsLaunchProtocol(arg);

        actual.Should().BeFalse();
    }

    [Theory]
    [InlineData("x://")]
    [InlineData("1://")]
    [InlineData("xyz://")]
    [InlineData("123://")]
    [InlineData("xyz://abc")]
    [InlineData("xyz://\"abc\"")]
    [InlineData("xyz://abc?def?ghi")]
    [InlineData("tlx://\"a:\\b c\\d e f.exe?arg with space?arg2?333\"")]
    [InlineData("\"tlx://a:\\b c\\d e f.exe?arg with space?arg2?333\"")]
    public void IsLaunchProtocol_IsProtocol_True(string arg)
    {
        var actual = ProtocolDecoder.IsLaunchProtocol(arg);

        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData("x://", "")]
    [InlineData("1://", "")]
    [InlineData("xyz://", "")]
    [InlineData("123://", "")]
    [InlineData("xyz://abc", "abc")]
    [InlineData("xyz://\"abc\"", "\"abc\"")]
    [InlineData("xyz://abc?def?ghi", "abc?def?ghi")]
    [InlineData("tlx://\"a:\\b c\\d e f.exe?arg with space?arg2?333\"", "\"a:\\b c\\d e f.exe?arg with space?arg2?333\"")]
    [InlineData("tlx://a:\\b c\\d e f.exe?arg with space?arg2?333", "a:\\b c\\d e f.exe?arg with space?arg2?333")]
    [InlineData("tlx://a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333", "a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333")]
    [InlineData("tlx://%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22", "%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22")]
    public void StripProtocol(string arg, string expected)
    {
        var actual = ProtocolDecoder.StripProtocol(arg);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("unencoded protocol", "unencoded protocol")]
    [InlineData("unencoded protocol?with arg", "unencoded protocol?with arg")]
    [InlineData("a%3a%5cb+c%5cd+e+f.exe", "a:\\b c\\d e f.exe")]
    [InlineData("a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333", "a:\\b c\\d e f.exe?arg with space?arg2?333")]
    [InlineData("%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22", "a:\\b c\\d e f.exe?arg with space?arg2?333")]
    public void DecodeProtocol(string encodedProtocol, string expected)
    {
        var actual = ProtocolDecoder.DecodeProtocol(encodedProtocol);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(
        "app",
        "app",
        new string[] { }
    )]
    [InlineData(
        "app?",
        "app",
        new[] { "" }
    )]
    [InlineData(
        "app?arg",
        "app",
        new[] { "arg" }
    )]
    [InlineData(
        "a:\\bc\\def.exe?arg?arg2?333",
        "a:\\bc\\def.exe",
        new[] { "arg", "arg2", "333" }
    )]
    [InlineData(
        "a:\\b c\\d e f.exe?arg with space?arg2?333",
        "a:\\b c\\d e f.exe",
        new[] { "arg with space", "arg2", "333" }
    )]
    public void DecodeArgs(string decodedProtocol, string expectedPath, string[] expectedArgs)
    {
        var actual = ProtocolDecoder.DecodeArgs(decodedProtocol);

        actual.Path.Should().BeEquivalentTo(expectedPath);
        actual.Args.Should().BeEquivalentTo(expectedArgs);
    }

    [Theory]
    [InlineData("unencoded protocol", "unencoded protocol", new string[] { })]
    [InlineData("unencoded protocol?with arg", "unencoded protocol", new[] { "with arg" })]
    [InlineData("a%3a%5cb+c%5cd+e+f.exe", "a:\\b c\\d e f.exe", new string[] { })]
    [InlineData("a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333", "a:\\b c\\d e f.exe", new[] { "arg with space", "arg2", "333" })]
    [InlineData("%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22", "a:\\b c\\d e f.exe", new[] { "arg with space", "arg2", "333" })]
    public void DecodeApplication(string encodedProtocol, string expectedCommand, string[] expectedArgs)
    {
        var expected = LaunchApplication.Launch(expectedCommand, expectedArgs);

        var actual = ProtocolDecoder.DecodeApplication(encodedProtocol);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Decode_Null_Exception()
    {
        var actual = () => ProtocolDecoder.Decode(null!);

        actual.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Decode_Empty_Exception()
    {
        var actual = () => ProtocolDecoder.Decode(string.Empty);

        actual.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void Decode_EmptyUi_Exception()
    {
        var actual = () => ProtocolDecoder.Decode("abc://");

        actual.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(
        "tlx://unencoded protocol",
        "unencoded protocol",
        new string[] { }
    )]
    [InlineData(
        "tlx://unencoded protocol?with arg",
        "unencoded protocol",
        new[] { "with arg" }
    )]
    [InlineData(
        "tlx://a%3a%5cb+c%5cd+e+f.exe", "a:\\b c\\d e f.exe",
        new string[] { }
    )]
    [InlineData(
        "tlx://a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333",
        "a:\\b c\\d e f.exe",
        new[] { "arg with space", "arg2", "333" }
    )]
    [InlineData(
        "tlx://%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22",
        "a:\\b c\\d e f.exe",
        new[] { "arg with space", "arg2", "333" }
    )]
    [InlineData(
        "tlx://C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?test?Hallo Welt!",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe",
        new[] { "test", "Hallo Welt!" }
    )]
    [InlineData(
        "tlx://C:%5CTools%5CThe%20One%20And%20Only%20App%20Launcher%5CAppLauncher.exe/?test?Hallo%20Welt!",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe",
        new[] { "test", "Hallo Welt!" }
    )]
    [InlineData(
        "tlx://%22C:%5CTools%5CThe%20One%20And%20Only%20App%20Launcher%5CAppLauncher.exe/?test?Hallo%20Welt!%22",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe",
        new[] { "test", "Hallo Welt!" }
    )]
    [InlineData(
        "tlx://C:%5CTools%5CThe%20One%20And%20Only%20App%20Launcher%5CAppLauncher.exe?test?Hallo%20Welt!",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe",
        new[] { "test", "Hallo Welt!" }
    )]
    public void Decode(string protocol, string expectedCommand, string[] expectedArgs)
    {
        var expected = LaunchApplication.Launch(expectedCommand, expectedArgs);

        var actual = ProtocolDecoder.Decode(protocol);

        actual.Should().BeEquivalentTo(expected);
    }
}