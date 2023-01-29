namespace EssentialFrame.Identity;

public interface IIdentity
{
    ITenant Tenant { get; }

    IUser User { get; }

    IService Service { get; }
}