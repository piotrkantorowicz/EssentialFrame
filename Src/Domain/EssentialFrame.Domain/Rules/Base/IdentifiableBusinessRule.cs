﻿using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class IdentifiableBusinessRule<TDomainObjectIdentifier> : BusinessRule
    where TDomainObjectIdentifier : TypedGuidIdentifier
{
    protected IdentifiableBusinessRule(TDomainObjectIdentifier domainObjectIdentifier, Type domainObjectType,
        string businessRuleType) : base(domainObjectType, businessRuleType)
    {
        DomainObjectIdentifier = domainObjectIdentifier;

        Parameters.Add(nameof(DomainObjectIdentifier), domainObjectIdentifier);
    }

    protected TDomainObjectIdentifier DomainObjectIdentifier { get; }
}