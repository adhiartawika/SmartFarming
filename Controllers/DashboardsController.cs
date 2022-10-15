using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DashboardsController : ControllerBase
    {
        private readonly AppDbContext context;

        public DashboardsController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<DashboardDataDto>> OverviewMikroMini(int LandId){
            var obj_baru =( await this.context.Mikrokontrollers
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Include(x=>x.Sensor)
            .Where(x => x.MiniPc.Region.LandId == LandId)
            .OrderBy(x=> x.CreatedAt).ToListAsync())
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                MiniPcId = x.MiniPc.Id,
                MiniPc = x.MiniPc,
                IotStatus= x.IotStatus != null &&  x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!=null?new List<IotStatus>(){x.IotStatus.OrderBy(x=>x.CreatedAt).LastOrDefault()!}:new List<IotStatus>(),
                LastModifiedAt=x.LastModifiedAt,
                LastModifiedBy=x.LastModifiedBy,
                Name=x.Name,
                Sensor=x.Sensor,  
            }).ToList().Where(x=>x.Sensor.Where(y=>y.DeletedAt==null).Count()>0)
            .Select(y => new DashboardDataDto{
                LandName = y.MiniPc.Region.Land.Name,
                RegionName = y.MiniPc.Region.Name,
                MiniPcName = y.MiniPc.Name,
                MikroCount = y.MiniPc.Mikrokontrollers.Count(),
                SensorCount = y.Sensor.Count(),
                Status = y.IotStatus == null || y.IotStatus.Count()==0 ? false : y.IotStatus.OrderBy(x=>y.CreatedAt).LastOrDefault()!.IsActive
            });

            return obj_baru;
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<SensorParamViewDto>> SensorParamView(int LandId){
            // var data1 = this.context.Datas
            // .Include(x => x.Sensor).ThenInclude(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
            // .Include(x=>x.ParentParam).ThenInclude(x=>x.Plant)
            // .Include(x=>x.ParentParam).ThenInclude(x=>x.Plant).ThenInclude(x=>x.ParentParameters).ThenInclude(y => y.ParentTypes)
            // .ToList();
            // var roncom = data1.Select(x => new SensorParamViewDto{
            //     ParentTypeId = x.ParentParam.ParentTypesId,
            //     ParentTypeName = x.ParentParam.ParentTypes.Name,
            //     Values = x.ValueParameter
            // }).ToList().LastOrDefault();

            // Console.WriteLine(roncom);
            var datas = this.context.Sensors
            .Include(y => y.Datas)
            .Include(x => x.MikroController)
            .ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region)
            .ThenInclude(x => x.Plant)
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
