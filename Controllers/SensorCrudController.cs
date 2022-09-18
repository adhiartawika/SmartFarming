using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SensorCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public SensorCrudController(AppDbContext context){
            this.context = context;
        }

        [HttpGet]
        public IEnumerable<SensorType> GetSensorTypes(){
            List<SensorType> types = new List<SensorType>();
            foreach(int i in Enum.GetValues(typeof(TypeSensor))) {  
                SensorType  temp = new SensorType{
                    Id=i,
                    Name=Enum.GetName(typeof(TypeSensor), i)!.ToString()
                };
                types.Add(temp);

            }  
            return types;
        }

        [HttpGet]
        public async Task<IEnumerable<SensorItemDto>> ShowSensor(){
            return (await this.context.Sensors.Where(x=>x.DeletedAt==null).Include(x => x.MikroController).ToListAsync()).Select(y => new SensorItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MicroId = y.MikroController.Id,
                MicroName = y.MikroController.Name
            });
        }
        [HttpGet("{LandId:int?}")]
        public async Task<SensorSearchResponse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Sensors.Where(x=>x.DeletedAt==null)
            .Include(x => x.MikroController).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            .Where(x=>LandId==-1? true: x.MikroController.Region.LandId==LandId)
            .Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new SensorItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    LandId=x.MikroController.Region.LandId,
                    LandName=x.MikroController.Region.Land.Name,
                    MicroId=x.MikrocontrollerId,
                    MicroName=x.MikroController.Name,
                    RegionId=x.MikroController.RegionId,
                    RegionName=x.MikroController.Region.Name
                };
            }).ToList();
            return new SensorSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpPost]
        public async Task<int> AddSensor([FromBody] AddSensorDto model){
            var obj = await this.context.AddAsync(new Sensor{
                Name = model.Name,
                Description = model.Description,
                Type = model.type,
                MikrocontrollerId = model.MicroId
            });
            return await this.context.SaveChangesAsync();
        }

        [HttpPut("{SensorId}")]
        public async Task UpdateSensor(int SensorId,[FromBody] UpdateSensorDto model){
            var result = await this.context.Sensors.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==SensorId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.Type = model.type;
            result.MikrocontrollerId = model.MicroId;

            await this.context.SaveChangesAsync();
        }

        [HttpDelete("{SensorId}")]
        public async Task DeleteSensor(int SensorId){
            var result = await this.context.Sensors.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==SensorId);
            this.context.Sensors.Remove(result!);
            await this.context.SaveChangesAsync();
        }
    }
    public class SensorType{
         public int Id {get;set;}
        public string Name {get; set;}
    }
    public class SensorItemDto{
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get;set;}
        public int Type {get;set;} 
        public int MicroId {get;set;}
        public string MicroName {get; set;}
        public int RegionId {get;set;}
        public string RegionName {get; set;}
        public int LandId {get;set;}
        public string LandName {get; set;}
    }

    public class AddSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public TypeSensor type {get;set;} 
        public int MicroId {get;set;}

    }

    public class UpdateSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public TypeSensor type {get;set;} 
        public int MicroId {get;set;}
    }
    public class SensorSearchResponse : SearchResponse<SensorItemDto>
    {

    }
}