
    public class TaskFilterDto
    {
        public string Search { get; set; }
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public int? CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "CreatedOn";
        public bool SortDescending { get; set; } = true;
    }
