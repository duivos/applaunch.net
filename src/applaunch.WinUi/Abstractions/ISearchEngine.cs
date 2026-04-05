using System.Collections.Generic;

namespace applaunch.WinUi.Abstractions;

public interface ISearchEngine<T>
    where T : class
{
    List<T> Search(IEnumerable<T> objects, string query);
}
