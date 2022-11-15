namespace backend.Model.AppEntity
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartingDate { get; set; }
        public string Note { get; set; }

        public Disease Disease { get; set; }
        public ScheduleTag ScheduleTag { get; set; }
        public Land Land { get; set; }
        public ScheduleInterval? ScheduleInterval { get; set; }
        public IList<ScheduleLog> ScheduleLogs { get; } = new List<ScheduleLog>();
        
    }
}