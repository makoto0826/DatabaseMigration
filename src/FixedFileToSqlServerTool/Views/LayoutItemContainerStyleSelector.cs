using System.Windows;
using System.Windows.Controls;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Views;

public class LayoutItemContainerStyleSelector : StyleSelector
{
    public required Style MappingTableStyle { get; set; }

    public required Style TableStyle { get; set; }

    public required Style ScriptStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container) =>
        item switch
        {
            MappingTableContentViewModel => MappingTableStyle,
            ScriptContentViewModel => ScriptStyle,
            TableContentViewModel => TableStyle,
            _ => base.SelectStyle(item, container)
        };
}
