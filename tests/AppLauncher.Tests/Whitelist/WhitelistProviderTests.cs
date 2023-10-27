using AppLauncher.Whitelist;
using FluentAssertions;
using Moq;

namespace AppLauncher.Tests.Whitelist;

public class WhitelistProviderTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void IsValid_ApplicationToLaunchIsWhitelisted_True(AppLauncher.Whitelist.Whitelist whitelist, string command, bool expected)
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, command);

        var actual = WhitelistProvider.IsValid(launchApplication, whitelist);

        actual.Should().Be(expected);
    }

    [Fact]
    public void IsValid_WhitelistNotConfigured_True()
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, "any");

        var whitelist = AppLauncher.Whitelist.Whitelist.NotConfigured;
        var actual = WhitelistProvider.IsValid(launchApplication, whitelist);

        actual.Should().Be(true);
    }

    [Fact]
    public void ReadWhitelist_WhitelistFilePathNull_WhitelistNotConfigured()
    {
        // Arrange
        var whitelistFilePath = new WhitelistFilePath(null);
        var expected = AppLauncher.Whitelist.Whitelist.NotConfigured;

        // Act
        var actual = WhitelistProvider.ReadWhitelist(whitelistFilePath, default!);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ReadWhitelist_WhitelistFilePathEmpty_WhitelistNotConfigured()
    {
        // Arrange
        var whitelistFilePath = new WhitelistFilePath(string.Empty);
        var expected = AppLauncher.Whitelist.Whitelist.NotConfigured;

        // Act
        var actual = WhitelistProvider.ReadWhitelist(whitelistFilePath, default!);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ReadWhitelist_InvalidPath_Exception()
    {
        // Arrange
        var whitelistFilePath = new WhitelistFilePath("some/invalid/path");
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(whitelistFilePath.Path))
            .Returns(false);

        // Act
        var actual = () => WhitelistProvider.ReadWhitelist(whitelistFilePath, whitelistFileAdapter.Object);

        // Assert
        actual.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void ReadWhitelist_ValidPath_ListOfValidApplication()
    {
        // Arrange
        var expected = new AppLauncher.Whitelist.Whitelist(new List<string>
        {
            "app1",
            "app2"
        });
        var whitelistFilePath = new WhitelistFilePath("some/path");
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(whitelistFilePath.Path))
            .Returns(true);
        whitelistFileAdapter
            .Setup(adapter => adapter.ReadWhitelist(whitelistFilePath.Path))
            .Returns(expected.Entries.ToArray);

        // Act
        var actual = WhitelistProvider.ReadWhitelist(whitelistFilePath, whitelistFileAdapter.Object);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<object[]> TestData()
    {
        // simple whitelisted
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { "app1" }), "app1", true };

        // simple not whitelisted
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { "app1" }), "app2", false };

        // multiple whitelisted
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { "app1", "app2", "app3" }), "app3", true };

        // multiple not whitelisted
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { "app1", "app2", "app3" }), "app4", false };

        // canonical whitelisted
        var path1 = @"C:\Folder\..\Folder\File.txt";
        var path2 = @"c:\folder\File.txt";
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C:/Folder/File.txt";
        path2 = @"C:\folder\File.txt";
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C:\\Folder//File.txt";
        path2 = @"C:\folder\File.txt";
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { path1 }), path2, true };

        // canonical whitelisted
        path1 = @"C://Tools//App Launcher//AppLauncher.exe";
        path2 = @"C:\Tools\App Launcher\AppLauncher.exe";
        yield return new object[] { new AppLauncher.Whitelist.Whitelist(true, new[] { path1 }), path2, true };
    }
}