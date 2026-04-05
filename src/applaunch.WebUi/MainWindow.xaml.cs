using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using applaunch.WebUi.Config;
using applaunch.WebUi.Models;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace applaunch.WebUi;

public sealed partial class MainWindow : Window
{
    private readonly List<AppItem> _allApps = [];

    public ObservableCollection<AppItem> _visibleApps = [];

    public MainWindow()
    {
        InitializeComponent();
        SystemBackdrop = new MicaBackdrop()
        {
            Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt,
        };

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(null);

        ResizeWindow(this.AppWindow);
        RemoveBorder(this.AppWindow);
        CenterWindow(this.AppWindow);

        Task.Run(ScanWin32Apps);
    }

    private static void ResizeWindow(AppWindow appWindow)
    {
        appWindow.MoveAndResize(
            new Windows.Graphics.RectInt32
            {
                X = 0,
                Y = 0,
                Width = 640,
                Height = 320,
            }
        );
    }

    private static void RemoveBorder(AppWindow appWindow)
    {
        var presenter = appWindow.Presenter as OverlappedPresenter;
        presenter?.SetBorderAndTitleBar(false, false);
    }

    private static void CenterWindow(AppWindow appWindow)
    {
        var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest);
        var centralX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
        var centralY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

        appWindow.Move(new Windows.Graphics.PointInt32 { X = centralX, Y = centralY });
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = ((TextBox)sender).Text.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
        {
            _visibleApps.Clear();
            return;
        }

        var matches = _allApps
            .Where(app => app.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))
            .ToList();

        _visibleApps.Clear();
        foreach (var match in matches)
        {
            _visibleApps.Add(match);
        }

        Debug.WriteLine($"Found {matches.Count} matches for '{query}'");
    }

    private void ScanWin32Apps()
    {
        foreach (var folder in AppConfig.ProgramPaths)
        {
            if (!Directory.Exists(folder))
                continue;

            var shortcuts = Directory.GetFiles(folder, "*.lnk", SearchOption.AllDirectories);

            foreach (var path in shortcuts)
            {
                string name = Path.GetFileNameWithoutExtension(path);

                if (
                    AppConfig.ExcludeKeywords.Any(keyword =>
                        name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)
                    )
                )
                {
                    continue;
                }

                _allApps.Add(new AppItem { Name = name, Path = path });
            }
        }

        Debug.WriteLine($"Finished scanning. Loaded {_allApps.Count} local apps.");
    }

    private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var selectedApp = (ResultList.SelectedItem as AppItem) ?? _visibleApps.FirstOrDefault();
            if (selectedApp != null)
                LaunchApp(selectedApp);
            return;
        }

        int itemCount = ResultList.Items.Count;
        if (itemCount == 0)
            return;

        switch (e.Key)
        {
            case Windows.System.VirtualKey.Tab:
            case Windows.System.VirtualKey.Down:
                ResultList.SelectedIndex =
                    ResultList.SelectedIndex < itemCount - 1 ? ResultList.SelectedIndex + 1 : 0;
                break;

            case Windows.System.VirtualKey.Up:
                ResultList.SelectedIndex =
                    ResultList.SelectedIndex > 0 ? ResultList.SelectedIndex - 1 : itemCount - 1;
                break;

            default:
                return;
        }

        ResultList.ScrollIntoView(ResultList.SelectedItem);
        e.Handled = true;
    }

    private void LaunchApp(AppItem app)
    {
        try
        {
            ProcessStartInfo startInfo = new() { FileName = app.Path, UseShellExecute = true };

            Process.Start(startInfo);

            this.AppWindow.Hide();

            SearchBox.Text = string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to launch app: {ex.Message}");
        }
    }
}
