using LoadBalancer.Models;
using LoadBalancer.Services;
using LoadBalancer.Services.interfaces;

namespace LoadBalancer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите количество исполнителей:");
        _ = int.TryParse(Console.ReadLine(), out int workerCount);

        var workers = new List<IWorker>();
        for (int i = 0; i < workerCount; i++)
        {
            workers.Add(new Worker($"Worker-{i + 1}", maxLoad: 30, maxTasks: 10));
        }

        var taskQueue = new Queue<TaskItem>();
        var random = new Random();

        for (int i = 0; i < 100; i++)
        {
            taskQueue.Enqueue(new TaskItem(random.Next(1, 6), random.Next(2, 10)));
        }

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
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Балансировщик завершил работу.");
        }
    }
}