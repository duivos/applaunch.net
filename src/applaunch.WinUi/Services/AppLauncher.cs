using System;
using System.Diagnostics;
using applaunch.WinUi.Abstractions;
using applaunch.WinUi.Models;

namespace applaunch.WinUi.Services;

public class AppLauncher : IAppLauncher
{
    public void Launch(AppItem app)
    {
        try
        {
            ProcessStartInfo startInfo = new() { FileName = app.Path, UseShellExecute = true };

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to launch app: {ex.Message}");
        }
    }
}
