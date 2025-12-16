namespace TaskManagement.DataAccessLayer.Entities
{
    public class TaskType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public virtual TaskType Parent { get; set; }
    }
}
