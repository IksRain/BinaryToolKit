using Iks.BinaryToolkit.UnsafeToolkit;
using Xunit.Abstractions;

namespace BinaryKit_Test.UnsafeToolkit;

public class PinTest(ITestOutputHelper output)
{
    [Fact]
    public unsafe void NotNullPin()
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
            Assert.True(e is ArgumentNullException);
        }
    }
}