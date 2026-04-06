using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Config;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Services;

public class AppScanner : IScanner<AppItem>
{
    private readonly List<AppItem> _allApps = [];
    private readonly HashSet<string> _addedAppNames = [];
    private readonly AppConfig _config;

    public IReadOnlyList<AppItem> AllObjects => _allApps.AsReadOnly();

    public AppScanner(AppConfig config)
    {
        _config = config;
    }

    public void Scan()
    {
        _addedAppNames.Clear();

        foreach (string folder in _config.ProgramPaths)
        {
            if (!Directory.Exists(folder))
                continue;

            string[] shortcuts = Directory.GetFiles(folder, "*.lnk", SearchOption.AllDirectories);

            foreach (string path in shortcuts)
            {
                string name = Path.GetFileNameWithoutExtension(path);

                if (ShouldExclude(name))
                    continue;

                _addedAppNames.Add(name);
                _allApps.Add(new AppItem { Name = name, Path = path });
            }
        }

        Debug.WriteLine($"Finished scanning. Loaded {_allApps.Count} local apps.");
    }

    private bool ShouldExclude(string name)
    {
        return IsExcludedByKeyword(name) || IsDuplicate(name);
    }

    private bool IsExcludedByKeyword(string name)
    {
        return _config.ExcludeKeywords.Any(keyword =>
            name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
        );
    }

    private bool IsDuplicate(string name)
    {
        return _addedAppNames.Contains(name);
    }
}
