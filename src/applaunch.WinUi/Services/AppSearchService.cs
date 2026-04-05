using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Services;

public class AppSearchService : IAppSearchService
{
    private readonly IAppScanner _appScanner;
    private readonly ISearchEngine<AppItem> _searchEngine;
    private readonly ObservableCollection<AppItem> _visibleApps;

    public AppSearchService(
        IAppScanner appScanner,
        ISearchEngine<AppItem> searchEngine,
        ObservableCollection<AppItem> visibleApps
    )
    {
        _appScanner = appScanner;
        _searchEngine = searchEngine;
        _visibleApps = visibleApps;
    }

    public void UpdateSearch(string query)
    {
        List<AppItem> matches = _searchEngine.Search(_appScanner.AllApps, query);

        _visibleApps.Clear();
        foreach (AppItem match in matches)
        {
            _visibleApps.Add(match);
        }

        Debug.WriteLine($"Found {matches.Count} matches for '{query}'");
    }

    public AppItem? GetSelectedApp(int selectedIndex)
    {
        return selectedIndex >= 0 && selectedIndex < _visibleApps.Count
            ? _visibleApps[selectedIndex]
            : _visibleApps.FirstOrDefault();
    }

    public void Clear()
    {
        _visibleApps.Clear();
    }
}
