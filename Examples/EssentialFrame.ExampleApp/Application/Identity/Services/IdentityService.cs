using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IIdentityContext _identityContext;

    public IdentityService()
    {
        _identityContext = new IdentityContext();
    }

    public IIdentityContext GetCurrent()
    {
        return _identityContext;
    }
}