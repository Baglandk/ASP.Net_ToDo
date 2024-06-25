using Microsoft.EntityFrameworkCore;

namespace Trelloo.Models
{
    public class ToDoContext: DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options): base(options) 
        { 
        }

        public DbSet<Todo> ToDos { get; set; } = null!;
        public DbSet<Category> categories { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "work", CategoryName = "Work"},
                new Category { CategoryId = "home", CategoryName = "Home"},
                new Category { CategoryId = "ex", CategoryName = "Exercise" },
                new Category { CategoryId = "shop", CategoryName = "Shopping" },
                new Category { CategoryId = "Call", CategoryName = "Contact" }
                );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = "open", StatusName = "Open"},
                new Status { StatusId = "closed", StatusName = "Completed" }
                );
        }

    }
}
