using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace applaunch.WinUi.Services;

public static class WindowManager
{
    private const int WindowWidth = 640;
    private const int WindowHeight = 320;

    public static void Setup(AppWindow appWindow)
    {
        SetBorderAndTitleBar(appWindow);
        Resize(appWindow);
        Center(appWindow);
    }

    private static void SetBorderAndTitleBar(AppWindow appWindow)
    {
        OverlappedPresenter? presenter = appWindow.Presenter as OverlappedPresenter;
        if (presenter != null)
        {
            presenter.SetBorderAndTitleBar(false, false);
            presenter.IsAlwaysOnTop = true;
        }
    }

    private static void Resize(AppWindow appWindow)
    {
        appWindow.MoveAndResize(
            new RectInt32
            {
                X = 0,
                Y = 0,
                Width = WindowWidth,
                Height = WindowHeight,
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
