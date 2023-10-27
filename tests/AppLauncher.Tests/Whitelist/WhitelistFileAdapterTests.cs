using AppLauncher.Whitelist;
using FluentAssertions;

namespace AppLauncher.Tests.Whitelist;

public class WhitelistFileAdapterTests
{
    [Fact]
    public void ReadWhitelist_NullPath_Exception()
    {
        // Arrange
        var sut = new WhitelistFileAdapter();

        // Act
        var actual = () => sut.ReadWhitelist(null);

        // Assert
        actual.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ReadWhitelist_EmptyPath_Exception()
    {
        // Arrange
        var sut = new WhitelistFileAdapter();

        // Act
        var actual = () => sut.ReadWhitelist(string.Empty);

        // Assert
        actual.Should().Throw<ArgumentNullException>();
    }
}