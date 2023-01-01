using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ChangedIsSelectedPaneMessage : ValueChangedMessage<IPaneViewModel>
{
    public ChangedIsSelectedPaneMessage(IPaneViewModel value) : base(value) { }
}
