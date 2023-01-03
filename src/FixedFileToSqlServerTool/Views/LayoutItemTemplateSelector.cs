using System.Windows;
using System.Windows.Controls;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Views;

public class LayoutItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate MappingTableTemplate { get; set; }

    public DataTemplate TableTemplate { get; set; }

    public DataTemplate ScriptTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container) =>
        item switch
        {
            MappingTablePaneViewModel => MappingTableTemplate,
            TablePaneViewModel => TableTemplate,
            ScriptPaneViewModel => ScriptTemplate,
            _ => base.SelectTemplate(item, container)
        };
}
