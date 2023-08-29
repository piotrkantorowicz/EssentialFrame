using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ExtendedExpirationDate;

public class ExtendedExpirationDateCommand : Command
{
    public ExtendedExpirationDateCommand(Guid aggregateIdentifier, IIdentityContext identityContext,
        DateTimeOffset expirationDate) : base(aggregateIdentifier, identityContext)
    {
        ExpirationDate = expirationDate;
    }

    public ExtendedExpirationDateCommand(Guid aggregateIdentifier, Guid commandIdentifier,
        IIdentityContext identityContext, DateTimeOffset expirationDate) : base(aggregateIdentifier, commandIdentifier,
        identityContext)
    {
        ExpirationDate = expirationDate;
    }

    public ExtendedExpirationDateCommand(Guid aggregateIdentifier, Guid commandIdentifier,
        IIdentityContext identityContext, int expectedVersion, DateTimeOffset expirationDate) : base(
        aggregateIdentifier, commandIdentifier, expectedVersion, identityContext)
    {
        ExpirationDate = expirationDate;
    }

    public DateTimeOffset ExpirationDate { get; }
}