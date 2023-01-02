using System.Collections.ObjectModel;

namespace FixedFileToSqlServerTool.ViewModels;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> self, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            self.Add(item);
        }
    }
}
