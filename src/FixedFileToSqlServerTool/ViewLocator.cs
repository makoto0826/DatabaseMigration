using HanumanInstitute.MvvmDialogs.Wpf;

namespace FixedFileToSqlServerTool;

public class ViewLocator : ViewLocatorBase
{
    /// <inheritdoc />
    protected override string GetViewName(object vm) =>
        vm.GetType().FullName!.Replace("ViewModels", "Views").Replace("ViewModel", "");
}
