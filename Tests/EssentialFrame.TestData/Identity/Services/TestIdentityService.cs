using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity.Services;

public class TestIdentityService : IIdentityService
{
    public IIdentity GetCurrent()
    {
        return new TestIdentity();
    }
}