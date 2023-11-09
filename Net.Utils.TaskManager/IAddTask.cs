namespace Net.Utils.TaskManager
{
    public interface IAddTask
    {
        void AddTask(Task task);
        void AddRange(IEnumerable<Task> tasks);
    }
}