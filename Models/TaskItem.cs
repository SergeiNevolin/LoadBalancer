namespace LoadBalancer.Models;

public class TaskItem(int load, int executionTime)
{
    public int Load { get; set; } = load;
    public int ExecutionTime { get; set; } = executionTime;
}
