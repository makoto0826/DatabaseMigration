using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ClosedMessage : ValueChangedMessage<IContentViewModel>
{
    public ClosedMessage(IContentViewModel value) : base(value) { }
}
