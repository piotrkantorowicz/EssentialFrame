using System;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Identity;

public sealed class AppIdentityContext : IdentityContext
{
    public AppIdentityContext()
    {
        Service = new Service(new Guid("6F2D5BFE-622D-460D-BC60-D1290098498A"), "ExampleApp Service", "1.0.0");
        User = new User(new Guid("4D0AB6DC-84CA-48A9-8FEB-3BF93313D01D"), "essentialframe@email.com", "EssentialFrame",
            true, 135791);
        Tenant = new Tenant(new Guid("73EF7E95-6718-440B-8EBC-2B1396FAAAAF"), "EF", "EssentialFrame", 197531);
        Correlation = new Correlation(new Guid("9534092F-8FC5-4488-9DFD-AEC47E19387C"));
    }
}