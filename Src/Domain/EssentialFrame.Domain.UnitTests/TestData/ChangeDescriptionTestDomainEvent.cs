using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.UnitTests.TestData;

public class ChangeDescriptionTestDomainEvent : DomainEventBase
{
    public ChangeDescriptionTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, string newDescription) : base(
        aggregateIdentifier, identity)
    {
        NewDescription = newDescription;
    }

    public string NewDescription { get; }
}