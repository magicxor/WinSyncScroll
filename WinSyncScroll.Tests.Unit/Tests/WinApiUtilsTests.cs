using WinSyncScroll.Common.Utils;
using WinSyncScroll.Tests.Unit.Utils;

namespace WinSyncScroll.Tests.Unit.Tests;

public class WinApiUtilsTests
{
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(31001, 5)]
    [TestCase(6, 31002)]
    [TestCase(short.MaxValue, 0)]
    [TestCase(0, short.MaxValue)]
    [TestCase(short.MaxValue, short.MaxValue)]
    public void GetHiLoWords_WhenGivenValidLParam_ReturnsExpectedValues(short expectedHiWord, short expectedLoWord)
    {
        var lParamInt = TestUtils.CreateLParam(expectedHiWord, expectedLoWord);
        var (actualLoWord, actualHiWord) = WinApiUtils.GetHiLoWords(lParamInt);

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
