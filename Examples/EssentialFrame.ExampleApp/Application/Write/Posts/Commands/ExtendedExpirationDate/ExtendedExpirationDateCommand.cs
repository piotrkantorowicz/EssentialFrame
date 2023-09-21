using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ExtendedExpirationDate;

public class ExtendedExpirationDateCommand : Command
{
    public ExtendedExpirationDateCommand(Guid aggregateIdentifier, IdentityContext identityContext,
        DateTimeOffset expirationDate) : base(aggregateIdentifier, identityContext)
    {
        ExpirationDate = expirationDate;
    }

    public DateTimeOffset ExpirationDate { get; }
}