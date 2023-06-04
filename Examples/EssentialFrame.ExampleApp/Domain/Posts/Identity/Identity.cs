using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.Identity;

public sealed class Identity : IIdentity
{
    public Identity()
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