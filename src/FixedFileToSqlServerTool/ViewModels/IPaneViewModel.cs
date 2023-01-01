using CommunityToolkit.Mvvm.ComponentModel;

namespace FixedFileToSqlServerTool.ViewModels;

public interface IPaneViewModel
{
    string Title { get; }

    bool IsSelected { get; set; }
}
