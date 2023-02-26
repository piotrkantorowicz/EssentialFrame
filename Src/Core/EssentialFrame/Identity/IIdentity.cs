namespace EssentialFrame.Identity;

public interface IIdentity
{
    ITenant Tenant { get; }

    IUser User { get; }

    ICorrelation Correlation { get; }

    IService Service { get; }
}