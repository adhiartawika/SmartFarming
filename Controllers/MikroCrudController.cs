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
            return (await this.context.Mikrokontrollers.Include(x =>x.Region).ToListAsync()).Select(y => new MicroItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name
            });
        }
        
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MicroItemMinimalDto>> ShowMinimalMicro(int LandId){
            return (await this.context.Mikrokontrollers.Include(x =>x.Region).Where(x=>x.Region.LandId == LandId).ToListAsync()).Select(y => new MicroItemMinimalDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name
            }).ToList();
        }
        [HttpPost]
        public async Task<int> AddMicro([FromBody] AddMicroDto model){
            var AddMicro = await this.context.Mikrokontrollers.AddAsync(new Mikrokontroller{
                Name = model.Name,
                Description = model.Description,
                RegionId = model.RegionId
            });
            return await this.context.SaveChangesAsync();
        }
        [HttpGet("{LandId:int?}")]
        public async Task<MicrocontrollerSearchResponse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Mikrokontrollers
            .Include(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.Region).ThenInclude(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Where(x=>LandId==-1? true: x.Region.LandId==LandId)
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                IotId=x.IotId,
                IotStatus= x.IotStatus != null &&  x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!=null?new List<IotStatus>(){x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!}:new List<IotStatus>(),
                LastModifiedAt=x.LastModifiedAt,
                LastModifiedBy=x.LastModifiedBy,
                Name=x.Name,
                Region=x.Region,
                RegionId=x.RegionId
            })
            .Where(x => x.Name.ToLower().Contains(query.Search));
            
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new MicroItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    LandId=x.Region.Land.Id,
                    LandName=x.Region.Land.Name,
                    RegionId=x.RegionId,
                    RegionName=x.Region.RegionPlant !=null && x.Region.RegionPlant.Count()>0? x.Region.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",//x.Region.Name,
                    Status=x.IotStatus == null || x.IotStatus.Count()==0 ? false : x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!.IsActive
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
            result.RegionId = model.RegionId;
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

    public class AddMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int RegionId{get; set;}

    }
    public class UpdateMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int RegionId{get; set;}

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
        public int LandId {get; set;}
        public string LandName {get; set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}
        public bool Status {get;set;}
    }
    public class MicrocontrollerSearchResponse : SearchResponse<MicroItemDto>
    {

    }
}