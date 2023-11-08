using System;
using System.Threading.Tasks;

namespace TaskManager;

internal class Program
{
    internal static Random random = new();
    static async Task Main(string[] args)
    {
        TaskManager manager = new TaskManager(); // Initialize with an empty collection if needed

        // Add tasks
        for (int i = 0; i < 10; i++)
        {
            manager.AddTask(GetTask(i, 0, manager));
        }

        // Start and wait for all tasks to complete
        await manager.StartAsync();

        Console.WriteLine("All tasks have been processed.");
    }

    public static Task GetTask(int i, int page, IAddTask addTask)
    {
        return new Task(() =>
        {
            Console.WriteLine($"Starting task {i}, page {page}");
            var result = random.NextDouble(); // Simulate work
            Thread.Sleep((int)(result * 10000)); // Simulate asynchronous work
            var gotMore = result < 0.5;
            if (gotMore) //this is the "GotMore" on the result
                addTask.AddTask(GetTask(i, page + 1, addTask));
            Console.WriteLine($"Finished task {i}, page {page}, gotMore {gotMore}");
        });
    }
}