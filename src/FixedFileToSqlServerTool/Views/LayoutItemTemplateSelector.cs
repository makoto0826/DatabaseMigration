using System.Windows;
using System.Windows.Controls;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Views;

public class LayoutItemTemplateSelector : DataTemplateSelector
{
    public required DataTemplate MappingTableTemplate { get; set; }

    public required DataTemplate TableTemplate { get; set; }

    public required DataTemplate ScriptTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container) =>
        item switch
        {
            MappingTablePaneViewModel => MappingTableTemplate,
            TableContentPaneViewModel => TableTemplate,
            ScriptContentPaneViewModel => ScriptTemplate,
            _ => base.SelectTemplate(item, container)
        };
}
