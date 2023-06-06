namespace EssentialFrame.Identity;

public interface IIdentityService
{
    IIdentityContext GetCurrent();
}