using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Identity;

public class IdentityContext : IIdentityContext
{
    protected IdentityContext()
    {
    }

    public virtual ITenant Tenant { get; protected init; }

    public virtual IUser User { get; protected init; }

    public virtual ICorrelation Correlation { get; protected init; }

    public virtual IService Service { get; protected init; }
}