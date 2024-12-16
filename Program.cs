using LoadBalancer.Models;
using LoadBalancer.Services;
using LoadBalancer.Services.interfaces;

namespace LoadBalancer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите количество исполнителей:");
        _ = int.TryParse(Console.ReadLine(), out int workersCount);

        var workers = GenerateWorkers(workersCount);
        var taskQueue = GenerateTasks();

        var balancer = new Balancer(workers, taskQueue);
        var consoleMonitor = new ConsoleMonitor(balancer);
        var cts = new CancellationTokenSource();

        try
        {
            var distributionTask = balancer.DistributeAsync(cts.Token);
            var monitorTask = consoleMonitor.MonitorAsync(cts.Token);

            await distributionTask;
            cts.Cancel();

            await monitorTask;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Задача отменена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Балансировщик завершил работу.");
        }
    }

    static private List<IWorker> GenerateWorkers(int workersCount = 3, int maxLoad = 30, int maxTasks = 10)
    {
        var workers = new List<IWorker>();
        for (int i = 0; i < workersCount; i++)
        {
            workers.Add(new Worker($"Worker-{i + 1}", maxLoad, maxTasks));
        }

        return workers;
    }

    static private Queue<TaskItem> GenerateTasks(int tasksCount = 100)
    {
        var taskQueue = new Queue<TaskItem>();
        var random = new Random();

        for (int i = 0; i < tasksCount; i++)
        {
            taskQueue.Enqueue(new TaskItem(random.Next(1, 6), random.Next(2, 10)));
        }

        return taskQueue;
    }
}