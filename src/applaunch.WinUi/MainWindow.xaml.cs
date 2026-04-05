using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Models;
using applaunch.WinUi.Services;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace applaunch.WinUi;

public sealed partial class MainWindow : Window
{
    public ObservableCollection<AppItem> VisibleApps { get; } = [];

    private readonly IAppScanner _appScanner;
    private readonly IAppSearchEngine _searchEngine;
    private readonly IAppLauncher _appLauncher;
    private readonly IHotkeyManager _hotkeyManager;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _appScanner =
            serviceProvider.GetService(typeof(IAppScanner)) as IAppScanner
            ?? throw new InvalidOperationException("IAppScanner not found");
        _searchEngine =
            serviceProvider.GetService(typeof(IAppSearchEngine)) as IAppSearchEngine
            ?? throw new InvalidOperationException("IAppSearchEngine not found");
        _appLauncher =
            serviceProvider.GetService(typeof(IAppLauncher)) as IAppLauncher
            ?? throw new InvalidOperationException("IAppLauncher not found");
        _hotkeyManager =
            serviceProvider.GetService(typeof(IHotkeyManager)) as IHotkeyManager
            ?? throw new InvalidOperationException("IHotkeyManager not found");

        SetupUI();
        RegisterHotkey();
        ScanAppsAsync();
        Closed += (s, e) => _hotkeyManager.Dispose();
    }

    private void SetupUI()
    {
        SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };

        ExtendsContentIntoTitleBar = true;
        WindowManager.Setup(this.AppWindow);
        SetTitleBar(null);
    }

    private void RegisterHotkey()
    {
        try
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _hotkeyManager.Register(hwnd, ShowWindow);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to setup hotkey: {ex.Message}");
        }
    }

    private void ScanAppsAsync()
    {
        Task.Run(() => _appScanner.Scan());
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = ((TextBox)sender).Text.Trim().ToLower();
        UpdateVisibleApps(query);
    }

    private void UpdateVisibleApps(string query)
    {
        var matches = _searchEngine.Search(_appScanner.AllApps, query);

        VisibleApps.Clear();
        foreach (var match in matches)
        {
            VisibleApps.Add(match);
        }

        Debug.WriteLine($"Found {matches.Count} matches for '{query}'");
    }

    private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            HandleEnterKey();
            return;
        }

        HandleNavigationKeys(e);
    }

    private void HandleEnterKey()
    {
        AppItem? selectedApp = (ResultList.SelectedItem as AppItem) ?? VisibleApps.FirstOrDefault();
        if (selectedApp != null)
        {
            LaunchAppAndHide(selectedApp);
        }
    }

    private void HandleNavigationKeys(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
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

    private void LaunchAppAndHide(AppItem app)
    {
        _appLauncher.Launch(app);
        this.AppWindow.Hide();
        SearchBox.Text = string.Empty;
    }

    private void ShowWindow()
    {
        DispatcherQueue?.TryEnqueue(() =>
        {
            this.AppWindow.Show();
            SearchBox.Focus(FocusState.Programmatic);
        });
    }
}
