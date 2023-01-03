using System.Windows;
using System.Windows.Controls;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Views;

public class LayoutItemContainerStyleSelector : StyleSelector
{
    public Style MappingTableStyle { get; set; }

    public Style TableStyle { get; set; }

    public Style ScriptStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container) =>
        item switch
        {
            MappingTablePaneViewModel => MappingTableStyle,
            ScriptPaneViewModel => ScriptStyle,
            TablePaneViewModel => TableStyle,
            _ => base.SelectStyle(item, container)
        };
}
