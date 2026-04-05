using applaunch.WinUi.Config;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace applaunch.WinUi.Utils;

public static class WindowUtility
{
    public static void Setup(AppWindow appWindow, WindowSettings windowSettings)
    {
        SetBorderAndTitleBar(appWindow, windowSettings);
        Resize(appWindow, windowSettings);

        if (windowSettings.Centered)
        {
            Center(appWindow);
        }
    }

    private static void SetBorderAndTitleBar(AppWindow appWindow, WindowSettings settings)
    {
        OverlappedPresenter? presenter = appWindow.Presenter as OverlappedPresenter;
        if (presenter != null)
        {
            presenter.SetBorderAndTitleBar(false, false);
            presenter.IsAlwaysOnTop = settings.AlwaysOnTop;
        }
    }

    private static void Resize(AppWindow appWindow, WindowSettings settings)
    {
        appWindow.MoveAndResize(
            new RectInt32
            {
                X = 0,
                Y = 0,
                Width = settings.Width,
                Height = settings.Height,
            }
        );
    }

    private static void Center(AppWindow appWindow)
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(
            appWindow.Id,
            DisplayAreaFallback.Nearest
        );

        int centralX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
        int centralY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

        appWindow.Move(new PointInt32 { X = centralX, Y = centralY });
    }
}
