namespace EssentialFrame.Domain.Core;

public interface IEvent
{
    Guid AggregateIdentifier { get; set; }

    int AggregateVersion { get; set; }

    Guid IdentityTenant { get; set; }

    Guid IdentityUser { get; set; }

    DateTimeOffset EventTime { get; set; }
}

