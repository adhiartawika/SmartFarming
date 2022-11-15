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
        if(DateTime.Compare(ScheduleForm.EndDate, ScheduleForm.StartingDate) < 0 )return BadRequest();
        try{
            var land = this.context.Lands.Where(x => x.Id == ScheduleForm.LandId).FirstOrDefault();
            var tag = this.context.ScheduleTags.Where(x => x.Id == ScheduleForm.ScheduleTagId).FirstOrDefault();
            var disease = this.context.Disease.Where(x => x.Id == ScheduleForm.DiseaseId).FirstOrDefault();
            var interval = Enum.IsDefined(typeof(IntervalType), ScheduleForm.IntervalType) ? new ScheduleInterval{
                Count = ScheduleForm.Count,
                IntervalType = (IntervalType)Enum.Parse(typeof(IntervalType),ScheduleForm.IntervalType) ,
                WeekDays = ScheduleForm.WeekDays,
                EndDate = ScheduleForm.EndDate
                } : null;
            var schedule = new Schedule{
                Name = ScheduleForm.Name,
                StartingDate = ScheduleForm.StartingDate,
                Note = ScheduleForm.Note,
                Disease = disease,
                ScheduleTag = tag,
                Land = land,
                ScheduleInterval = interval
            };

            /* Get First Occurrence*/
            this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                Date = schedule.StartingDate,
                Schedule = schedule
            });

            if(interval != null){
                this.context.ScheduleIntervals.Add(interval);
                getOccurrence(schedule);
            }
            this.context.Schedules.Add(schedule);
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
            .Include(x => x.Disease)
            .Include(x => x.ScheduleTag)
            // .Include(x => x.ScheduleInterval)
            .Include(x => x.ScheduleLogs)
            .ThenInclude(Sg => Sg.Images)
            .ToListAsync());
        return response.Count() == 0 ? NotFound() : Ok(
            response.Select(r => {
                return new ShowScheduleDetailResponse{
                    Id = r.Id,
                    Name = r.Name,
                    Land = r.Land.Name,
                    tag = r.ScheduleTag.Name,
                    Note = r.Note,
                    logs = r.ScheduleLogs,
                };
            }));
    }

    [HttpGet]
    public async Task<ActionResult<Schedule>> ShowSchedules(int id){
        var result = await this.context.Schedules
            .Include(x => x.Land)
            .Include(x => x.ScheduleTag)
            .Include(x => x.ScheduleInterval)
            .ToListAsync();
        if (result == null) return NotFound();

        var response = new List<ShowSchedulesResponse>();
        foreach(Schedule sc in result){
            response.Add(new ShowSchedulesResponse{
                Id = sc.Id,
                Name = sc.Name,
                StartingDate = sc.StartingDate,
                EndDate = sc.ScheduleInterval == null ? sc.StartingDate :sc.ScheduleInterval.EndDate,
                land = sc.Land.Name,
                tag = sc.ScheduleTag.Name,
            });
        }
        return Ok(response);
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

    public async void getOccurrence(Schedule schedule){
        /* Get occurrence type Day */
        if(schedule.ScheduleInterval.IntervalType == IntervalType.Hari){
            var temp = schedule.StartingDate.AddDays(schedule.ScheduleInterval.Count);
            while(DateTime.Compare(temp, schedule.ScheduleInterval.EndDate) <= 0 ){
                this.context.ScheduleOccurrences.Add(new ScheduleOccurrence{
                    Date = temp,
                    Schedule = schedule
                });
                temp = temp.AddDays(schedule.ScheduleInterval.Count);
            }
        };
        return;
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
        public int ScheduleTagId { get; set; }
        public int DiseaseId { get; set; }

        // Interval
        public int Count { get; set; }
        public string IntervalType { get; set; }
        public int WeekDays { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ShowSchedulesResponse{
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndDate { get; set; }
        public string land { get; set; }
        public string tag { get; set; }
    }
    public class ShowScheduleDetailResponse{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Land { get; set; }
        public string tag { get; set; }
        public string Note { get; set; }
        public ICollection<ScheduleLog> logs { get; set; }
    }
}