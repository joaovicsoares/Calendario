using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace EventCalendar.Tests;

public class VerifySetup
{
    [Fact]
    public void XUnit_Works()
    {
        Assert.True(true);
    }

    [Property]
    public Property FsCheck_Works(int x)
    {
        return (x + 0 == x).ToProperty();
    }
}
