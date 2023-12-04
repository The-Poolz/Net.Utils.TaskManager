![linkedin_banner_image_1](https://github.com/The-Poolz/Net.Utils.TaskManager/assets/143507456/d7f973c6-fed0-4f6a-85ff-766e392fdebc)
# Net.Utils.TaskManager
[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/summary/new_code?id=The-Poolz_Net.Utils.TaskManager)

[![SonarCloud](https://github.com/The-Poolz/Net.Utils.TaskManager/actions/workflows/sonarcloud.yml/badge.svg)](https://github.com/The-Poolz/Net.Utils.TaskManager/actions/workflows/sonarcloud.yml)
[![CodeFactor](https://www.codefactor.io/repository/github/the-poolz/net.utils.taskmanager/badge)](https://www.codefactor.io/repository/github/the-poolz/net.utils.taskmanager)
[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](https://github.com/The-Poolz/Net.Utils.TaskManager/blob/master/LICENSE)

C# library offering an advanced asynchronous Task Manager optimized for handling concurrent task execution with a focus on dynamic task queuing.
It is specifically designed to allow runtime addition of tasks, ensuring seamless integration of newly discovered tasks into the ongoing execution process.
The library demonstrates effective use of Task parallelism and continuation in .NET.

## Features
- Concurrent and asynchronous task execution.
- Ability to add and handle tasks dynamically after the task manager has started.
- Efficient handling of tasks with continuation support.
- Extensible design with [`ITaskManager`](#itaskmanager) interface for customizable task addition.

## Handling of Dynamic Tasks

One of the key features of `Net.Utils.TaskManager` is its ability to handle tasks that are added dynamically, even after the task execution has started.
This is useful in scenarios where a task's execution might lead to the discovery of new tasks that need to be run in the same cycle.

## Initialization

To begin using the `Net.Utils.TaskManager`, you must first create an instance of the `TaskManager` class:
```csharp
var taskManager = new TaskManager();
```

Alternatively, if you already have a collection of tasks to execute, you can initialize the `TaskManager` with them:
```csharp
var initialTasks = new List<Task> { /* your tasks here */ };
var taskManager = new TaskManager(initialTasks);
```

## Methods

The `TaskManager` class provides the following methods:

- `AddTask(Task task)` : Adds a single task to the manager.
- `AddRange(IEnumerable<Task> tasks)` : Adds a collection of tasks to the manager.
- `StartAsync()` : Starts executing the tasks asynchronously.
- `AwaitFinish()` : Awaits the completion of all tasks.

#### `AddTask`
```csharp
// Adds a new task to the manager
taskManager.AddTask(new Task(() => DoWork()));
```

#### `AddRange`
```csharp
// Adds a range of tasks at once
taskManager.AddRange(new List<Task> { task1, task2, task3 });
```

#### `StartAsync`
```csharp
// Starts the task manager and executes all tasks
await taskManager.StartAsync();
```

#### `AwaitFinish`
```csharp
// Waits for all tasks to complete
await taskManager.AwaitFinish();
```

## ITaskManager

The `ITaskManager` interface is designed to allow for extensibility in task management.
By implementing this interface, developers can customize how tasks are added to the manager, enabling integration with different task sources or adding additional logic when tasks are registered.

```csharp
public class CustomTaskManager : ITaskManager
{
    private readonly TaskManager manager;

    public CustomTaskManager(TaskManager taskManager)
    {
        manager = taskManager;
    }

    public void AddTask(Task task)
    {
        // Custom logic before adding a task
        manager.AddTask(task);
    }

    public void AddRange(IEnumerable<Task> tasks)
    {
        // Custom logic before adding a range of tasks
        manager.AddRange(tasks);
    }
}
```
