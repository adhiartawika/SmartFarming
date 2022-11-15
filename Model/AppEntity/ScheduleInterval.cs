namespace backend.Model.AppEntity
{
    public enum IntervalType: byte{
        Hari,
        Minggu,
        Bulan,
        Tahun
    }
    public enum WeekDays{
        Senin = 2,
        Selasa = 3,
        Rabu = 5,
        Kamis = 7,
        Jumat = 11,
        Sabtu = 13,
        Minggu = 17
    }

    public class ScheduleInterval
    {        
        public int Id { get; set; }
        public int Count { get; set; }
        public IntervalType IntervalType { get; set; }
        public int WeekDays { get; set; }   // Weekdays is multiplication of selected days
        public DateTime EndDate { get; set; }
    }
}