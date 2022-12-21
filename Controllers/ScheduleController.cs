using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext context;
        public ScheduleController(AppDbContext context) {
            this.context = context;
        }

    [HttpPost]
    public async Task<IActionResult> createSchedule([FromForm] CreateScheduleForm ScheduleForm){
        Console.WriteLine(ScheduleTagType.IsDefined(typeof(ScheduleTagType), ScheduleForm.Tag));
        
        if(ScheduleForm.EndDate == default(DateTime)) ScheduleForm.EndDate = ScheduleForm.StartingDate;
        else if(DateTime.Compare(ScheduleForm.EndDate, ScheduleForm.StartingDate) < 0 ) return BadRequest();
        try{
            var land = this.context.Lands.Where(x => x.Id == ScheduleForm.LandId).FirstOrDefault();
            var diseaseMonitor = this.context.VirusMonitors.Where(x => x.id == ScheduleForm.DiseaseMonitorId).FirstOrDefault();
            var schedule = new Schedule{
                Name = ScheduleForm.Name,
                StartingDate = ScheduleForm.StartingDate,
                EndDate = ScheduleForm.EndDate,
                Note = ScheduleForm.Note,
                Tag = (ScheduleTagType)Enum.Parse(typeof(ScheduleTagType), ScheduleForm.Tag),
                DiseaseMonitor = diseaseMonitor,
                Land = land,
            };
            this.context.Schedules.Add(schedule);
            await this.context.SaveChangesAsync();

            /* Get Occurrence*/
            this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                Date = schedule.StartingDate,
                ScheduleId = schedule.Id
            });
            if(ScheduleForm.IntervalType != null) getOccurrence(schedule, ScheduleForm.IntervalType, ScheduleForm.Count);
            await this.context.SaveChangesAsync();

            return new OkObjectResult(new AppResponse { message = "Berhasil menambahkan jadwal baru" });
        }
        catch (Exception ex)
            {
                throw ex;
            }

    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Schedule>> ShowDetailSchedule(int id){

        var response = (await this.context.Schedules
            .Where(x => x.Id == id)
            .Include(x => x.Land)
            .Include(x => x.DiseaseMonitor)
            .Include(x => x.ScheduleLogs)
            .ThenInclude(Sg => Sg.Images)
            .ToListAsync());
        return response.Count() == 0 ? NotFound() : Ok(
            response.Select(r => {
                return new ShowScheduleDetailResponse{
                    Id = r.Id,
                    Name = r.Name,
                    Land = r.Land.Name,
                    Tag = r.Tag,
                    Note = r.Note,
                    Logs = r.ScheduleLogs,
                };
            }));
    }

    [HttpGet]
    public async Task<ActionResult> ShowSchedules(int id){
        var result = await this.context.Schedules
            .Include(x => x.Land)
            .ToListAsync();
        if (result == null) return NotFound();

        var schedule = new List<ShowSchedulesResponse>();
        foreach(Schedule sc in result){
            schedule.Add(new ShowSchedulesResponse{
                Id = sc.Id,
                Name = sc.Name,
                StartingDate = sc.StartingDate,
                EndDate = sc.EndDate,
                Land = sc.Land.Name,
                Tag = sc.Tag,
            });
        }

        var occurrence = await this.context.ScheduleOccurrences.ToListAsync();

        return Ok(new { Schedule = schedule, occurrence = occurrence} );
    }
    
    [HttpPost]
    public async Task<IActionResult> createScheduleLog([FromForm] CreateLogForm LogForm){
        var schedule = this.context.Schedules.Where(x => x.Id == LogForm.ScheduleId).FirstOrDefault();
        if(schedule == null) return NotFound();

        var log = new ScheduleLog{
            Timestamp = LogForm.Date,
            Description = LogForm.Description,
            ScheduleId = LogForm.ScheduleId
        };
        this.context.ScheduleLogs.Add(log);
        await this.context.SaveChangesAsync();

        foreach (var img in LogForm.Images){
            if (img.Length > 500000) return BadRequest();
            
            using (var target = new MemoryStream()){
                img.CopyTo(target);
                var image = new ScheduleLogImage{
                    Byte = target.ToArray(),
                    Name = img.FileName,
                    ScheduleLogId = log.Id
                };
                this.context.ScheduleLogImages.Add(image);
            }
        }
        await this.context.SaveChangesAsync();

        return Ok(log);
        }

    public async void getOccurrence(Schedule schedule, string intervalType, int count){
        if(intervalType == "Hari"){
            var temp = schedule.StartingDate.AddDays(count);
            while(DateTime.Compare(temp, schedule.EndDate) <= 0 ){
                this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                    Date = temp,
                    ScheduleId = schedule.Id
                });
                temp = temp.AddDays(count);
            }
        }
        else if(intervalType == "Minggu"){
            count *= 7;
            var temp = schedule.StartingDate.AddDays(count);
            while(DateTime.Compare(temp, schedule.EndDate) <= 0 ){
                this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                    Date = temp,
                    ScheduleId = schedule.Id
                });
                temp = temp.AddDays(count);
            }
        }
        else if(intervalType == "Bulan"){
            var temp = schedule.StartingDate.AddMonths(count);
            while(DateTime.Compare(temp, schedule.EndDate) <= 0 ){
                this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                    Date = temp,
                    ScheduleId = schedule.Id
                });
                temp = temp.AddMonths(count);
            }
        }
        // ? Tanya/Cek ada per tahun
        else if(intervalType == "Tahun"){
            var temp = schedule.StartingDate.AddYears(count);
            while(DateTime.Compare(temp, schedule.EndDate) <= 0 ){
                this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                    Date = temp,
                    ScheduleId = schedule.Id
                });
                temp = temp.AddYears(count);
            }
        };
    }

    }
    public class CreateLogForm{
        [Required(ErrorMessage = "Jadwal tidak valid")]
        public int ScheduleId { get; set; }
        [Required(ErrorMessage = "Tanggal harus diisi")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Deskripsi harus diisi")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Gambar harus diisi")]
        public List<IFormFile> Images { get; set; }

    }
    public class CreateScheduleForm{
        [Required(ErrorMessage = "Nama harus diisi")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Tanggal Mulai Jadwal harus diisi")]
        public DateTime StartingDate { get; set; }
        [Required(ErrorMessage = "Petak harus diisi")]
        public int LandId { get; set; }
        [Required(ErrorMessage = "Catatan Jadwal harus diisi")]
        public string Note { get; set; }
        [Required(ErrorMessage = "Tag harus diisi")]
        public string Tag { get; set; }
        public int? DiseaseMonitorId { get; set; }

        // Interval
        public int Count { get; set; }
        public string? IntervalType { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ShowSchedulesResponse{
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Land { get; set; }
        public ScheduleTagType Tag { get; set; }
    }
    public class ShowScheduleDetailResponse{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Land { get; set; }
        public ScheduleTagType Tag { get; set; }
        public string Note { get; set; }
        public ICollection<ScheduleLog> Logs { get; set; }
    }
}