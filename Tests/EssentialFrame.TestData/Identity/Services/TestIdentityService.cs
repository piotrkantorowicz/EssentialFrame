using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity.Services;

public class TestIdentityService : IIdentityService
{
    private readonly IIdentity _identity;

    public TestIdentityService()
    {
        _identity = new TestIdentity();
    }

    public IIdentity GetCurrent()
    {
        return _identity;
    }
}