using LoadBalancer.Models;

namespace LoadBalancer.Services.interfaces;

public interface IWorker: IStatusProvider
{
    int GetCurrentLoad();
    bool CanExecute(TaskItem task);
    Task ExecuteAsync(TaskItem task, CancellationToken cancellationToken);
}