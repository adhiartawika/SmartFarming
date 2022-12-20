namespace backend.Model.AppEntity
{
    public class ScheduleLogImage
    {        
        public int Id { get; set; }
        public byte[] Byte { get; set; }
        public string Name { get; set; }
        public int? ScheduleLogId { get; set; }
    }
}