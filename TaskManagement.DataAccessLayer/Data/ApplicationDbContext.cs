using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccessLayer.Entities;


namespace TaskManagement.DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskItemStatus> TaskItemStatuses { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<Priority> Priorities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
            });

            // Task configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasOne(t => t.Creator)
                    .WithMany(u => u.TasksCreated)
                    .HasForeignKey(t => t.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Assignee)
                    .WithMany(u => u.TasksAssigned)
                    .HasForeignKey(t => t.AssignedTo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Type)
                    .WithMany()
                    .HasForeignKey(t => t.TypeId);

                entity.HasOne(t => t.Status)
                    .WithMany()
                    .HasForeignKey(t => t.StatusId);

                entity.HasOne(t => t.Priority)
                    .WithMany()
                    .HasForeignKey(t => t.PriorityId);
            });

            // Seed data
            modelBuilder.Entity<Priority>().HasData(
                new Priority { Id = 1, Name = "High", Level = 1 },
                new Priority { Id = 2, Name = "Medium", Level = 2 },
                new Priority { Id = 3, Name = "Low", Level = 3 }
            );

            modelBuilder.Entity<TaskItemStatus>().HasData(
                new TaskItemStatus { Id = 1, Name = "Pending", Description = "Task is pending" },
                new TaskItemStatus { Id = 2, Name = "Active", Description = "Task is active" },
                new TaskItemStatus { Id = 3, Name = "Completed", Description = "Task is completed" },
                new TaskItemStatus { Id = 4, Name = "Deleted", Description = "Task is deleted" }
            );

            modelBuilder.Entity<TaskType>().HasData(
                new TaskType { Id = 1, Name = "Travel", Description = "Travel related tasks" },
                new TaskType { Id = 2, Name = "Music", Description = "Music related tasks" },
                new TaskType { Id = 3, Name = "Sport", Description = "Sport related tasks" },
                new TaskType { Id = 4, Name = "Work", Description = "Work related tasks" },
                new TaskType { Id = 5, Name = "Personal", Description = "Personal tasks" }
            );
        }
    }
}