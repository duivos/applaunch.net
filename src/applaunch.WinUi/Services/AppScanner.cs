using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Config;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Services;

public class AppScanner : IAppScanner
{
    private readonly List<AppItem> _allApps = [];

    public IReadOnlyList<AppItem> AllApps => _allApps.AsReadOnly();

    public void Scan()
    {
        foreach (var folder in AppConfig.ProgramPaths)
        {
            if (!Directory.Exists(folder))
                continue;

            string[] shortcuts = Directory.GetFiles(folder, "*.lnk", SearchOption.AllDirectories);

            foreach (var path in shortcuts)
            {
                var name = Path.GetFileNameWithoutExtension(path);

                if (ShouldExclude(name))
                    continue;

                _allApps.Add(new AppItem { Name = name, Path = path });
            }
        }

        Debug.WriteLine($"Finished scanning. Loaded {_allApps.Count} local apps.");
    }

    private static bool ShouldExclude(string name)
    {
        return AppConfig.ExcludeKeywords.Any(keyword =>
            name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
        );
    }
}
