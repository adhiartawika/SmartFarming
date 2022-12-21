using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class MonitorVirusController:ControllerBase{
        private readonly AppDbContext context;

        private readonly ICurrentUserService currentUser;
        private readonly IUtilityCurrentUserAces UserAcess;
        public MonitorVirusController(AppDbContext context,IUtilityCurrentUserAces UserAcess, ICurrentUserService currentUser){
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
        }

        [HttpPost]
        public async Task<int> AddMonitorVirus ([FromBody] AddDiseaseMonitorDto model){
            if(this.currentUser.RoleId != 3){
                List<LanLatDiseases> Cordinate = new List<LanLatDiseases>();
                for( int i = 0; i < model.Cordinate.Count() ; i++){
                    Cordinate.Add( new LanLatDiseases{
                        latitude = model.Cordinate.ElementAt(i).Latitude,
                        longitude = model.Cordinate.ElementAt(i).Longtitude,
                    });
                }
                var datamonitor = new VirusMonitor
                {
                    VirusId = model.IdDisease,
                    RegionId = model.IdRegion,
                    MonitorStatusId = model.IdStatus,
                    CreatedById = this.currentUser.UserId,
                    LanLatDiseases = Cordinate
                };
                // if(form.ImageDisease.Count > 0){
                //     foreach(var item in form.ImageDisease){
                //         var special = Guid.NewGuid().ToString();
                //         var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Utility\Images", special+item.FileName);

                //         using( var file = new FileStream(filePath, FileMode.Create))
                //         {
                //             item.CopyTo(file);
                //         }
                //         obj.Add(filePath);
                //     }
                // }
                // datamonitor.ImageDisease = string.Join(",",obj);
                var temp = this.context.VirusMonitors.Add(datamonitor);
                var res = await this.context.SaveChangesAsync(new CancellationToken());
                for( int i =0; i< model.DiseaseImages.Count(); i++){
                    var result = this.context.DiseaseImages.Where(x => x.Id == model.DiseaseImages.ElementAt(i).Id).FirstOrDefault();
                    result.VirusMonitorId = temp.Entity.id;
                    await this.context.SaveChangesAsync(new CancellationToken());
                }
                return temp.Entity.id;
            }else{
                return 0;
            }
        }

        [HttpGet("{DiseaseMonitorId:int?}")]
        public async Task<IEnumerable<DiseaseMonitorDetail>> DetailDisease (int DiseaseMonitorId){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var CheckDiseaseId = this.context.VirusMonitors.Where(x => x.DeletedAt == null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.id == DiseaseMonitorId).FirstOrDefault();
            if(CheckDiseaseId != false){
                var longlat = this.context.LanLatDiseases.Where(x => x.VirusMonitorId == DiseaseMonitorId)
                .Select(x => new CordinateDiseaseDto{
                    Id = x.Id,
                    Latitude = x.latitude,
                    Longtitude = x.longitude,
                }).ToList();
                var diseaseImg = this.context.VirusMonitors.Where(x => x.id == DiseaseMonitorId).Select(x => x.ImageDisease).FirstOrDefault();
                var diseaseImgtoList = diseaseImg.Split(',').ToList();
                List<DiseaseImageItem> Disease = new List<DiseaseImageItem>();
                for(int i = 0; i < diseaseImgtoList.Count(); i++){
                    Disease.Add( new DiseaseImageItem{
                        PathPhoto  = diseaseImgtoList[i],
                        Id = i,
                    });
                }
                var disease = this.context.VirusMonitors.Include(x=> x.LanLatDiseases).Where(x => x.id == DiseaseMonitorId)
                .Select(x => new DiseaseMonitorDetail{
                    Id = x.id,
                    DiseaseName= x.Virus.Nama,
                    RegionName = x.Region.Name,
                    Condition = x.MonitorStatus.Name,
                    LatLanRegions = longlat,
                    PathPhoto = Disease
                }).ToList();
                return disease;
            }
                return null;
        }
        [HttpPut("{DiseaseMonitorId:int?}")]
        public async Task<IActionResult> UpdateDetailDisease (int DiseaseMonitorId,[FromForm] UpdateDiseasMonitor form){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList(); 
            var CheckDiseaseId = this.context.VirusMonitors.Where(x=>x.DeletedAt==null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.id == DiseaseMonitorId).FirstOrDefault();
            if(this.currentUser.RoleId != 3 && CheckDiseaseId != false){
                var result = await this.context.VirusMonitors.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.id==DiseaseMonitorId);
                var resDisease = await this.context.PlantViruses.Where(x => x.id == result.VirusId).FirstOrDefaultAsync();
                result.VirusId = form.IdDisease;
                result.RegionId = form.IdRegion;
                result.MonitorStatusId = form.IdStatus;
                // var diseaseImgtoList = result.ImageDisease.Split(',').ToList();
                // if(form.Photo != null && form.Photo.Count > 0){
                //     foreach(var item in form.Photo){
                //         var special = Guid.NewGuid().ToString();
                //         var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Utility\Images", special+item.FileName);

                //         using( var file = new FileStream(filePath, FileMode.Create))
                //         {
                //             item.CopyTo(file);
                //         }
                //         diseaseImgtoList.Add(filePath);
                //     }
                //     result.ImageDisease = string.Join(",",diseaseImgtoList);
                // }
                result.ImageDisease = form.PathPhoto;
                var res = await this.context.SaveChangesAsync(new CancellationToken());
                return new OkObjectResult(new AppResponse { message = "Berhasil" });          
            }
            return new BadRequestObjectResult(new AppResponse { message="Akses Tidak Ditemukan"});
        }
        [HttpGet]
        public IEnumerable<DiseaseMinimalItemDto> GetDiseaseType(){           
            List<DiseaseMinimalItemDto> types = new List<DiseaseMinimalItemDto>();
            foreach(var j in this.context.PlantViruses){
                DiseaseMinimalItemDto temp = new DiseaseMinimalItemDto{
                    Id = j.id,
                    Nama = j.Nama
                };
                types.Add(temp);
            }
            return types;
        }
        
        [HttpGet("{RegionId:int?}")]
        public async Task<DiseaseSearch> Search([FromQuery] SearchRequest query, int RegionId = -1)
        {
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.VirusMonitors.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(x => x.Virus)
            .Include(x => x.Region)
            .Include(x => x.MonitorStatus)
            .Include(x=>x.LanLatDiseases)
            .Where(x => x.DeletedAt == null)
            .Where(x=>RegionId==-1? true: x.RegionId==RegionId)
            .Where(x => x.Virus.Nama.ToLower().Contains(query.Search));
            
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.id).ToListAsync()).Select(x =>
            {
                return new DiseaseItemDto
                {  
                    Id = x.id,
                    Name = x.Virus.Nama,
                    RegionName = x.Region.Name,
                    Condition = x.MonitorStatus.Name,
                    MonitorDate = x.CreatedAt,
                };
            }).ToList();
            return new DiseaseSearch
            {
                Data = res,
                NTotal = q.Count()
            };
        }
    }
    public class CordinateDiseaseDto{
        public int Id {get;set;}
        public double Latitude {get;set;}
        public double Longtitude {get;set;}
    }
    public class DiseaseImageItem{
        public int Id {get;set;}
        public string PathPhoto {get;set;}
    }
    public class DiseaseImageId{
        public int Id {get;set;}
    }
    public class AddDiseaseMonitorDto{
        public int IdDisease {get;set;}
        public int IdRegion {get;set;}
        public int IdStatus {get;set;}
        public string? PathPhoto { get; set; }
        public ICollection<DiseaseImageId>? DiseaseImages {get;set;}
        public ICollection<CordinateDiseaseDto>? Cordinate{get;set;}
    }

    public class DiseaseMinimalItemDto{ //combobox
        public int Id {get; set;}
        public string Nama {get;set;}
    }
    public class RegionItemDto{ //combobox
        public int Id {get; set;}
        public string Nama {get;set;}
    }

    public class DiseaseMonitorDetail{
        public int Id {get;set;}
        public string DiseaseName {get;set;}
        public string RegionName {get;set;}
        public string Condition  {get;set;}
        public ICollection<DiseaseImageItem>? PathPhoto {get;set;}
        public ICollection<CordinateDiseaseDto> LatLanRegions {get;set;}
    }


    public class UpdateDiseasMonitor{
        public int IdDisease {get;set;}
        public int IdRegion {get;set;}
        public int IdStatus {get;set;}
        public string PathPhoto { get; set; }
    }
    public class DiseaseItemDto{
        public int Id {get;set;}
        public string Name {get;set;}
        public string RegionName {get;set;}
        public string Condition {get;set;}
        public DateTime MonitorDate {get;set;}
    }
    public class DiseaseSearch : SearchResponse<DiseaseItemDto>
    {

    }
}