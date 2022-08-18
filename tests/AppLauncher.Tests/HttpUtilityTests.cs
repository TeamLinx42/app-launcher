using System.Web;
using FluentAssertions;
using Xunit.Abstractions;

namespace AppLauncher.Tests;

public class HttpUtilityTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpUtilityTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("\"C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe\"?test?1?2?3")]
    [InlineData("C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?arg with spaces")]
    [InlineData("\"C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?arg with spaces\"")]
    public void UrlEncode_Decode_Roundtrip(string expected)
    {
        var urlEncode = HttpUtility.UrlEncode(expected);
        _testOutputHelper.WriteLine(urlEncode);

        var urlDecode = HttpUtility.UrlDecode(urlEncode);
        _testOutputHelper.WriteLine(urlDecode);

        urlDecode.Should().Be(expected);
    }

    [Theory]
    [InlineData(
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?arg with spaces",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?arg with spaces"
    )]
    [InlineData(
        "C%3a%5cTools%5cThe+One+And+Only+App+Launcher%5cAppLauncher.exe%3farg+with+spaces",
        "C:\\Tools\\The One And Only App Launcher\\AppLauncher.exe?arg with spaces"
    )]
    public void UrlDecode(string actual, string expected)
    {
        var urlDecode = HttpUtility.UrlDecode(actual);
        _testOutputHelper.WriteLine(urlDecode);

        urlDecode.Should().Be(expected);
    }

    [Theory]
    [InlineData("a:\\b c\\d e f.exe?arg with space?arg2?333", "a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333")]
    [InlineData("\"a:\\b c\\d e f.exe?arg with space?arg2?333\"", "%22a%3a%5cb+c%5cd+e+f.exe%3farg+with+space%3farg2%3f333%22")]
    public void UrlEncode(string actual, string expected)
    {
        var urlEncode = HttpUtility.UrlEncode(actual);
        _testOutputHelper.WriteLine(urlEncode);

        urlEncode.Should().Be(expected);
    }
}