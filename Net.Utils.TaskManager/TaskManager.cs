using System.Collections.Concurrent;

namespace Net.Utils.TaskManager;

public class TaskManager : IAddTask
{
    private readonly ConcurrentBag<Task> _tasks;
    private bool started;
    public bool IsCompleted => _tasks.All(t => t.IsCompleted);

    public TaskManager()
    {
        _tasks = new ConcurrentBag<Task>();
    }

    public TaskManager(IEnumerable<Task> tasks) : this()
    {
        AddRange(tasks);
    }

    public void AddTask(Task task)
    {
        _tasks.Add(task);
        if (started) task.Start();
    }

    public void AddRange(IEnumerable<Task> tasks)
    {
        tasks.ToList().ForEach(AddTask);
    }

    public async Task StartAsync()
    {
        if (started) throw new InvalidOperationException("Cannot start the task manager twice");
        started = true;
        Parallel.ForEach(_tasks, task => { task.Start(); });
        await AwaitFinish();
    }

    private async Task AwaitFinish()
    {
        while (!IsCompleted)
        {
            await Task.WhenAll(_tasks);
        }
    }
}