using System;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Abstractions;

public interface IHotkeyManager : IDisposable
{
    void Register(IntPtr hwnd, Action onHotkeyPressed);
    void Unregister();
}
