namespace EssentialFrame.Domain.Persistence.Models;

public class AggregateDataModel
{
    public virtual string AggregateIdentifier { get; set; }

    public virtual Guid? TenantIdentifier { get; set; }

    public virtual object State { get; set; }
}