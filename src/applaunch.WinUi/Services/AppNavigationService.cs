using applaunch.WinUi.Abstractions;

namespace applaunch.WinUi.Services;

public class AppNavigationService : IAppNavigationService
{
    public int GetNextIndex(int currentIndex, int totalItems, bool moveDown)
    {
        if (totalItems == 0)
            return -1;

        if (moveDown)
        {
            return currentIndex < totalItems - 1 ? currentIndex + 1 : 0;
        }
        else
        {
            return currentIndex > 0 ? currentIndex - 1 : totalItems - 1;
        }
    }
}
