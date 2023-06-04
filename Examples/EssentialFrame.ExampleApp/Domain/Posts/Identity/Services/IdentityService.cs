using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IIdentity _identity;

    public IdentityService()
    {
        _identity = new Identity();
    }

    public IIdentity GetCurrent()
    {
        return _identity;
    }
}