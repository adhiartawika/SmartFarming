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
        //inject icurrentuser

        public MikroCrudController(AppDbContext context){

            this.context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<MicroItemDto>> ShowMicro(){
            return (await this.context.Mikrokontrollers
            // .Where(x => IUrlHelper.ro==9?true:IUrlHelper.8?x.createdBy = Iuser.userId : )
            .Include(x => x.MiniPc).ThenInclude(x =>x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPc).ThenInclude(x =>x.Region).ThenInclude(x=>x.Plant)
            .ToListAsync()).Select(y => new MicroItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MiniPcId = y.MiniPc.Id,
                MiniPcName = y.MiniPc.Name,
                RegionId = y.MiniPc.Region.Id,
                RegionName = y.MiniPc.Region.Name,
                LandId=y.MiniPc.Region.LandId,
                LandName=y.MiniPc.Region.Land.Name,
                PlantId=y.MiniPc.Region.PlantId,
                PlantName=y.MiniPc.Region.Plant.Name
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MicroItemDto>> ShowOverviewMicro(int LandId, [FromQuery] MicrosIdenity model){
            return (await this.context.Mikrokontrollers
            .Include(x => x.MiniPc)
            .ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            .Include(x => x.MiniPc)
            .ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Include(x=>x.Sensor)
            .Where(x=>LandId==-1? true: x.MiniPc.Region.LandId==LandId)
            .Where(x=>model.Ids == null ? false:model.Ids.Contains(x.Id))
            .OrderBy(x=>x.CreatedAt)
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                MiniPcId = x.MiniPc.Id,
                MiniPc = x.MiniPc,
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
                RegionId = y.MiniPc.Region.Id,
                RegionName = y.MiniPc.Region.Name,
                LandId=y.MiniPc.Region.LandId,
                LandName=y.MiniPc.Region.Land.Name,
                PlantId=y.MiniPc.Region.Plant.Id,
                PlantName=y.MiniPc.Region.Plant.Name,
                Status = y.IotStatus == null || y.IotStatus.Count()==0 ? false : y.IotStatus.OrderBy(x=>y.CreatedAt).LastOrDefault()!.IsActive
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<MicroItemMinimalDto>> ShowMinimalMicro(int LandId){
            return (await this.context.Mikrokontrollers
            .Include( x=> x.MiniPc).ThenInclude(x => x.Region)
            .Where(x=>x.MiniPc.Region.LandId == LandId)
            .Where(x=>x.Sensor.Count() > 0)
            .ToListAsync()).Select(y => new MicroItemMinimalDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.MiniPc.Region.Id,
                RegionName = y.MiniPc.Region.Name
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
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Land)
            // .Include(x => x.MiniPc).ThenInclude(x=>x.Region).ThenInclude(x => x.Land)
            // .Include(x => x.Region).ThenInclude(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x=>x.Plant)
            .Include(x=>x.IotStatus)
            .Where(x=>LandId==-1? true: x.MiniPc.Region.LandId==LandId)
            .Select(x=> new Mikrokontroller{
                CreatedAt=x.CreatedAt,
                CreatedBy=x.CreatedBy,
                DeletedAt=x.DeletedAt,
                DeletedBy=x.DeletedBy,
                Description=x.Description,
                Id=x.Id,
                MiniPcId = x.MiniPcId,
                MiniPc = x.MiniPc,
                Sensor = x.Sensor,
                // IotId=x.MiniPc.IotId,
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
                    MiniPcName = x.MiniPc.Name,
                    LandId=x.MiniPc.Region.Land.Id,
                    LandName=x.MiniPc.Region.Land.Name,
                    RegionId=x.MiniPc.RegionId,
                    RegionName=x.MiniPc.Region.Name,
                    PlantId = x.MiniPc.Region.Plant.Id,
                    PlantName = x.MiniPc.Region.Plant.Name,
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
        [HttpGet]
        public IEnumerable<MikroNameDto>GetMikroName(){
            // return (await this.context.get)
            List<MikroNameDto> types = new List<MikroNameDto>();
            foreach(var j in this.context.Mikrokontrollers){
                MikroNameDto temp = new MikroNameDto{
                    Id = j.Id,
                    Name = j.Name,
                };
                types.Add(temp);
            }
            return types;
        }
    [HttpGet("{RegionId}")]
    public async Task<IEnumerable<GraphDataParameterDto>> ShowSensorParameterWithRegion(int RegionId, [FromQuery]SensorParamRegionOverv model){
        // var obj_baru = this.context.Sensors
        // .Include(x => x.MikroController).ThenInclude(x => x.MiniPc)
        // .Include(x => x.ParentTypes)
        // .Where(x => model.MicroIds.Contains(x.MikroController.Id))
        // .Where(x => model.ParentTypesIds.Contains(x.ParentTypeId)).Select(x => x.Id).ToList();
        // var M = this.context.Mikrokontrollers
        //         .Include(x => x.Sensor).ThenInclude(x => x.ParentTypes)
        //         .Where(x => model.MicroIds.Contains(x.Id)).GroupBy(x =>)


        // var datas = this.context.Datas
        // .Include(x => x.Sensor).ThenInclude(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
        // .Where(x => obj_baru.Contains(x.Sensor.Id))
        // .Where(x => x.CreatedAt.Year == model.ParamDates.Year && 
        // (model.ParamDates.Month == x.CreatedAt.Month) && (model.ParamDates.Day == x.CreatedAt.Day))
        // .GroupBy(
        //     x =>x.Sensor.Id,
        //     x =>x,
        //     (key, g) => new { SensorId = key, Datas = g.ToList() }
        // )
        // .ToList(); 
        var datas = this.context.Sensors
        .Include(y => y.Datas.Where(x => x.CreatedAt.Year == model.ParamDates.Year && 
        (model.ParamDates.Month == x.CreatedAt.Month) && (model.ParamDates.Day == x.CreatedAt.Day)))
        .Include(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x => x.Plant)
        .Where(x => model.MicroIds.Contains(x.MikroController.Id))
        .Where(x => model.ParentTypesIds.Contains(x.ParentTypeId))
        .Select(x => new GraphDataParameterDto{
            MicroId = x.MikroController.Id,
                MicroName = x.MikroController.Name,
                ParentTypeId = x.ParentTypeId,
                ParentTypeName = x.ParentType.Name,
            Values = x.Datas.Select(y => new GraphDataDto{
                CreatedAt = y.CreatedAt,
                    Value = y.ValueParameter
            }).ToList()
        } ).ToList();
        Console.WriteLine(datas.Count());
        // var res = new List<GraphDataParameterDto>();
        // if(datas.Count() > 0){
        // foreach(var obj in datas){
        //     var temp = this.context.Sensors.Include(x => x.MikroController)
        //     .Include(x => x.ParentTypes)
        //     .Where(x => x.Id == obj.SensorId).FirstOrDefault();
        //     res.Add(new GraphDataParameterDto{
        //         MicroId = temp.MikroController.Id,
        //         MicroName = temp.MikroController.Name,
        //         ParentTypeId = temp.ParentTypeId,
        //         ParentTypeName = temp.ParentTypes.Name,
        //         Values = obj.Datas.Select(x => new GraphDataDto{
        //             CreatedAt = x.CreatedAt,
        //             Value = x.ValueParameter
        //         }).ToList()
        //     });
        //     }            
        // }
        // else{
        //     foreach(var obj in obj_baru){
        //     var temp = this.context.Sensors.Include(x => x.MikroController)
        //     .Include(x => x.ParentTypes)
        //     .Where(x => x.Id == obj).FirstOrDefault();
        //     res.Add(new GraphDataParameterDto{
        //         MicroId = temp.MikroController.Id,
        //         MicroName = temp.MikroController.Name,
        //         ParentTypeId = temp.ParentTypeId,
        //         ParentTypeName = temp.ParentTypes.Name,
        //         Values = new List<GraphDataDto>()
        //     });
        //     }           
        // }
       
        // var results = this.context.Sensors.
        // Include(x => x.d).GroupBy(
        // p => p.Id, 
        // p => p.data,
        // (key, g) => new { PersonId = key, Cars = g.ToList() });
        return datas;
    }
    }
    public class MikroNameDto{
        public int Id {get;set;}
        public string Name {get; set;}
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
        public int MiniPcId {get; set;}
        public string MiniPcName {get;set;}
        public decimal value {get;set;}
        public DateTime ParamDates {get;set;}

        //NamaSensor, Id Mikro, Nama Mikro, Value,Date
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
    public class SensorParamRegionOverv{
        public List<int>? MicroIds {get;set;}
        public List<int>? ParentTypesIds {get;set;}
        public DateTime ParamDates {get;set;}
    }
    public class GraphDataDto {
        public decimal Value {get;set;}
        public DateTime CreatedAt {get;set;}
    }

    public class GraphDataParameterDto
    {
    public int MicroId {get;set;}
    public string MicroName {get;set;}
    public int ParentTypeId {get;set;}
    public string ParentTypeName {get;set;}
    public List<GraphDataDto> Values {get;set;}
    }
    public class MicrocontrollerSearchResponse : SearchResponse<MicroItemDto>
    {

    }
}