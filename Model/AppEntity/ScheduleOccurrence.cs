namespace backend.Model.AppEntity
{
    public class ScheduleOccurrence
    {        
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Schedule Schedule { get; set; }
    }
}