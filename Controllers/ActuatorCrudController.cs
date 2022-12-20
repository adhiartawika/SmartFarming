using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class ActuatorCrudController:ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;

        private readonly IUtilityCurrentUserAces UserAcess;
        public ActuatorCrudController(AppDbContext context,ICurrentUserService currentUser,IUtilityCurrentUserAces UserAcess){
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
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
            return types;
        }
        // [HttpGet]
        // public async Task<IEnumerable<ActuatorItemDto>> ShowActuator(){
        //     return (await this.context.Actuators.Include(x => x.ActuatorType)
        //     .Include(x => x.Status)
        //     .Include(x => x.MikroController).ToListAsync()).Select(y => new ActuatorItemDto{
        //         Id = y.Id,
        //         Name = y.Name,
        //         Description = y.Description,
        //         MicroId = y.MikroController.Id,
        //         MicroName = y.MikroController.Name,
        //         TypeId = y.ActuatorTypeId,
        //         TypeName = y.ActuatorType.Name,
        //         StatusActuator = y.Status.Select( x => new StatusActuatorDto{
        //             Status = x.Status
        //         }).ToList()
        //     });
        // }

        [HttpPost]
        public async Task<int> AddActuator([FromBody] AddActuatorDto model){
            List<ActuatorStatus> statuses = new List<ActuatorStatus>();
            if(this.currentUser.RoleId != 3){
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
                    Status = statuses,
                    CreatedById = this.currentUser.UserId
                });
                return await this.context.SaveChangesAsync();
            }
            return 0;

        }
        [HttpGet("{LandId:int?}")]
        public async Task<ActuatorSearchRespsonse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Actuators.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false).Where(x=>x.DeletedAt==null)
            .Include(x => x.Status)
            .Include(x=> x.ActuatorType).Include(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            .Where(x=>LandId==-1? true: x.MikroController.MiniPc.Region.LandId==LandId)
            .Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new ActuatorItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    LandId=x.MikroController.MiniPc.Region.LandId,
                    LandName=x.MikroController.MiniPc.Region.Land.Name,
                    TypeId = x.ActuatorTypeId,
                    TypeName = x.ActuatorType.Name,
                    MicroId=x.MikroController.Id,
                    MicroName=x.MikroController.Name,
                    RegionId=x.MikroController.MiniPc.RegionId,
                    RegionName=x.MikroController.MiniPc.Region.Name,
                    StatusActuator=x.Status.Select(x => x.Status).First()
                };
            }).ToList();
            return new ActuatorSearchRespsonse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpPut("{ActuatorId}")]
        public async Task<IActionResult> UpdateActuator(int ActuatorId,[FromBody] UpdateActuatorDto model){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var CheckActuatorId = this.context.Actuators.Where(x=>x.DeletedAt==null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.Id == ActuatorId).FirstOrDefault();
            if(this.currentUser.RoleId != 3 && CheckActuatorId != false){
                var result = await this.context.Actuators.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==ActuatorId);
                result.Name = model.Name;
                result.Description = model.Description;
                result.ActuatorTypeId = model.ActuatorTypeId;
                result.MikrocontrollerId = model.MicroId;

                await this.context.SaveChangesAsync();
                return new OkObjectResult(new AppResponse { message="Actuator Berhasil Terubah"});
            }
            return new BadRequestObjectResult(new AppResponse { message="Role Tidak Dizinkan"});
        }

        [HttpDelete("{ActuatorId}")]
        public async Task<IActionResult> DeleteActuator(int ActuatorId){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var CheckActuatorId = this.context.Actuators.Where(x=>x.DeletedAt==null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.Id == ActuatorId).FirstOrDefault();
            if(this.currentUser.RoleId != 3 && CheckActuatorId != false){
                var result = await this.context.Actuators.Where(x => x.DeletedAt == null).FirstOrDefaultAsync(x=>x.Id==ActuatorId);
                this.context.Actuators.Remove(result);
                await this.context.SaveChangesAsync();
                return new OkObjectResult(new AppResponse { message="Actuator Berhasil Terhapus"});
            }
            return new BadRequestObjectResult(new AppResponse { message="Role Tidak Dizinkan"});
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
        public bool StatusActuator {get;set;}
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