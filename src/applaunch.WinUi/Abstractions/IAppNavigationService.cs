namespace applaunch.WinUi.Abstractions;

public interface IAppNavigationService
{
    int GetNextIndex(int currentIndex, int totalItems, bool moveDown);
}
