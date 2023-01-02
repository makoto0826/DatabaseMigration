namespace FixedFileToSqlServerTool.ViewModels;

public interface IPaneViewModel
{
    string Title { get; }

    bool IsSelected { get; set; }
}
