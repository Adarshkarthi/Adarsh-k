using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.ViewModels;

public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        _logger.LogInformation($"Retrieved username from session: {username}");

        try
        {
            var tasks = await _taskService.GetTasksByUsernameAsync(username);
            var taskViewModels = tasks.Select(task => new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Completed = task.Completed,
                duedate = task.duedate
            }).ToList();

            ViewBag.Username = username; // Pass username to the view
            return View(taskViewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        var taskViewModel = new TaskViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Completed = task.Completed,
            duedate = task.duedate
        };

        return View(taskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            await _taskService.CompleteTaskAsync(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing the task");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            await _taskService.AddTaskForUserAsync(username, model.Title, model.Description, model.duedate);
            return RedirectToAction("Index");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            var taskViewModel = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Completed = task.Completed,
                duedate = task.duedate
            };

            return View(taskViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading the edit view");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Fetch the existing task entity from the database
                var existingTask = await _taskService.GetTaskByIdAsync(model.Id);

                if (existingTask == null)
                {
                    return NotFound();
                }

                // Update the task properties with the values from the view model
                existingTask.Title = model.Title;
                existingTask.Description = model.Description;
                existingTask.Completed = model.Completed;
                existingTask.duedate = model.duedate;

                // Save the updated task back to the database
                await _taskService.UpdateTaskAsync(existingTask);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the task");
                return StatusCode(500, "Internal server error");
            }
        }

        // If ModelState is not valid, redisplay the form with validation errors
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return RedirectToAction("Index"); // Redirect to the list of tasks after deletion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting the task");
            return StatusCode(500, "Internal server error");
        }
    }

}
