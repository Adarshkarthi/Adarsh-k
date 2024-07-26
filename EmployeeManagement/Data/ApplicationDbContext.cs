using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models; // Ensure this matches the namespace of your models

namespace EmployeeManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        //public DbSet<TaskEntity> Tasks { get; set; }

        public DbSet<User> Users { get; set; }
       public DbSet<EmployeeManagement.Models.TaskEntity> Tasks { get; set; }
      

       
    }
}
