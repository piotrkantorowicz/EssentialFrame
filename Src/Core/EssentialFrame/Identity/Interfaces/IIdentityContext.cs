namespace EssentialFrame.Identity.Interfaces;

public interface IIdentityContext
{
    ITenant Tenant { get; }

    IUser User { get; }

    ICorrelation Correlation { get; }

    IService Service { get; }
}