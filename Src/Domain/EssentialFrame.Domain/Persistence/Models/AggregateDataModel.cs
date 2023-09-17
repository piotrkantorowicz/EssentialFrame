namespace EssentialFrame.Domain.Persistence.Models;

public class AggregateDataModel
{
    public Guid AggregateIdentifier { get; set; }

    public Guid? TenantIdentifier { get; set; }

    public object State { get; set; }

    public DateTimeOffset? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }
}