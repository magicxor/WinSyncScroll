using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using WinSyncScroll.Models;
using WinSyncScroll.Services;

namespace WinSyncScroll.ViewModels;

public class MainViewModel
{
    private readonly WinApiManager _winApiManager;

    public ObservableCollection<WindowInfo> Windows { get; } = [];
    public ICollectionView WindowsOrdered { get; }

    public RelayCommand RefreshWindowsCommand { get; }

    public MainViewModel(WinApiManager winApiManager)
    {
        _winApiManager = winApiManager;
        RefreshWindowsCommand = new RelayCommand(RefreshWindows);

        WindowsOrdered = CollectionViewSource.GetDefaultView(Windows);
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ProcessId), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.ClassName), ListSortDirection.Ascending));
        WindowsOrdered.SortDescriptions.Add(new SortDescription(nameof(WindowInfo.WindowHandle), ListSortDirection.Ascending));
    }

    private void RefreshWindows()
    {
        var newWindows = _winApiManager.ListWindows();

        var toBeRemoved = Windows
            .Where(w => newWindows.All(nw => nw.WindowHandle != w.WindowHandle));

        var toBeUpdated = Windows
            .Join(newWindows,
                w => w.WindowHandle,
                nw => nw.WindowHandle,
                (w, nw) => new
                {
                    OldWindow = w,
                    NewWindow = nw,
                });

        var toBeAdded = newWindows
            .Where(nw => Windows.All(w => nw.WindowHandle != w.WindowHandle));

        foreach (var window in toBeRemoved)
        {
            Windows.Remove(window);
        }

        foreach (var windowToBeUpdated in toBeUpdated)
        {
            var i = Windows.IndexOf(windowToBeUpdated.OldWindow);
            if (i != -1)
            {
                Windows[i] = windowToBeUpdated.NewWindow;
            }
        }

        foreach (var window in toBeAdded)
        {
            Windows.Add(window);
        }
    }
}
