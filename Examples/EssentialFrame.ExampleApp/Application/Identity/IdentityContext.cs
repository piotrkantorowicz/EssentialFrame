using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Identity;

public sealed class IdentityContext : IIdentityContext
{
    public IdentityContext()
    {
        Service = new Service();
        User = new User();
        Tenant = new Tenant();
        Correlation = new Correlation();
    }

    public ITenant Tenant { get; }

    public IUser User { get; }

    public ICorrelation Correlation { get; }

    public IService Service { get; }
}