using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts.Rules;

public class PostCommentTextCanHaveAtLeast3AndAtMost1500CharactersRule : BusinessRule
{
    private readonly string _postCommentTextValue;

    public PostCommentTextCanHaveAtLeast3AndAtMost1500CharactersRule(Type domainObjectType, string postCommentTextValue)
        : base(domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _postCommentTextValue = postCommentTextValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value have to be between 3 and 1500 characters";

    public override bool IsBroken()
    {
        return _postCommentTextValue?.Length is < 3 or > 1500;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("PostCommentTextValue", _postCommentTextValue);
    }
}