using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Iks.BinaryToolkit.UnsafeToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test.UnsafeToolkit;

public class PinTest(ITestOutputHelper output)
{
    // pin must be not null
    [Fact]
    public void NotNullPin()
    {
        try
        {
            // set a null ref
            string str = null!;
            using var pin = str.AsPinned();
        }
        // throws is right,null cannot be pinned
        catch (Exception e)
        {
            return;
        }
        Assert.Fail();
    }
    
    [Fact]
    public void NotUseAfterRelease()
    {
        var pin = new object().AsPinned();
        pin.Dispose();
        // use after release
        try
        {
            var pinTarget = pin.Target;
        }
        catch 
        {
            return;
        }
        Assert.Fail();
    }
    [Fact]
    public void NotMultipleRelease()
    {
        var pin = new object().AsPinned();
        pin.Dispose();
        // release again
        try
        {
            pin.Dispose();
        }
        catch 
        {
            return;
        }
        Assert.Fail();
    }
    
    [Fact]
    public void CopyRelease()
    {
        var pin_old = new object().AsPinned();
        var pin_new = pin_old;
        pin_new.Dispose();
    }

    
}