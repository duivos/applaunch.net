using System.Collections.Generic;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Abstractions;

public interface IAppScanner
{
    IReadOnlyList<AppItem> AllApps { get; }
    void Scan();
}
