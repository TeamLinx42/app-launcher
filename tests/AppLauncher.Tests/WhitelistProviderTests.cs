using AppLauncher.Whitelist;
using FluentAssertions;
using Moq;

namespace AppLauncher.Tests;

public class WhitelistProviderTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void IsValid_ApplicationToLaunchIsWhitelisted_True(Whitelist.Whitelist whitelist, string command, bool expected)
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, command);

        var actual = WhitelistProvider.IsValid(launchApplication, whitelist);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ReadWhitelist_NullPath_EmptyList()
    {
        // Act
        var actual = WhitelistProvider.ReadWhitelist(null, default!);

        // Assert
        actual.Entries.Should().BeEmpty();
    }

    [Fact]
    public void ReadWhitelist_EmptyPath_EmptyList()
    {
        // Act
        var actual = WhitelistProvider.ReadWhitelist(string.Empty, default!);

        // Assert
        actual.Entries.Should().BeEmpty();
    }

    [Fact]
    public void ReadWhitelist_InvalidPath_Exception()
    {
        // Arrange
        const string pathToWhitelist = "some/invalid/path";
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(pathToWhitelist))
            .Returns(false);

        // Act
        var actual = () => WhitelistProvider.ReadWhitelist(pathToWhitelist, whitelistFileAdapter.Object);

        // Assert
        actual.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void ReadWhitelist_ValidPath_ListOfValidApplication()
    {
        // Arrange
        var expected = new Whitelist.Whitelist(new List<string>
        {
            "app1",
            "app2"
        });
        const string pathToWhitelist = "some/path";
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(pathToWhitelist))
            .Returns(true);
        whitelistFileAdapter
            .Setup(adapter => adapter.ReadWhitelist(pathToWhitelist))
            .Returns(expected);

        // Act
        var actual = WhitelistProvider.ReadWhitelist(pathToWhitelist, whitelistFileAdapter.Object);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<object[]> TestData()
    {
        // simple whitelisted
        yield return new object[] { new Whitelist.Whitelist(new[] { "app1" }), "app1", true };

        // simple not whitelisted
        yield return new object[] { new Whitelist.Whitelist(new[] { "app1" }), "app2", false };

        // multiple whitelisted
        yield return new object[] { new Whitelist.Whitelist(new[] { "app1", "app2", "app3" }), "app3", true };

        // multiple not whitelisted
        yield return new object[] { new Whitelist.Whitelist(new[] { "app1", "app2", "app3" }), "app4", false };

        // canonical whitelisted
        var path1 = @"C:\Folder\..\Folder\File.txt";
        var path2 = @"c:\folder\File.txt";
        yield return new object[] { new Whitelist.Whitelist(new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C:/Folder/File.txt";
        path2 = @"C:\folder\File.txt";
        yield return new object[] { new Whitelist.Whitelist(new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C:\\Folder//File.txt";
        path2 = @"C:\folder\File.txt";
        yield return new object[] { new Whitelist.Whitelist(new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C://Tools//App Launcher//AppLauncher.exe";
        path2 = @"C:\Tools\App Launcher\AppLauncher.exe";
        yield return new object[] { new Whitelist.Whitelist(new[] { path1 }), path2, true };
    }
}