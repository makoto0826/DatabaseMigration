namespace FixedFileToSqlServerTool.ViewModels;

public interface IPaneViewModel
{
    public string Name { get; }

    public string ContentId { get; }

    public bool IsActive { get; set; }
}
