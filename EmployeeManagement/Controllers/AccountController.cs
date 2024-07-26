using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly string _connectionString;

    public AccountController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // GET: Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    [HttpPost]
   
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = GetUserByUsername(model.Username);

            if (user != null)
            {
                // Set username in session
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Tasks");
            }
            ModelState.AddModelError("", "Invalid username or password");
        }
        return View(model);
    }

    private User GetUserByUsername(string username)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "EXEC getUserByUsername @Username"; // Replace with your stored procedure name
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", username);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return new User
                    {
                        Username = reader["Username"].ToString(),
                        // Add other user properties as needed based on your database schema
                    };
                }
            }
        }
        return null;
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("Username");
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Register model)
    {
        if (ModelState.IsValid)
        {
            // No password hashing implemented (security risk)
            int newUserID = CreateUser(model.Username, model.password);

            if (newUserID > 0)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "User registration failed.");
            }
        }
        return View(model);
    }

    private int CreateUser(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "EXEC createUser @Username, @Password"; // Replace with your stored procedure name
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            int newUserID = 0; // Initialize to 0

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    // Assuming the stored procedure returns the new user ID
                    newUserID = Convert.ToInt32(reader["NewUserID"]); // Get the ID from the stored procedure
                }
            }

            return newUserID;
        }
    }
}
