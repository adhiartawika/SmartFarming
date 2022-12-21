namespace backend.Model.AppEntity
{
    public enum ScheduleTagType: byte{
        Pemeliharaan,
        Penyakit
    }
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public ScheduleTagType Tag { get; set; }
        public Land Land { get; set; }
        public IList<ScheduleLog> ScheduleLogs { get; } = new List<ScheduleLog>();
        public VirusMonitor? DiseaseMonitor { get; set; }
    }
}