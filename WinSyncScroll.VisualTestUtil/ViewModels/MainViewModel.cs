using System.Collections.ObjectModel;
using System.Windows;
using JetBrains.Annotations;
using PropertyChanged.SourceGenerator;
using WinSyncScroll.VisualTestUtil.Models;

namespace WinSyncScroll.VisualTestUtil.ViewModels;

public sealed partial class MainViewModel
{
    [Notify]
    private int _lastScrollAbsoluteX;

    [Notify]
    private int _lastScrollAbsoluteY;

    [Notify]
    private int _lastScrollWpfX;

    [Notify]
    private int _lastScrollWpfY;

    [Notify]
    private int _lastScrollWpfEllipseX;

    [Notify]
    private int _lastScrollWpfEllipseY;

    [Notify]
    private int _xCursorPosition;

    [Notify]
    private int _yCursorPosition;

    private const int MaxScrollEvents = 14;

    public ObservableCollection<ScrollEventViewModel> ScrollEvents { get; set; } = [];

    private static readonly string RandomNumber = new Random().Next(int.MaxValue).ToString();

    private const int ScrollAreaEllipseSize = 50;

    [UsedImplicitly]
    public string Title { get; } = $"!TEST {RandomNumber}";

    [UsedImplicitly]
    public string LastScrollAbsoluteMessage => $"Last Scroll ABS: X {LastScrollAbsoluteX}, Y {LastScrollAbsoluteY}";

    [UsedImplicitly]
    public string LastScrollWpfMessage => $"Last Scroll WPF: X {LastScrollWpfX}, Y {LastScrollWpfY}";

    [UsedImplicitly]
    public string CursorPositionMessage => $"Cursor: X {XCursorPosition}, Y {YCursorPosition}";

    [UsedImplicitly]
    public Visibility ScrollAreaVisibility => LastScrollAbsoluteX > 0 || LastScrollAbsoluteY > 0
        ? Visibility.Visible
        : Visibility.Hidden;

    public void UpdateLatestScrollCoordinates(int eventAbsoluteX, int eventAbsoluteY, int wpfX, int wpfY)
    {
        LastScrollAbsoluteX = eventAbsoluteX;
        LastScrollAbsoluteY = eventAbsoluteY;

        LastScrollWpfX = wpfX;
        LastScrollWpfY = wpfY;

        LastScrollWpfEllipseX = wpfX - (ScrollAreaEllipseSize / 2);
        LastScrollWpfEllipseY = wpfY - (ScrollAreaEllipseSize / 2);
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
