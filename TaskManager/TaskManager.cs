using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager;

public class TaskManager : IAddTask
{
    private readonly ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
    private bool started = false;

    public void AddTask(Task task)
    {
        tasks.Add(task);
        if (started) task.Start();
    }

    public async Task StartAsync()
    {
        if (started)
            throw new InvalidOperationException("Cannot start the task manager twice");
        started = true;
        Parallel.ForEach(tasks, (task) => { task.Start(); });
        await AwaitFinish();
    }

    public bool IsCompleted => tasks.All(t => t.IsCompleted);
    public async Task AwaitFinish()
    {
        while (!IsCompleted)
        {
            await Task.WhenAll(tasks);
        }
    }
}