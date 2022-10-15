using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            // return (await this.context.get)
            List<SensorType> types = new List<SensorType>();
            foreach(var j in this.context.ParentTypes){
                SensorType temp = new SensorType{
                    Id = j.Id,
                    Name = j.Name,
                    Description = j.Description
                };
                types.Add(temp);
            }
            // foreach (var Sensortypes in types.ge){
            // }
            // for (int i = 0; i < model.ParentTypes.Count(); i++)
            // foreach( Object ob in)
            // foreach(int i in Enum.GetValues(typeof(TypeSensor))) {  
            // //     SensorType  temp = new SensorType{
            // //         Id=i,
            // //         Name=Enum.GetName(typeof(TypeSensor), i)!.ToString()
            // //     };
            // //     types.Add(temp);

            // }  
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
            .Include(x=> x.ParentType).Include(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            .Where(x=>LandId==-1? true: x.MikroController.MiniPc.Region.LandId==LandId)
            .Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new SensorItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    LandId=x.MikroController.MiniPc.Region.LandId,
                    LandName=x.MikroController.MiniPc.Region.Land.Name,
                    TypeId = x.ParentTypeId,
                    TypeName = x.ParentType.Name,
                    MicroId=x.MikroController.Id,
                    MicroName=x.MikroController.Name,
                    RegionId=x.MikroController.MiniPc.RegionId,
                    RegionName=x.MikroController.MiniPc.Region.Name
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
                MikrocontrollerId = model.MicroId,
                ParentTypeId = model.ParentTypeId,
                ParameterId = model.ParameterId
            });
            return await this.context.SaveChangesAsync();
        }

        // [HttpPost]
        // public async Task<int> AddSensors([FromBody] AddSensorDto model)
        // {
        //     List<ParentType> parenttype = new List<ParentType>();
            
        //     for (int i = 0; i < model.ParentTypes.Count(); i++)
        //     {
        //         parenttype.Add(new ParentType
        //         {
        //             Name = model.ParentTypeId.ElementAt(i).Name,
        //             Description = model.ParentTypes.ElementAt(i).Description
        //         });
        //     }
        //     var obj_baru = await this.context.AddAsync(new Sensor{
        //         Name = model.Name,
        //         Description = model.Description,
        //         MikrocontrollerId = model.MicroId,
        //         ParameterId = model.ParameterId,
        //         ParentTypes = parenttype
        //     });
        //     // var obj_baru = await this.context.Sensors.AddAsync(new Sensor { Name = model.Name, LatinName = model.LatinName, Description = model.Description, Parameters = parameters });
        //     await this.context.SaveChangesAsync();
        //     return obj_baru.Entity.Id;
        // }
        [HttpPost]
        public async Task Disconnect([FromBody] SensorConnection model){
             this.context.IotStatus.Add(
                        new IotStatus { 
                            MicroControllerId=model.Id,       
                            MiniPc=null,
                            IsActive=false,
                            CreatedAt= DateTime.Now
                        });
            await this.context.SaveChangesAsync();
             
        }
        [HttpPost]
        public async Task Connect([FromBody] SensorConnection model){
            this.context.IotStatus.Add(
                        new IotStatus { 
                            MicroControllerId=model.Id,       
                            MiniPc=null,
                            IsActive=true,
                            CreatedAt= DateTime.Now
                        });
            await this.context.SaveChangesAsync();
        }

        [HttpPut("{SensorId}")]
        public async Task UpdateSensor(int SensorId,[FromBody] UpdateSensorDto model){
            var result = await this.context.Sensors.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==SensorId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.ParentTypeId = model.ParentTypeId;
            result.MikrocontrollerId = model.MicroId;

            await this.context.SaveChangesAsync();
        }

        [HttpDelete("{SensorId}")]
        public async Task DeleteSensor(int SensorId){
            var result = await this.context.Sensors.FirstOrDefaultAsync(x=>x.Id==SensorId);
            this.context.Sensors.Remove(result!);
            await this.context.SaveChangesAsync();
        }
    }
    public class SensorType{
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get; set;}
    }
    public class SensorMinimaItemDto{
        public int Id {get;set;}
        public string Name {get;set;}
        public int MicroId {get;set;}
        public string MicroName {get;set;}
    }
    public class SensorItemDto{
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get;set;}
        public int TypeId {get;set;} 
        public string TypeName {get;set;} 
        public int MicroId {get;set;}
        public string MicroName {get; set;}
        public int RegionId {get;set;}
        public string RegionName {get; set;}
        public int LandId {get;set;}
        public string LandName {get; set;}
    }

    public class SensorConnection{
        public int Id{get;set;}
    }
    public class AddSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public int ParentTypeId {get;set;}
        public int MicroId {get;set;}
        public int ParameterId {get;set;}
        // public List<SensorType> SensorTypes { get; set; }
    }
    public class UpdateSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public int ParentTypeId {get;set;}
        public int MicroId {get;set;}
    }
    public class SensorSearchResponse : SearchResponse<SensorItemDto>
    {

    }
}