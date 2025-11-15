using System.Collections.ObjectModel;
using System.Windows;
using PropertyChanged.SourceGenerator;

namespace WinSyncScroll.VisualTestUtil;

public sealed partial class MainViewModel
{
    [Notify]
    private int _xScrollCoordinate;

    [Notify]
    private int _yScrollCoordinate;

    [Notify]
    private int _xScrollWpfCoordinate;

    [Notify]
    private int _yScrollWpfCoordinate;

    [Notify]
    private int _xCursorPosition;

    [Notify]
    private int _yCursorPosition;

    private const int MaxScrollEvents = 14;

    public ObservableCollection<ScrollEventViewModel> ScrollEvents { get; set; } = [];

    private static readonly string RandomNumber = new Random().Next(int.MaxValue).ToString();

    private const int ScrollAreaEllipseSize = 50;

    public string Title { get; } = $"!TEST {RandomNumber}";
    public string ScrollCoordinateMessage => $"Last Scroll: X {XScrollCoordinate}, Y {YScrollCoordinate}";
    public string CursorPositionMessage => $"Current Cursor: X {XCursorPosition}, Y {YCursorPosition}";
    public Visibility ScrollAreaVisibility => XScrollCoordinate > 0 || YScrollCoordinate > 0
        ? Visibility.Visible
        : Visibility.Hidden;

    public void UpdateLatestScrollCoordinates(int x, int y, int wpfX, int wpfY)
    {
        XScrollCoordinate = x;
        YScrollCoordinate = y;

        XScrollWpfCoordinate = wpfX - (ScrollAreaEllipseSize / 2);
        YScrollWpfCoordinate = wpfY - (ScrollAreaEllipseSize / 2);
    }

    public void UpdateCurrentCursorPosition(int x, int y)
    {
        XCursorPosition = x;
        YCursorPosition = y;
    }

    public void AddScrollEvent(ScrollEventViewModel scrollEvent)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ScrollEvents.Insert(0, scrollEvent);
            if (ScrollEvents.Count > MaxScrollEvents)
            {
                ScrollEvents.RemoveAt(ScrollEvents.Count - 1);
            }
        });
    }

    public void Initialize()
    {
    }

    public void HandleWindowClosing()
    {
    }
}
