using System.Collections.Generic;

namespace applaunch.WinUi.Abstractions;

public interface IScanner<T>
    where T : class
{
    IReadOnlyList<T> AllObjects { get; }
    void Scan();
}
