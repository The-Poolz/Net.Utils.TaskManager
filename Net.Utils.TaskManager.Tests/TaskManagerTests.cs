using System.Collections.Concurrent;
using Xunit;

namespace Net.Utils.TaskManager.Tests;

public class TaskManagerTests
{
    [Fact]
    public async Task TaskManager_CompleteOneTaskAdded()
    {
        var manager = new TaskManager();
        var initialTask = new Task(() => Task.Delay(100));

        manager.AddTask(initialTask);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task TaskManager_CompletesMultipleAddedTasks()
    {
        IEnumerable<Task> tasks = new Task[]
        {
            new Task(() => Task.Delay(100)),
            new Task(() => Task.Delay(200)),
            new Task(() => Task.Delay(300))
        };

        var manager = new TaskManager(tasks);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task TaskManager_CompletesTasksWhenDynamicallyAdded()
    {
        var manager = new TaskManager();

        var initialTask = new Task(() => // Simulates work and adds another task dynamically
        {        
            Task.Delay(100);
            manager.AddTask(new Task(() => Task.Delay(100)));
        });
        manager.AddTask(initialTask);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task TasksAddedAfterStart_ShouldStartImmediately()
    {
        var manager = new TaskManager();
        await manager.StartAsync(); // Start with no tasks

        var taskStarted = false;
        var task = new Task(() => { taskStarted = true; });
        manager.AddTask(task);
        await Task.Delay(50);

        Assert.True(taskStarted, "Task added after `StartAsync` should've started immediately.");
    }

    [Fact]
    public async Task StartAsync_ThrowsInvalidOperationException_IfCalledTwice()
    {
        var manager = new TaskManager();

        manager.AddTask(new Task(() => Task.Delay(100)));

        await manager.StartAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.StartAsync());
    }

    [Fact]
    public async Task AwaitFinish_OnlyReturns_WhenAllTasksAreComplete()
    {
        var manager = new TaskManager();
        var shortTask = Task.Delay(100);
        var longTask = Task.Delay(1000);

        manager.AddTask(shortTask);
        manager.AddTask(longTask);

        _ = manager.StartAsync();
        await manager.AwaitFinish();

        Assert.True(shortTask.IsCompleted, "Short task should be completed if `AwaitFinish` returned");
        Assert.True(longTask.IsCompleted, "Long task should be completed if `AwaitFinish` returned");
        Assert.True(manager.IsCompleted, "All tasks should be completed if `AwaitFinish` returned");
    }

    [Fact]
    public async Task ShouldAllowConcurrentTaskAdditions()
    {
        var manager = new TaskManager();
        var numberOfTasksToAdd = 100;
        var tasks = new ConcurrentBag<Task>();

        Parallel.For(0, numberOfTasksToAdd, (i) =>
        {
            var task = new Task(() => { Task.Delay(10 * i); });
            manager.AddTask(task);
            tasks.Add(task);
        });

        await manager.StartAsync();
        await Task.WhenAll(tasks);

        Assert.Equal(numberOfTasksToAdd, tasks.Count(t => t.IsCompleted));
    }
}