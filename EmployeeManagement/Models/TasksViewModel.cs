using System.Collections.Generic;
using EmployeeManagement.Models;


    namespace EmployeeManagement.ViewModels
    {
        public class TaskViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Completed { get; set; }
            public DateTime? duedate { get; set; }
        
    }
    }




