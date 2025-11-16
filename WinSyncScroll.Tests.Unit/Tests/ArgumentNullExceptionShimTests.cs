using WinSyncScroll.Common.Shim;

namespace WinSyncScroll.Tests.Unit.Tests;

public class ArgumentNullExceptionShimTests
{
    [Test]
    public void ThrowIfNull_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        object? value = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            ArgumentNullExceptionShim.ThrowIfNull(value));

        Assert.That(ex.ParamName, Is.EqualTo("value"));
    }

    [Test]
    public void ThrowIfNull_WithNonNullValue_DoesNotThrow()
    {
        // Arrange
        var value = new object();

        // Act & Assert
        Assert.DoesNotThrow(() =>
            ArgumentNullExceptionShim.ThrowIfNull(value));
    }
}
