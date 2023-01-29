namespace EssentialFrame.Identity;

public interface IIdentityService
{
    IIdentity GetCurrent();
}