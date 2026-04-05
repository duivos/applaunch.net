using System;

namespace applaunch.WinUi.Abstractions;

public interface IHotkeyService : IDisposable
{
    void Register(IntPtr hwnd, Action onHotkeyPressed);
    void Unregister();
}
