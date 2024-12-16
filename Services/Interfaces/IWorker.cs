using LoadBalancer.Models;

namespace LoadBalancer.Services.interfaces;

public interface IWorker: IStatusProvider
{
    int GetCurrentLoad();
    bool CanTake(TaskItem task);
    Task ExecuteAsync(TaskItem task, CancellationToken cancellationToken);
}