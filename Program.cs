using LoadBalancer.Models;
using LoadBalancer.Services;

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

        var loadBalancer = new Balancer(workers, taskQueue);
        var cts = new CancellationTokenSource();
        var distributionTask = loadBalancer.DistributeTasksAsync(cts.Token);

        while (!cts.Token.IsCancellationRequested)
        {
            Console.Clear();
            Console.WriteLine(loadBalancer.GetStatus());
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            await Task.Delay(2000);

            if (Console.KeyAvailable)
            {
                cts.Cancel();
            }
        }

        await distributionTask;
    }
}