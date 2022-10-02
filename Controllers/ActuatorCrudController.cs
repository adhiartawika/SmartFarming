using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ActuatorCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public ActuatorCrudController(AppDbContext context){
            this.context = context;
        }
        [HttpGet]
        public IEnumerable<ActuatorType> GetActuatorType(){
            // return (await this.context.get)
            List<ActuatorType> types = new List<ActuatorType>();
            foreach(var j in this.context.TypeActuators){
                ActuatorType temp = new ActuatorType{
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
        public async Task<IEnumerable<ActuatorItemDto>> ShowActuator(){
            return (await this.context.Actuators.Include(x => x.ActuatorType)
            .Include(x => x.Status)
            .Include(x => x.MikroController).ToListAsync()).Select(y => new ActuatorItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MicroId = y.MikroController.Id,
                MicroName = y.MikroController.Name,
                TypeId = y.ActuatorTypeId,
                TypeName = y.ActuatorType.Name,
                StatusActuator = y.Status.Select( x => new StatusActuatorDto{
                    Status = x.Status
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<int> AddActuator([FromBody] AddActuatorDto model){
            List<ActuatorStatus> statuses = new List<ActuatorStatus>();

            for(int i = 0; i < model.StatusActuator.Count(); i++){
                statuses.Add( new ActuatorStatus
                {
                    Status = model.StatusActuator.ElementAt(i).Status
                });
            }
            var obj = await this.context.AddAsync(new Actuator{
                Name = model.Name,
                Description = model.Description,
                ActuatorTypeId = model.ActuatorTypeId,
                MikrocontrollerId = model.MicroId,
                Status = statuses
            });
            return await this.context.SaveChangesAsync();
        }
        [HttpGet("{LandId:int?}")]
        public async Task<ActuatorSearchRespsonse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Actuators.Where(x=>x.DeletedAt==null)
            .Include(x=> x.ActuatorType).Include(x => x.MikroController).ThenInclude(x => x.MiniPcs).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            .Where(x=>LandId==-1? true: x.MikroController.MiniPcs.Region.LandId==LandId)
            .Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new ActuatorItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    LandId=x.MikroController.MiniPcs.Region.LandId,
                    LandName=x.MikroController.MiniPcs.Region.Land.Name,
                    TypeId = x.ActuatorTypeId,
                    TypeName = x.ActuatorType.Name,
                    MicroId=x.MikroController.Id,
                    MicroName=x.MikroController.Name,
                    RegionId=x.MikroController.MiniPcs.RegionId,
                    RegionName=x.MikroController.MiniPcs.Region.Name
                };
            }).ToList();
            return new ActuatorSearchRespsonse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpPut("{ActuatorId}")]
        public async Task UpdateActuator(int ActuatorId,[FromBody] UpdateActuatorDto model){
            var result = await this.context.Actuators.FindAsync(ActuatorId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.ActuatorTypeId = model.ActuatorTypeId;
            result.MikrocontrollerId = model.MicroId;

            await this.context.SaveChangesAsync();
        }

        [HttpDelete("{ActuatorId}")]
        public async Task DeleteActuator(int ActuatorId){
            var result = await this.context.Actuators.FindAsync(ActuatorId);
            this.context.Actuators.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }
    public class ActuatorType{
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get; set;}
    }
    public class ActuatorItemDto{
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

        public virtual ICollection<StatusActuatorDto> StatusActuator {get;set;}
    }

    public class StatusActuatorDto{
        public bool Status{get;set;}
    }
    
    public class AddActuatorDto{

        public int Id {get;set;}
        public string Name {get;set;}
        public string Description{get;set;}
        public int ActuatorTypeId {get;set;}
        public int MicroId {get;set;}

        public List<StatusActuatorDto> StatusActuator {get;set;}
    }

    public class UpdateActuatorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public int ActuatorTypeId {get;set;}
        public int MicroId {get;set;}
        public bool Status {get;set;}
    }
    public class ActuatorSearchRespsonse : SearchResponse<ActuatorItemDto>
    {

    }
}