namespace AppLauncher.Tests;

public class WhitelistAdapterTests
{
    [Fact]
    public void IsValid_ApplicationToLaunchIsWhitelisted_True()
    {
        // Arrange
        var launchApplication = new LaunchApplication();
        var pathToWhitelist = "irgendwo/file.txt";
        var expected = true;

        // Act
        var actual = WhitelistAdapter.IsValid(launchApplication, pathToWhitelist);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void IsValid_ApplicationToLaunchIsNotWhitelisted_False()
    {

    }

    [Fact]
    public void ReadWhitelist_ValidPath_ListOfValidApplication()
    {
        // Arrange
        var pathToWhitelist = "irgendwo/file.txt";
        var expected = new List<string>()
        {
            "app1",
            "app2"
        };

        // Act
        var actual = WhitelistAdapter.ReadWhitelist(pathToWhitelist);

        // Assert
        actual.Should().Be(expected);
    }
}