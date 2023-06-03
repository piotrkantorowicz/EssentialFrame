using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Commands;

public class ChangeTitleCommand : Command
{
    public ChangeTitleCommand(IIdentity identity, string title, bool uppercase) : base(identity)
    {
        Title = title;
        Uppercase = uppercase;
    }

    public string Title { get; }

    public bool Uppercase { get; }
}