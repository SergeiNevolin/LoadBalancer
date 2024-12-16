using LoadBalancer.Models;
using LoadBalancer.Services.interfaces;

namespace LoadBalancer.Services;

public class Worker(string name, int maxLoad, int maxTasks) : IWorker
{
    public string Name { get; } = name;
    public int MaxLoad { get; } = maxLoad;
    public int MaxTasks { get; } = maxTasks;

    private int _currentLoad = 0;
    private int _currentTaskCount = 0;

    private readonly Lock _lock = new();

    public int GetCurrentLoad()
    {
        lock (_lock)
        {
            return _currentLoad;
        }
    }

    public bool CanTakeTask(TaskItem task)
    {
        lock (_lock)
        {
            return _currentLoad + task.Load <= MaxLoad && _currentTaskCount < MaxTasks;
        }
    }

    public async Task ExecuteTaskAsync(TaskItem task, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _currentLoad += task.Load;
            _currentTaskCount++;
        }

        await Task.Delay(task.ExecutionTime * 1000, cancellationToken);

        lock (_lock)
        {
            _currentLoad -= task.Load;
            _currentTaskCount--;
        }
    }

    public string GetStatus()
    {
        lock (_lock)
        {
            return $"{Name}: Загрузка {_currentLoad}/{MaxLoad}, задачи {_currentTaskCount}/{MaxTasks}";
        }
    }
}