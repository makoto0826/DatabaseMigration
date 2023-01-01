using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
