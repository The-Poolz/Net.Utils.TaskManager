using System.Collections.Concurrent;

namespace Net.Utils.TaskManager;

public class TaskManager : IAddTask
{
    private readonly ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
    private bool started = false;

    public TaskManager() {}

    public TaskManager(IEnumerable<Task> tasks) : this()
    {
        AddRange(tasks);
    }

    public void AddRange(IEnumerable<Task> tasks)
    {
        foreach (var task in tasks) AddTask(task);
    }

    public virtual void AddTask(Task task)
    {
        tasks.Add(task);
        if (started) task.Start();
    }

    public virtual async Task StartAsync()
    {
        if (started)
            throw new InvalidOperationException("Cannot start the task manager twice");
        started = true;
        Parallel.ForEach(tasks, (task) => { task.Start(); });
        await AwaitFinish();
    }

    public bool IsCompleted => tasks.All(t => t.IsCompleted);
    public virtual async Task AwaitFinish()
    {
        while (!IsCompleted)
        {
            await Task.WhenAll(tasks);
        }
    }
}