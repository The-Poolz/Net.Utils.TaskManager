namespace Net.Utils.TaskManager;

public interface ITaskManager
{
    void AddTask(Task task);
    void AddRange(IEnumerable<Task> tasks);
}