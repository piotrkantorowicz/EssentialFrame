using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Votes;

public record Vote : Enumeration<Vote>
{
    public static readonly Vote None = new(0, "None");
    public static readonly Vote UpVote = new(1, "UpVote");
    public static readonly Vote DownVote = new(2, "DownVote");

    private Vote(int value, string displayName) : base(value, displayName)
    {
    }
}