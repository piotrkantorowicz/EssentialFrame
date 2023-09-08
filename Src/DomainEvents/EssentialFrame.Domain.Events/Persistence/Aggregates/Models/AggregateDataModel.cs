namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Models;

public class AggregateDataModel
{
    public Guid AggregateIdentifier { get; set; }

    public Guid? TenantIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public DateTimeOffset? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }
}