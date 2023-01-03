using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ClosedPaneMessage : ValueChangedMessage<IPaneViewModel>
{
    public ClosedPaneMessage(IPaneViewModel value) : base(value) { }
}
