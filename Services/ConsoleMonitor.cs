using LoadBalancer.Services.interfaces;

namespace LoadBalancer.Services;

public class ConsoleMonitor(IStatusProvider statusProvider): IMonitor
{
    private readonly IStatusProvider _statusProvider = statusProvider;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(2000);

    public async Task MonitorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Clear();
            Console.WriteLine(_statusProvider.GetStatus());
            await Task.Delay(_pollingInterval, cancellationToken);
        }
    }
}