using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Config;
using applaunch.WinUi.Models;
using applaunch.WinUi.Utils;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace applaunch.WinUi;

public sealed partial class MainWindow : Window
{
    public ObservableCollection<AppItem> VisibleApps { get; }

    private readonly IAppScanner _appScanner;
    private readonly IAppLauncher _appLauncher;
    private readonly IHotkeyService _hotkeyManager;
    private readonly IAppSearchService _searchService;
    private readonly IAppNavigationService _navigationService;
    private readonly AppConfig _config;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _appScanner =
            serviceProvider.GetService(typeof(IAppScanner)) as IAppScanner
            ?? throw new InvalidOperationException("IAppScanner not found");
        _appLauncher =
            serviceProvider.GetService(typeof(IAppLauncher)) as IAppLauncher
            ?? throw new InvalidOperationException("IAppLauncher not found");
        _hotkeyManager =
            serviceProvider.GetService(typeof(IHotkeyService)) as IHotkeyService
            ?? throw new InvalidOperationException("IHotkeyManager not found");
        _searchService =
            serviceProvider.GetService(typeof(IAppSearchService)) as IAppSearchService
            ?? throw new InvalidOperationException("IAppSearchService not found");
        _navigationService =
            serviceProvider.GetService(typeof(IAppNavigationService)) as IAppNavigationService
            ?? throw new InvalidOperationException("IAppNavigationService not found");
        _config =
            serviceProvider.GetService(typeof(AppConfig)) as AppConfig
            ?? throw new InvalidOperationException("AppConfig not found");

        VisibleApps =
            serviceProvider.GetService(typeof(ObservableCollection<AppItem>))
                as ObservableCollection<AppItem>
            ?? throw new InvalidOperationException("ObservableCollection<AppItem> not found");

        SetupUI();
        RegisterHotkey();
        ScanAppsAsync();
        Closed += (s, e) => _hotkeyManager.Dispose();
    }

    private void SetupUI()
    {
        SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };

        ExtendsContentIntoTitleBar = true;
        WindowUtility.Setup(this.AppWindow, _config.WindowSettings);
        SetTitleBar(null);
    }

    private void RegisterHotkey()
    {
        try
        {
            nint hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _hotkeyManager.Register(hwnd, ShowWindow);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to setup hotkey: {ex.Message}");
        }
    }

    private void ScanAppsAsync()
    {
        Task.Run(_appScanner.Scan);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string query = ((TextBox)sender).Text.Trim().ToLower();
        _searchService.UpdateSearch(query);
        if (VisibleApps.Count > 0)
        {
            ResultList.SelectedIndex = 0;
        }
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

        bool moveDown =
            e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.Tab;
        int newIndex = _navigationService.GetNextIndex(
            ResultList.SelectedIndex,
            itemCount,
            moveDown
        );

        if (newIndex >= 0)
        {
            ResultList.SelectedIndex = newIndex;
            ResultList.ScrollIntoView(ResultList.SelectedItem);
            e.Handled = true;
        }
    }

    private void LaunchAppAndHide(AppItem app)
    {
        _appLauncher.Launch(app);
        VisibleApps.Clear();
        SearchBox.Text = string.Empty;
        this.AppWindow.Hide();
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
