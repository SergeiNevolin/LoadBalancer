namespace LoadBalancer.Services.interfaces;

public interface IBalancer: IStatusProvider
{
    Task DistributeAsync(CancellationToken cancellationToken);
}