using AppLauncher.Whitelist;
using FluentAssertions;

namespace AppLauncher.Tests.Whitelist;

public class WhitelistFilePathTests
{
    [Fact]
    public void IsConfigured_NullPath_False()
    {
        // Act
        var actual = new WhitelistFilePath(null).IsConfigured;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsConfigured_EmptyPath_False()
    {
        // Act
        var actual = new WhitelistFilePath(string.Empty).IsConfigured;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsConfigured_ValidPath_True()
    {
        // Act
        var actual = new WhitelistFilePath("path").IsConfigured;

        // Assert
        actual.Should().BeTrue();
    }
}