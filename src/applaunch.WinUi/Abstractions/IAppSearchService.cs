using applaunch.WinUi.Models;

namespace applaunch.WinUi.Abstractions;

public interface IAppSearchService
{
    void UpdateSearch(string query);
    AppItem? GetSelectedApp(int selectedIndex);
    void Clear();
}
