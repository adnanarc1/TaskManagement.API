
    public class CreateTaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
    }
