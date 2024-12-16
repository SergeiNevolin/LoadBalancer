using LoadBalancer.Models;

namespace LoadBalancer.Services;

public interface IWorker
{
    int GetCurrentLoad();
    bool CanTakeTask(TaskItem task);
    Task ExecuteTaskAsync(TaskItem task, CancellationToken cancellationToken);
    string GetStatus();
}