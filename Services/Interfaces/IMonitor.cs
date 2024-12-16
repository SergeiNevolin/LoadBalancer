namespace LoadBalancer.Services.interfaces;

public interface IMonitor
{
    Task MonitorAsync(CancellationToken cancellationToken);
}