using System.Text;
using LoadBalancer.Models;

namespace LoadBalancer.Services;

public class Balancer(List<IWorker> workers, Queue<TaskItem> taskQueue) : IBalancer
{
    private readonly List<IWorker> _workers = workers;
    private readonly Queue<TaskItem> _taskQueue = taskQueue;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(100);

    public async Task DistributeTasksAsync(CancellationToken cancellationToken)
    {
        var runnedTasks = new List<Task>();

        while (!cancellationToken.IsCancellationRequested && _taskQueue.Count != 0)
        {
            var taskItem = _taskQueue.Peek();
            var availableWorker = _workers
                .Where(w => w.CanTakeTask(taskItem))
                .MinBy(w => w.GetCurrentLoad());

            if (availableWorker is not null)
            {
                runnedTasks.Add(availableWorker.ExecuteTaskAsync(taskItem, cancellationToken));
                _taskQueue.Dequeue();
            }

            await Task.Delay(_pollingInterval, cancellationToken);
        }

        await Task.WhenAll(runnedTasks);
    }

    public string GetStatus()
    {
        StringBuilder status = new();

        foreach (var worker in _workers)
        {
            status.Append($"{worker.GetStatus()}\n");
        }
        status.Append($"Задач в очереди: {_taskQueue.Count}\n");

        return status.ToString();
    }
}