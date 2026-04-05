using System.Collections.Generic;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Abstractions;

public interface IAppSearchEngine
{
    List<AppItem> Search(IEnumerable<AppItem> apps, string query);
}
