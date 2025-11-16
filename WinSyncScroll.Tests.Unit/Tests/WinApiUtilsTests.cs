using WinSyncScroll.Common.Utils;
using WinSyncScroll.Tests.Unit.Utils;

namespace WinSyncScroll.Tests.Unit.Tests;

public class WinApiUtilsTests
{
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(short.MaxValue, 0)]
    [TestCase(0, short.MaxValue)]
    [TestCase(short.MaxValue, short.MaxValue)]
    [TestCase(ushort.MaxValue, 0)]
    [TestCase(0, ushort.MaxValue)]
    [TestCase(ushort.MaxValue, ushort.MaxValue)]
    public void GetHiLoWords_WhenGivenValidLParam_ReturnsExpectedValues(int expectedHiWord, int expectedLoWord)
    {
        var lParam = TestUtils.CreateLParam(expectedHiWord, expectedLoWord);
        var (actualLoWord, actualHiWord) = WinApiUtils.GetHiLoWords((uint)lParam);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actualLoWord, Is.EqualTo(expectedLoWord));
            Assert.That(actualHiWord, Is.EqualTo(expectedHiWord));
        }
    }

    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(short.MaxValue, 0)]
    [TestCase(0, short.MaxValue)]
    [TestCase(short.MaxValue, short.MaxValue)]
    [TestCase(ushort.MaxValue, 0)]
    [TestCase(0, ushort.MaxValue)]
    [TestCase(ushort.MaxValue, ushort.MaxValue)]
    public void GetHiLoWords_WhenGivenValidWParam_ReturnsExpectedValues(int expectedHiWord, int expectedLoWord)
    {
        var wParam = TestUtils.CreateWParam(expectedHiWord, expectedLoWord);
        var (actualLoWord, actualHiWord) = WinApiUtils.GetHiLoWords((uint)wParam);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actualLoWord, Is.EqualTo(expectedLoWord));
            Assert.That(actualHiWord, Is.EqualTo(expectedHiWord));
        }
    }

    [TestCase(0, 0, true)]
    [TestCase(5, 5, true)]
    [TestCase(10, 10, true)]
    [TestCase(-1, -1, false)]
    [TestCase(11, 11, false)]
    [TestCase(0, -1, false)]
    [TestCase(-1, 0, false)]
    [TestCase(10, 11, false)]
    [TestCase(11, 10, false)]
    public void IsPointInRect_WhenGivenPointAndRect_ReturnsExpectedResult(int x, int y, bool expectedResult)
    {
        var windowRect = new Common.Models.WindowRect(0, 0, 10, 10);
        var actualResult = WinApiUtils.IsPointInRect(windowRect, x, y);
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }
}
