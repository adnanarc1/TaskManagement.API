using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DataAccessLayer.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        public virtual User Creator { get; set; }
        public virtual User Assignee { get; set; }
        public virtual TaskType Type { get; set; }
        public virtual TaskItemStatus Status { get; set; }
        public virtual Priority Priority { get; set; }
    }
}