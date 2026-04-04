using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace applaunch.WebUi
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SystemBackdrop = new MicaBackdrop()
            {
                Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt
            };

            ResizeWindow(this.AppWindow);
            RemoveBorder(this.AppWindow);
            CenterWindow(this.AppWindow);
        }

        private static void ResizeWindow(AppWindow appWindow)
        {
            appWindow.MoveAndResize(new Windows.Graphics.RectInt32
            {
                X = 0,
                Y = 0,
                Width = 640,
                Height = 320
            });
        }

        private static void RemoveBorder(AppWindow appWindow)
        {
            var presenter = appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.SetBorderAndTitleBar(false, false);
            }
        }

        private static void CenterWindow(AppWindow appWindow)
        {
            var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest);
            var centralX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            var centralY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

            appWindow.Move(
                new Windows.Graphics.PointInt32
                {
                    X = centralX,
                    Y = centralY
                }
            );
        }
    }
}
