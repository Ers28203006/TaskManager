namespace TaskManager.DataAccess
{
    using System.Data.Entity;
    using TaskManager.Models;

    public class TasksContext : DbContext
    {
        public TasksContext()
            : base("name=TasksContext")
        {
        }

        public DbSet<Task> Tasks { get; set; }
    }
}