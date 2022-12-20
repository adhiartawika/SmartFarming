namespace backend.Model.AppEntity
{
    public class ScheduleLog
    {        
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public IList<ScheduleLogImage> Images { get; } = new List<ScheduleLogImage>();
        public int? ScheduleId { get; set; }
    }
}