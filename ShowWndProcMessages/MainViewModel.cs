using System.Windows;
using PropertyChanged.SourceGenerator;

namespace ShowWndProcMessages;

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

    private static readonly string RandomNumber = new Random().Next(int.MaxValue).ToString();

    private const int ScrollAreaEllipseSize = 50;

    public string Title { get; } = $"!TEST {RandomNumber}";
    public string ScrollCoordinateMessage => $"Scroll X: {XScrollCoordinate}, Y: {YScrollCoordinate}";
    public string CursorPositionMessage => $"Cursor X: {XCursorPosition}, Y: {YCursorPosition}";
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

    public void Initialize()
    {
    }

    public void HandleWindowClosing()
    {
    }
}
