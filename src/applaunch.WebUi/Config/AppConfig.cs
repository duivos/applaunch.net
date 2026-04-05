using System;

namespace applaunch.WebUi.Config;

internal static class AppConfig
{
    public static readonly string[] ProgramPaths =
    [
        Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms),
        Environment.GetFolderPath(Environment.SpecialFolder.Programs),
    ];

    public static string[] ExcludeKeywords = ["uninstall", "readme", "help"];
}
