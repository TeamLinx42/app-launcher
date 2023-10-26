using AppLauncher.Whitelist;
using FluentAssertions;

namespace AppLauncher.Tests.Whitelist;

public class WhitelistFileAdapterTests
{
    [Fact]
    public void ReadWhitelist_NullPath_Exception()
    {
        var sut = new WhitelistFileAdapter();
        var whitelistFilePath = new WhitelistFilePath(null);

        // Act
        var actual = () => sut.ReadWhitelist(whitelistFilePath);

        // Assert
        actual.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ReadWhitelist_EmptyPath_Exception()
    {
        var sut = new WhitelistFileAdapter();
        var whitelistFilePath = new WhitelistFilePath(string.Empty);

        // Act
        var actual = () => sut.ReadWhitelist(whitelistFilePath);

        // Assert
        actual.Should().Throw<InvalidOperationException>();
    }
}