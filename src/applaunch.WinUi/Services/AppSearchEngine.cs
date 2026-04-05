using System;
using System.Collections.Generic;
using System.Linq;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Services;

public class AppSearchEngine : ISearchEngine<AppItem>
{
    public List<AppItem> Search(IEnumerable<AppItem> apps, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        return apps.Where(app =>
                app.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)
            )
            .OrderBy(app => app.Name.Length)
            .ToList();
    }
}
