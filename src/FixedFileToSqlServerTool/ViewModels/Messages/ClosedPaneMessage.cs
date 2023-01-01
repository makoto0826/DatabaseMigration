using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FixedFileToSqlServerTool.ViewModels.Messages;

public class ClosedPaneMessage : ValueChangedMessage<IPaneViewModel>
{
    public ClosedPaneMessage(IPaneViewModel value) : base(value) { }
}
