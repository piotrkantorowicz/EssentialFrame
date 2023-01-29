using EssentialFrame.Identity;

namespace EssentialFrame.Domain.UnitTests.TestData;

public sealed class TestIdentity : IIdentity
{
    public TestIdentity()
    {
        Service = new TestService();
        User = new TestUser();
        Tenant = new TestTenant();
    }

    public ITenant Tenant { get; }
    public IUser User { get; }
    public IService Service { get; }
}