namespace LoadBalancer.Services;

public interface IBalancer
{
    Task DistributeTasksAsync(CancellationToken cancellationToken);
    string GetStatus();
}