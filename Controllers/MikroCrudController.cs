using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MikroCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public MikroCrudController(AppDbContext context){

            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<MicroItemDto>> ShowMicro(){
            return (await this.context.Mikrokontrollers
            .Include(x => x.MiniPcs).ThenInclude(x =>x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPcs).ThenInclude(x =>x.Region).ThenInclude(x=>x.Plant)
            .ToListAsync()).Select(y => new MicroItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MiniPcId = y.MiniPcs.Id,
                MiniPcName = y.MiniPcs.Name,
                RegionId = y.MiniPcs.Region.Id,
                RegionName = y.MiniPcs.Region.Name,
                LandId=y.MiniPcs.Region.LandId,
                LandName=y.MiniPcs.Region.Land.Name,
                PlantId=y.MiniPcs.Region.PlantId,
                PlantName=y.MiniPcs.Region.Plant.Name
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MicroItemDto>> ShowOverviewMicro(int LandId, [FromQuery] MicrosIdenity model){
            return (await this.context.Mikrokontrollers
            .Include(x => x.MiniPcs)
            .ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPcs)
            .ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Include(x=>x.Sensor)
            .Where(x=>LandId==-1? true: x.MiniPcs.Region.LandId==LandId)
            .Where(x=>model.Ids == null ? false:model.Ids.Contains(x.Id))
            .OrderBy(x=>x.CreatedAt)
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                MiniPcId = x.MiniPcs.Id,
                MiniPcs = x.MiniPcs,
                // IotId=x.IotId,
                IotStatus= x.IotStatus != null &&  x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!=null?new List<IotStatus>(){x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!}:new List<IotStatus>(),
                LastModifiedAt=x.LastModifiedAt,
                LastModifiedBy=x.LastModifiedBy,
                Name=x.Name,
                // Region=x.Region,
                Sensor=x.Sensor,
                // RegionId=x.RegionId,
                
                
            })
            .ToListAsync())
            .Where(x=>x.Sensor.Where(y=>y.DeletedAt==null).Count()>0)
            .Select(y => new MicroItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.MiniPcs.Region.Id,
                RegionName = y.MiniPcs.Region.Name,
                LandId=y.MiniPcs.Region.LandId,
                LandName=y.MiniPcs.Region.Land.Name,
                PlantId=y.MiniPcs.Region.Plant.Id,
                PlantName=y.MiniPcs.Region.Plant.Name,
                Status = y.IotStatus == null || y.IotStatus.Count()==0 ? false : y.IotStatus.OrderBy(x=>y.CreatedAt).LastOrDefault()!.IsActive
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MicroItemMinimalDto>> ShowMinimalMicro(int LandId){
            return (await this.context.Mikrokontrollers
            .Include( x=> x.MiniPcs).ThenInclude(x => x.Region)
            .Where(x=>x.MiniPcs.Region.LandId == LandId)
            .Where(x=>x.Sensor.Count() > 0)
            .ToListAsync()).Select(y => new MicroItemMinimalDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.MiniPcs.Region.Id,
                RegionName = y.MiniPcs.Region.Name
            }).ToList();
        }
        [HttpPost]
        public async Task<int> AddMicro([FromBody] AddMicroDto model){
            var AddMicro = await this.context.Mikrokontrollers.AddAsync(new Mikrokontroller{
                Name = model.Name,
                Description = model.Description,
                MiniPcId = model.MiniPcId
                // RegionId = model.RegionId
            });
            return await this.context.SaveChangesAsync();
        }
        [HttpGet("{LandId:int?}")]
        public async Task<MicrocontrollerSearchResponse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Mikrokontrollers
            .Include(x => x.MiniPcs).ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            // .Include(x => x.MiniPcs).ThenInclude(x=>x.Region).ThenInclude(x => x.Land)
            // .Include(x => x.Region).ThenInclude(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x => x.MiniPcs).ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Where(x=>LandId==-1? true: x.MiniPcs.Region.LandId==LandId)
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                MiniPcId = x.MiniPcId,
                MiniPcs = x.MiniPcs,
                Sensor = x.Sensor,
                // IotId=x.MiniPcs.IotId,
                IotStatus= x.IotStatus != null &&  x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!=null?new List<IotStatus>(){x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!}:new List<IotStatus>(),
                LastModifiedAt=x.LastModifiedAt,
                LastModifiedBy=x.LastModifiedBy,
                Name=x.Name
                // Region=x.Region,
                // RegionId=x.RegionId
            })
            .Where(x => x.Name.ToLower().Contains(query.Search));
            
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new MicroItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    MiniPcId = x.MiniPcId,
                    MiniPcName = x.MiniPcs.Name,
                    LandId=x.MiniPcs.Region.Land.Id,
                    LandName=x.MiniPcs.Region.Land.Name,
                    RegionId=x.MiniPcs.RegionId,
                    RegionName=x.MiniPcs.Region.Name,
                    PlantId = x.MiniPcs.Region.Plant.Id,
                    PlantName = x.MiniPcs.Region.Plant.Name,
                    // RegionName=x.Region.RegionPlant !=null && x.Region.RegionPlant.Count()>0? x.Region.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",//x.Region.Name,
                    Status= x.IotStatus == null || x.IotStatus.Count()==0 ? false : x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!.IsActive
                };
            }).ToList();
            return new MicrocontrollerSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpPut("{MicroId}")]
        public async Task UpdateMicro( int MicroId ,[FromBody] UpdateMicroDto model){
            var result = await this.context.Mikrokontrollers.FindAsync(MicroId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.MiniPcId = model.MiniPcId;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{MicroId}")]
        public async Task DeleteMicro(int MicroId)
        {
            var result = await this.context.Mikrokontrollers.FindAsync(MicroId);
            this.context.Mikrokontrollers.Remove(result!);
            await this.context.SaveChangesAsync();
        }
    }

    public class MicrosIdenity{
        public List<int>? Ids {get;set;}
    }
    public class AddMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int MiniPcId{get; set;}
        // public int RegionId{get; set;}

    }
    public class UpdateMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int MiniPcId{get; set;}
        // public int RegionId{get; set;}

    }
    public class MicroItemMinimalDto{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}
    }
    public class MicroItemDto{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int MiniPcId {get;set;}
        public string MiniPcName {get;set;}
        public int LandId {get; set;}
        public string LandName {get; set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}
        public int PlantId {get; set;}
        public string PlantName {get; set;}
        public bool Status {get;set;}
    }
    public class MicrocontrollerSearchResponse : SearchResponse<MicroItemDto>
    {

    }
}