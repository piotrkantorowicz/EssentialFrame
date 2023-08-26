﻿using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public sealed class PostCanBeOnlyUpdatedByAuthorRule : BusinessRule
{
    private readonly Guid _authorIdentifier;
    private readonly Guid _updaterIdentifier;

    public PostCanBeOnlyUpdatedByAuthorRule(Guid domainObjectIdentifier, Type businessObjectType, Guid authorIdentifier,
        Guid updaterIdentifier) : base(domainObjectIdentifier, businessObjectType,
        BusinessRuleTypes.AggregateBusinessRule)
    {
        _authorIdentifier = authorIdentifier;
        _updaterIdentifier = updaterIdentifier;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be updated only by author. Creator identifier ({_authorIdentifier}), updater identifier ({_updaterIdentifier})";

    public override bool IsBroken()
    {
        return _authorIdentifier != _updaterIdentifier;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("CreatorIdentifier", _authorIdentifier);
        Parameters.TryAdd("UpdaterIdentifier", _updaterIdentifier);
    }
}