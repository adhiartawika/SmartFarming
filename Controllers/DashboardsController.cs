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
        public async Task<IEnumerable<DashboardDataDto>> OverviewMikroMini(){
            var obj_baru =( await this.context.Mikrokontrollers
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Include(x=>x.Sensor)
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
                RegioName = y.MiniPc.Region.Name,
                MikroName = y.MiniPc.Name,
                SensorName = y.Name,
                Status = y.IotStatus == null || y.IotStatus.Count()==0 ? false : y.IotStatus.OrderBy(x=>y.CreatedAt).LastOrDefault()!.IsActive
            });

            return obj_baru;
        }
        [HttpGet]
        public async Task<IEnumerable<SensorParamViewDto>> SensorParamView(){
    
            // var data1 = this.context.Sensors
            // .Include(y => y.Datas)
            // .Include(x => x.MikroController).ThenInclude(x => x.MiniPc)
            // .ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
            // .Select(x => new SensorParamViewDto{
            //     ParentTypeId = x.Parameters.ParentParameters.ParentType.Id,
            //     ParentTypeName = x.Parameters.ParentParameters.ParentType.Name,
            //     Values = x.Datas.OrderBy(x => x.ValueParameter).Last()
            // return data1;
            var sensor = this.context.Sensors
            .Include(y => y.Datas)
            .Include(x => x.MikroController).ThenInclude(x => x.MiniPc)
            .ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
            .Select(x => x.Id).ToList();

            var data1 = this.context.Datas
            .Include(x => x.Sensor).ThenInclude(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
            .Include(x=>x.ParentParam).ThenInclude(x=>x.Plant)
            .Include(x=>x.ParentParam).ThenInclude(x=>x.Plant).ThenInclude(x=>x.ParentParameters).ThenInclude(y => y.ParentTypes)
            .ToList();
            var roncom = data1.Select(x => new SensorParamViewDto{
                ParentTypeId = x.ParentParam.ParentTypesId,
                ParentTypeName = x.ParentParam.ParentTypes.Name,
                Values = x.ValueParameter
            }).ToList();
            
            return roncom;
        }
    }
    // <IEnumerable<DashboardDataDto>>
    public class DashboardDataDto{
        public string LandName {get;set;}
        public string RegioName {get;set;}
        public string MikroName {get;set;}
        public string SensorName {get;set;}
        public bool Status {get;set;}
    }
    // public class SensorValueLast {
    //     public string DeskripsiParameter {get;set;}
    //     public decimal Value {get;set;}
    // }
    public class SensorParamViewDto{
        public int ParentTypeId {get;set;}
        public string ParentTypeName {get;set;}
        public decimal Values {get;set;}
        // public List<SensorValueLast> Values {get;set;}
    }
    public class DashboardSearchResponse : SearchResponse<SensorItemDto>
    {

    }
}
