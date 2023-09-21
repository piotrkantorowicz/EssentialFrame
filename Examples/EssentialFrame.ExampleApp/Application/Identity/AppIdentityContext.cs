using System;
using EssentialFrame.Identity;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Identity;

public sealed class AppIdentityContext : IdentityContext
{
    public AppIdentityContext()
    {
        Random rnd = new();

        Service = new Service(new Guid("6F2D5BFE-622D-460D-BC60-D1290098498A"), "ExampleApp Service", "1.0.0");
        User = new User(new Guid("4D0AB6DC-84CA-48A9-8FEB-3BF93313D01D"), "essentialframe@email.com", "EssentialFrame",
            true, rnd.Next(1000000000));
        Tenant = new Tenant(new Guid("73EF7E95-6718-440B-8EBC-2B1396FAAAAF"), "EF", "EssentialFrame",
            rnd.Next(1000000000));
        Correlation = new Correlation(new Guid("9534092F-8FC5-4488-9DFD-AEC47E19387C"));
    }

    public AppIdentityContext(ITenant tenant, IUser user, ICorrelation correlation, IService service)
    {
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        User = user ?? throw new ArgumentNullException(nameof(user));
        Correlation = correlation ?? throw new ArgumentNullException(nameof(correlation));
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }
}