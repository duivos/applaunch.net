using System;
using System.Collections.Generic;

namespace applaunch.WinUi.Config;

public class AppConfig
{
    public static readonly string[] DefaultProgramPaths =
    [
        Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
        Environment.GetFolderPath(Environment.SpecialFolder.Programs),
    ];

    public static readonly string[] DefaultExcludeKeywords = ["uninstall", "help"];

    // TODO: Add support for custom paths
    public List<string> ProgramPaths { get; set; } = new(DefaultProgramPaths);
    public List<string> ExcludeKeywords { get; set; } = new(DefaultExcludeKeywords);

    public WindowSettings WindowSettings { get; set; } = new();
}

public class WindowSettings
{
    public int Width { get; set; } = 640;

    public int Height { get; set; } = 320;

    public bool Centered { get; set; } = true;

    public bool AlwaysOnTop { get; set; } = true;
}
