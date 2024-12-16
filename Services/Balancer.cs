using System.Collections.Concurrent;
using System.Text;
using LoadBalancer.Models;
using LoadBalancer.Services.interfaces;

namespace LoadBalancer.Services;

public class Balancer(List<IWorker> workers, ConcurrentQueue<TaskItem> taskQueue) : IBalancer
{
    private readonly List<IWorker> _workers = workers;
    private readonly ConcurrentQueue<TaskItem> _taskQueue = taskQueue;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(100);

    public async Task DistributeAsync(CancellationToken cancellationToken)
    {
        var runnedTasks = new List<Task>();

        while (!cancellationToken.IsCancellationRequested && !_taskQueue.IsEmpty)
        {
            if (_taskQueue.TryPeek(out var taskItem))
            {
                var availableWorker = _workers
                    .Where(w => w.CanTake(taskItem))
                    .MinBy(w => w.GetCurrentLoad());

                if (availableWorker is not null && _taskQueue.TryDequeue(out var _))
                    runnedTasks.Add(availableWorker.ExecuteAsync(taskItem, cancellationToken));
            }

            await Task.Delay(_pollingInterval, cancellationToken);
        }

        await Task.WhenAll(runnedTasks);
    }

    public string GetStatus()
    {
        StringBuilder status = new();

        foreach (var worker in _workers)
            status.Append($"{worker.GetStatus()}\n");

        status.Append($"Задач в очереди: {_taskQueue.Count}\n");

        return status.ToString();
    }
}