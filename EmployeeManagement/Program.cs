using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using EmployeeManagement.Data;
//using EmployeeManagement.Services; // Ensure this namespace is correct

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<ITaskService, TaskService>(); // Ensure TaskService implements ITaskService
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add in-memory caching
builder.Services.AddDistributedMemoryCache();

// Configure session management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout period for the session
    options.Cookie.HttpOnly = true; // Set the session cookie to be HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line if you're using authentication
app.UseAuthorization();

app.UseSession(); // Enable session middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
