namespace applaunch.WinUi.Models;

public class AppItem
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;

    public AppItem() { }

    public AppItem(string name, string path)
    {
        Name = name;
        Path = path;
    }
}
