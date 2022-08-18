using FluentAssertions;

namespace AppLauncher.Tests;

public class LaunchApplicationTests
{
    [Fact]
    public void ToString_Undefined()
    {
        var actual = LaunchApplication.Undefined.ToString();

        actual.Should().Be("{\"Type\":\"Undefined\",\"Command\":\"\",\"Args\":null}");
    }

    [Fact]
    public void ToString_Register()
    {
        var actual = LaunchApplication.Register("protocol").ToString();

        actual.Should().Be("{\"Type\":\"Register\",\"Command\":\"protocol\",\"Args\":null}");
    }

    [Fact]
    public void ToString_UnRegister()
    {
        var actual = LaunchApplication.UnRegister("protocol").ToString();

        actual.Should().Be("{\"Type\":\"UnRegister\",\"Command\":\"protocol\",\"Args\":null}");
    }

    [Fact]
    public void ToString_Launch()
    {
        const string command = "c:\\abc\\def\\ghi.jkl";
        var args = new[] { "1", "2", "3" };
        var actual = LaunchApplication.Launch(command, args).ToString();

        actual.Should().Be("{\"Type\":\"Launch\",\"Command\":\"c:\\\\abc\\\\def\\\\ghi.jkl\",\"Args\":[\"1\",\"2\",\"3\"]}");
    }
}