using LoadBalancer.Models;

namespace LoadBalancer.Services.interfaces;

public interface IWorker: IStatusProvider
{
    int GetCurrentLoad();
    bool CanTakeTask(TaskItem task);
    Task ExecuteTaskAsync(TaskItem task, CancellationToken cancellationToken);
}