namespace EssentialFrame.Identity.Interfaces;

public interface IIdentityService
{
    IdentityContext GetCurrent();
}