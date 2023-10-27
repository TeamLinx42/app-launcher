using AppLauncher.Whitelist;
using FluentAssertions;
using Moq;

namespace AppLauncher.Tests;

public class ArgumentExtensionsTests
{
    [Fact]
    public void IsValid_WhitelistNotConfigured_True()
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, "any");
        var whitelistFilePath = new WhitelistFilePath(null);
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);

        var actual = ArgumentExtensions.IsValid(launchApplication, whitelistFilePath, whitelistFileAdapter.Object);

        actual.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WhitelistConfiguredButNotFound_Exception()
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, "any");
        var whitelistFilePath = new WhitelistFilePath("path");
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(whitelistFilePath.Path))
            .Returns(false);

        var actual = () => ArgumentExtensions.IsValid(launchApplication, whitelistFilePath, whitelistFileAdapter.Object);

        actual.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void IsValid_WhitelistConfiguredButNotWhitelisted_False()
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, "app1");
        var whitelistFilePath = new WhitelistFilePath("path");
        var whitelistedApps = new[]
        {
            "app2"
        };
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(whitelistFilePath.Path))
            .Returns(true);
        whitelistFileAdapter
            .Setup(adapter => adapter.ReadWhitelist(whitelistFilePath.Path))
            .Returns(whitelistedApps);

        var actual = ArgumentExtensions.IsValid(launchApplication, whitelistFilePath, whitelistFileAdapter.Object);

        actual.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WhitelistConfiguredAndWhitelisted_True()
    {
        var launchApplication = new LaunchApplication(LaunchCommandType.Launch, "app1");
        var whitelistFilePath = new WhitelistFilePath("path");
        var whitelistedApps = new[]
        {
            "app1"
        };
        var whitelistFileAdapter = new Mock<IWhitelistFileAdapter>(MockBehavior.Strict);
        whitelistFileAdapter
            .Setup(adapter => adapter.WhitelistExists(whitelistFilePath.Path))
            .Returns(true);
        whitelistFileAdapter
            .Setup(adapter => adapter.ReadWhitelist(whitelistFilePath.Path))
            .Returns(whitelistedApps);

        var actual = ArgumentExtensions.IsValid(launchApplication, whitelistFilePath, whitelistFileAdapter.Object);

        actual.Should().BeTrue();
    }
}