using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity;

public sealed class TestIdentity : IIdentity
{
    public TestIdentity()
    {
        Service = new TestService();
        User = new TestUser();
        Tenant = new TestTenant();
        Correlation = new TestCorrelation();
    }

    public ITenant Tenant { get; }

    public IUser User { get; }

    public ICorrelation Correlation { get; }

    public IService Service { get; }
}