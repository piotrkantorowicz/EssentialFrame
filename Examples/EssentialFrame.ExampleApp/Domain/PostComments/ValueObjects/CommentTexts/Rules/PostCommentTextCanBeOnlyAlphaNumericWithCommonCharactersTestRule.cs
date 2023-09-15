using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts.Rules;

public class PostCommentTextCanBeOnlyAlphaNumericWithCommonCharactersTestRule : BusinessRule
{
    private readonly string _postCommentTextValue;

    public PostCommentTextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(Type domainObjectType,
        string postCommentTextValue) : base(domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _postCommentTextValue = postCommentTextValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value can be only alpha numeric with common special characters";

    public override bool IsBroken()
    {
        return _postCommentTextValue?.IsAlphaNumericWithSpacesAndCommonCharacters() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("PostCommentTextValue", _postCommentTextValue);
    }
}