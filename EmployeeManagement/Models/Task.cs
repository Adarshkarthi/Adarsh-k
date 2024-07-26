namespace EmployeeManagement.Models
{
    public class TaskEntity
    {
        // Rename Task to TaskModel
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public int userid { get; set; }
        public DateTime? duedate { get; set; }
        

    }




}
