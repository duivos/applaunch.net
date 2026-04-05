using applaunch.WinUi.Models;

namespace applaunch.WinUi.Abstractions;

public interface IAppLauncher
{
    void Launch(AppItem app);
}
