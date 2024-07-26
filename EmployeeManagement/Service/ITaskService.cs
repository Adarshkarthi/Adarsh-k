using EmployeeManagement.Models;

public interface ITaskService
{
    Task<List<TaskEntity>> GetTasksByUsernameAsync(string username);
    Task AddTaskForUserAsync(string username, string taskTitle, string taskDescription, DateTime? dueDate);
    Task<TaskEntity> GetTaskByIdAsync(int taskId);
    Task UpdateTaskAsync(TaskEntity task);
    Task CompleteTaskAsync(int taskId);
    Task UpdateTaskAsync(Task task);
    Task DeleteTaskAsync(int taskId);

}
