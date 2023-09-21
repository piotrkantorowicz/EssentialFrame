using System;
using EssentialFrame.Identity;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Identity.Services;

public class AppIdentityService : IIdentityService
{
    private readonly AppIdentityContext _appIdentityContext;

    public AppIdentityService()
    {
        _appIdentityContext = new AppIdentityContext();
    }

    public AppIdentityService(AppIdentityContext appIdentityContext)
    {
        _appIdentityContext = appIdentityContext ?? throw new ArgumentNullException(nameof(appIdentityContext));
    }

    public IdentityContext GetCurrent()
    {
        return _appIdentityContext;
    }
}