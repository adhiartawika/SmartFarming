using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Model.IdEntity;
using backend.Persistences;
using backend.Commons;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]

    public class MiniPcsCrudController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;
        private readonly IUtilityCurrentUserAces UserAcess;
        public MiniPcsCrudController(AppDbContext context, ICurrentUserService currentUser,IUtilityCurrentUserAces UserAcess){
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
        }
        [HttpGet]
        public async Task<IEnumerable<MiniPcItemDto>> ShowMiniPc(){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            return (await this.context.MiniPcs.Where(x=>x.DeletedAt==null).Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(x =>x.Region)
            .Include(x => x.Mikrokontrollers).ToListAsync()).Select(y => new MiniPcItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name,
                Code = y.Code,
                Secret = y.Secret,
                MicroItemDto = y.Mikrokontrollers.Select(x => new MicroControllerItemDto{
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MiniPcItem2DTO>> ShowMiniPcInALand(int LandId){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            return (await this.context.MiniPcs.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(x =>x.Region).ThenInclude(x=>x.Land)
            .Include(x=>x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus.OrderByDescending(y=>y.CreatedAt).Take(1))
            .Where(x => x.DeletedAt == null)
            .Where(x=>x.Region.LandId==LandId)
            .ToListAsync()).Select(y => new MiniPcItem2DTO{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name,
                LandId=y.Region.LandId,
                LandName=y.Region.Land.Name,
                PlantId=y.Region.Plant.Id,
                PlantName=y.Region.Plant.Name,
                Status = y.IotStatus.Count() > 0 ?y.IotStatus.FirstOrDefault()!.IsActive:false
            });
        }
        // [HttpPost]
        // public async Task<int> AddMiniPc([FromBody] AddMiniPcDto model){
        //     var obj = await this.context.AddAsync(new MiniPc{
        //         Name = model.Name,
        //         Description = model.Description,
        //         RegionId = model.RegionId,
                
        //     });
        //     return await this.context.SaveChangesAsync();
        // }
        [HttpPost]
        public async Task<int> AddMini([FromBody] CreateMiniDto model)
        {
            // reg.RegionPlant.Add(new RegionPlant{PlantId=model.PlantId});
            if(this.currentUser.RoleId != 3){
                var obj_iot = await this.context.IdentityIoTs.AddAsync(new IdIoT 
                    { 
                        Name = model.Name,
                        Code = model.Code,
                        Secret = model.Secret,
                        CreatedById = this.currentUser.UserId
                    });
                await this.context.SaveChangesAsync();
                
                var obj_baru = await this.context.MiniPcs.AddAsync(new MiniPc 
                { 
                    Name = model.Name, 
                    Description = model.Description, 
                    RegionId = model.RegionId,
                    IdentityId = obj_iot.Entity.Id,
                    Code = model.Code,
                    Secret = model.Secret,
                    CreatedById = this.currentUser.UserId
                });
                await this.context.SaveChangesAsync();
                return obj_baru.Entity.Id;
            }
            return 0;
            
        }
        [HttpPut("{MiniPcId}")]
        public async Task<IActionResult> UpdateMiniPc( int MiniPcId ,[FromBody] UpdateMiniPcDto model){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var CheckMiniPcId = this.context.MiniPcs.Where(x=>x.DeletedAt==null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.Id == MiniPcId).FirstOrDefault();
            Console.WriteLine(CheckMiniPcId);
            if(this.currentUser.RoleId != 3 && CheckMiniPcId != false){ 
                var result = await this.context.MiniPcs.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==MiniPcId);
                result.Name = model.Name;
                result.Description = model.Description;
                result.RegionId = model.RegionId;
                await this.context.SaveChangesAsync();
                return new OkObjectResult(new AppResponse { message="Sucess"});
            }
            return new BadRequestObjectResult(new AppResponse { message="Akses Tidak Ditemukan"});
        }
        [HttpDelete("{MiniPcId}")]
        public async Task<IActionResult> DeleteMiniPc(int MiniPcId)
        {
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var CheckMiniPcId = this.context.MiniPcs.Where(x=>x.DeletedAt==null).Where(x => arrayId.Contains(x.CreatedById.Value)).Select(x => x.Id == MiniPcId).FirstOrDefault();
            if(this.currentUser.RoleId != 3 && CheckMiniPcId != false){ 
                var result = await this.context.MiniPcs.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==MiniPcId);
                this.context.MiniPcs.Remove(result);
                await this.context.SaveChangesAsync();
                return new OkObjectResult(new AppResponse { message="Sucess"});
            }
            return new BadRequestObjectResult(new AppResponse { message="Akses Tidak Ditemukan"});
        }
        [HttpGet("{LandId:int?}")]
        public async Task<MiniPcSearchResponse> Search([FromQuery] SearchRequest query, int LandId = -1)
        {
            
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.MiniPcs.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(x => x.Mikrokontrollers)
            .Include(x => x.Region).ThenInclude(x=>x.Land)
            // .Include(x => x.MiniPcs).ThenInclude(x=>x.Region).ThenInclude(x => x.Land)
            // .Include(x => x.Region).ThenInclude(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Where(x => x.DeletedAt == null)
            .Where(x=>LandId==-1? true: x.Region.LandId==LandId)
            .Select(x=> new MiniPc{
                CreatedAt=x.CreatedAt,
                CreatedById=x.CreatedById,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                Code = x.Code,
                Secret = x.Secret,
                // IotId=x.MiniPcs.IotId,
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
                return new MiniPcItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description=x.Description,
                    RegionId=x.RegionId,
                    RegionName=x.Region.Name,
                    Code = x.Code,
                    Secret = x.Secret,
                    MikroCount = x.Mikrokontrollers.Count(),

                    // RegionName=x.Region.RegionPlant !=null && x.Region.RegionPlant.Count()>0? x.Region.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",//x.Region.Name,
                    Status= x.IotStatus == null || x.IotStatus.Count()==0 ? false : x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!.IsActive
                };
            }).ToList();
            return new MiniPcSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
    }
    public class MiniPcIdIotDto
    {

        public int IdIot {get;set;}
        public string Name { get; set; }
        public string Code { get; set; }
        public string Secret { get; set; }

        // ParentTypeId = PH
        // Desc = ['asam', 'optimal', 'mungkin optimal', 'basa']
        // values = [0,7,9,13]
        // values = [7,9,13,14]
    }
    public class CreateMiniDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RegionId {get;set;}
        public int IotId {get;set;}
        public string Code { get; set; }
        public string Secret { get; set; }

    }
    public class MicroControllerItemDto{
        public int Id {get;set;}
        public string Name {get;set;}
    }
    public class MiniPcItemDto{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public string Secret {get;set;}
        public string Code {get;set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}

        public int MikroCount {get;set;}
        public bool Status {get;set;}
        public List<MicroControllerItemDto> MicroItemDto {get;set;}

    }
    public class AddMiniPcDto{
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
        public int IotId {get; set;}
    }
    public class UpdateMiniPcDto{
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
    }
    public class MiniPcItem2DTO{
        public int Id {get;set;}
        public string Name {get;set;}
        public int RegionId {get;set;}
        public string RegionName {get;set;}
        public string Description {get;set;}
        public int LandId {get;set;}
        public string LandName {get;set;}
        public bool Status {get;set;}
        public int PlantId {get;set;}
        public string PlantName {get;set;}
    }
    public class MiniPcSearchResponse : SearchResponse<MiniPcItemDto>
    {

    }
}