using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly string _connectionString;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
        _connectionString = _context.Database.GetDbConnection().ConnectionString;

    }

    public async Task<List<TaskEntity>> GetTasksByUsernameAsync(string username)
        {
        var tasks = new List<TaskEntity>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetTasksByUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Username", username));

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var task = new TaskEntity
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                userid = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Completed = reader.GetBoolean(reader.GetOrdinal("Completed")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("duedate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("duedate"))
                            };
                            tasks.Add(task);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log detailed error information
            _logger.LogError(ex, "Error retrieving tasks for user: {Username}", username);
            throw; // Re-throw exception to propagate it up the stack
        }

        return tasks;
    }


    public async Task<TaskEntity> GetTaskByIdAsync(int taskId)
    {
        try
        {
            // Define the stored procedure name and parameters
            var sql = "GetTaskById";
            var parameters = new[]
            {
            new SqlParameter("@TaskId", taskId)
        };

            // Execute the stored procedure
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TaskEntity
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                userid = reader.GetInt32(reader.GetOrdinal("UserId")),
                                Completed = reader.GetBoolean(reader.GetOrdinal("Completed")),
                                duedate = reader.IsDBNull(reader.GetOrdinal("DueDate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("DueDate"))
                            };
                        }
                    }
                }
            }

            return null; // Return null if no result is found
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error retrieving task by Id");
            throw; // Rethrow the exception if needed
        }
    }


    public async Task AddTaskForUserAsync(string username, string taskTitle, string taskDescription, DateTime? dueDate)
    {
        var parameters = new[]
        {
            new SqlParameter("@Title", taskTitle),
            new SqlParameter("@Description", taskDescription),
            new SqlParameter("@AssignedUser", username),
            new SqlParameter("@DueDate", (object)dueDate ?? DBNull.Value)
        };

        await _context.Database.ExecuteSqlRawAsync("EXEC CreateTask @Title, @Description, @AssignedUser, @DueDate", parameters);
    }

    public async Task CompleteTaskAsync(int taskId)
    {
        var taskIdParam = new SqlParameter("@TaskId", taskId);
        await _context.Database.ExecuteSqlRawAsync("EXEC CompleteTask @TaskId", taskIdParam);
    }

    public async Task UpdateTaskAsync(int taskId, string title, string description, bool completed, DateTime? dueDate)
    {
        var sql = @"UPDATE Tasks SET Title = @Title, Description = @Description, Completed = @Completed, duedate = @DueDate WHERE Id = @TaskId";
        var parameters = new[]
        {
            new SqlParameter("@Title", title),
            new SqlParameter("@Description", description),
            new SqlParameter("@Completed", completed),
            new SqlParameter("@TaskId", taskId),
            new SqlParameter("@DueDate", (object)dueDate ?? DBNull.Value)
        };

        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var taskIdParam = new SqlParameter("@TaskId", taskId);
        await _context.Database.ExecuteSqlRawAsync("EXEC DeleteTask @TaskId", taskIdParam);
    }

    public Task UpdateTaskAsync(Task task)
    {
        throw new NotImplementedException();
    }
}
