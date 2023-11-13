using Xunit;

namespace Net.Utils.TaskManager.Tests;

public class TaskManagerTests
{
    [Fact]
    public async Task CompleteOneTaskAdded()
    {
        var manager = new TaskManager();
        var initialTask = new Task(() => Thread.Sleep(100));

        manager.AddTask(initialTask);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task CompleteArrayOfTasksAdded()
    {
        IEnumerable<Task> tasks = new Task[]
        {
            new Task(() => Thread.Sleep(100)),
            new Task(() => Thread.Sleep(200)),
            new Task(() => Thread.Sleep(300))
        };

        var manager = new TaskManager(tasks);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task CompleteDynamicTasksAdded()
    {
        var manager = new TaskManager();

        var initialTask = new Task(() => // Simulates work and adds another task dynamically
        {        
            Thread.Sleep(100);
            manager.AddTask(new Task(() => Thread.Sleep(100)));
        });
        manager.AddTask(initialTask);

        await manager.StartAsync();

        Assert.True(manager.IsCompleted, "Not all tasks were completed.");
    }

    [Fact]
    public async Task StartAsync_ThrowsInvalidOperationException_IfCalledTwice()
    {
        var manager = new TaskManager();

        manager.AddTask(new Task(() => Thread.Sleep(100)));

        await manager.StartAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await manager.StartAsync());
    }

    [Fact (Skip = "Fails, need to fix")]
    public async Task IsCompleted_ReturnsFalse_WhenTasksAreStillRunning()
    {
        var manager = new TaskManager();
        var taskCompletionSource = new TaskCompletionSource<bool>();

        manager.AddTask(Task.Run(() => { taskCompletionSource.Task.Wait(); }));
        manager.AddTask(Task.Run(() => { Thread.Sleep(100); })); 

        var startTask = manager.StartAsync();

        await Task.Delay(200);

        Assert.False(manager.IsCompleted, "IsCompleted should be false while tasks are still running");

        // Cleanup - Not necessary
        taskCompletionSource.SetResult(true); // This will allow the longRunningTask to complete
        await startTask; 
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
}