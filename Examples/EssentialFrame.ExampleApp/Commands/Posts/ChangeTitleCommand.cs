using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Commands.Posts;

public class ChangeTitleCommand : Command
{
    public ChangeTitleCommand(IIdentityContext identityContext, string title, bool uppercase) : base(identityContext)
    {
        Title = title;
        Uppercase = uppercase;
    }

    public string Title { get; }

    public bool Uppercase { get; }
}