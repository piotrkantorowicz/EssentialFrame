namespace EssentialFrame.Core.Identity;

public interface IIdentity
{
    ITenant Tenant { get; }

    IUser User { get; }

    IService Service { get; }
}


