using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using applaunch.WinUi.Abstractions;

namespace applaunch.WinUi.Services;

public class HotkeyManager : IHotkeyManager
{
    private const int HOTKEY_ID = 9000;
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_WIN = 0x0008;
    private const uint VK_V = 0x56;
    private const int WM_HOTKEY = 0x0312;
    private const int GWLP_WNDPROC = -4;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(
        IntPtr lpPrevWndFunc,
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        IntPtr lParam
    );

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private WndProcDelegate? _wndProc;
    private IntPtr _oldWndProc = IntPtr.Zero;
    private IntPtr _hwnd = IntPtr.Zero;
    private Action? _onHotkeyPressed;

    public void Register(IntPtr hwnd, Action onHotkeyPressed)
    {
        _hwnd = hwnd;
        _onHotkeyPressed = onHotkeyPressed;

        try
        {
            _wndProc = new WndProcDelegate(WndProc);
            _oldWndProc = GetWindowLongPtr(_hwnd, GWLP_WNDPROC);
            SetWindowLongPtr(_hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(_wndProc));

            bool result = RegisterHotKey(_hwnd, HOTKEY_ID, MOD_WIN | MOD_ALT, VK_V);
            Debug.WriteLine(
                result
                    ? "Global hotkey (WIN+ALT+V) registered successfully"
                    : "Failed to register hotkey"
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to register global hotkey: {ex.Message}");
        }
    }

    public void Unregister()
    {
        try
        {
            if (_hwnd != IntPtr.Zero)
            {
                UnregisterHotKey(_hwnd, HOTKEY_ID);
                Debug.WriteLine("Global hotkey unregistered");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to unregister hotkey: {ex.Message}");
        }
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
        {
            _onHotkeyPressed?.Invoke();
        }

        return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
    }

    public void Dispose()
    {
        Unregister();
        GC.SuppressFinalize(this);
    }
}
