using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ClosedPaneMessage : ValueChangedMessage<IContentViewModel>
{
    public ClosedPaneMessage(IContentViewModel value) : base(value) { }
}
