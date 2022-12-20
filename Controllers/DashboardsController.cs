using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
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
    public class DashboardsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;
        private readonly IUtilityCurrentUserAces UserAcess;
        public DashboardsController(AppDbContext context, ICurrentUserService currentUser,IUtilityCurrentUserAces UserAcess)
        {
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<DashboardDataDto>> OverviewMikroMini(int LandId){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var obj_baru =( await this.context.MiniPcs.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Include(x => x.Mikrokontrollers).ThenInclude(x=>x.Sensor)
            .Where(x => x.DeletedAt == null)
            .Where(x => x.Region.LandId == LandId)
            .OrderBy(x=> x.CreatedAt).ToListAsync())
            .Select(y => new DashboardDataDto{
                LandName = y.Region.Land.Name,
                RegionName = y.Region.Name,
                MiniPcName = y.Name,
                MikroCount = y.Mikrokontrollers.Count(),
                SensorCount = y.Mikrokontrollers.SelectMany(x => x.Sensor).Count(),
                Status = y.IotStatus == null || y.IotStatus.Count()==0 ? false : y.IotStatus.OrderBy(y=>y.CreatedAt).LastOrDefault()!.IsActive
            });

            return obj_baru;
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<SensorParamViewDto>> SensorParamView(int LandId){
            var arrayId = this.context.Users.Where(x => x.institutedId == this.UserAcess.instId).Select(x => x.Id).ToList();
            var datas = this.context.Sensors.Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.CreatedById.Value): this.currentUser.RoleId == 3 ? arrayId.Contains(x.CreatedById.Value):false)
            .Include(y => y.Datas)
            .Include(x => x.MikroController)
            .ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region)
            .ThenInclude(x => x.Plant)
            .Where(x => x.DeletedAt == null)
            .Where( x => x.MikroController.MiniPc.Region.LandId == LandId)
            .Select(x => new SensorParamViewDto{
                SensorId = x.Id,
                ParentTypeId = x.ParentType.Id,
                ParentTypeName = x.ParentType.Name,
                Values = new List<SensorValueLast>()
            } ).ToList();

            for (int i = 0; i < datas.Count(); i++){
                var t = this.context.Datas.Where( x => x.SensorId == datas.ElementAt(i).SensorId )
                .OrderBy(x => x.CreatedAt).Select(x=>x.ValueParameter).LastOrDefault();    
                var k = new List<SensorValueLast>();
                var ttt = new SensorValueLast{
                    Value = t
                };
                Console.WriteLine(ttt);
                datas.ElementAt(i).Values.Add(ttt);          
            }
            return datas;
        }
    }
    // <IEnumerable<DashboardDataDto>>
    public class DashboardDataDto{
        public int LandId {get;set;}
        public string LandName {get;set;}
        public string RegionName {get;set;}
        public string MiniPcName {get;set;}
        public int MikroCount {get;set;}
        public int SensorCount {get;set;}
        public bool Status {get;set;}
    }
    public class SensorValueLast {
        public decimal Value {get;set;}
    }
    public class SensorParamViewDto{
        public int SensorId {get;set;}
        public int ParentTypeId {get;set;}
        public string ParentTypeName {get;set;}
        public string color {get;set;}
        // public decimal Values {get;set;}
        public List<SensorValueLast> Values {get;set;}
    }
    public class DashboardSearchResponse : SearchResponse<SensorItemDto>
    {

    }
}
