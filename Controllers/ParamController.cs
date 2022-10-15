using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ParamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParamController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ParentParamItemDto>> ShowParentParams(){
            return (await _context.ParentTypes.ToListAsync()).Select(x => new ParentParamItemDto{
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        [HttpGet("{LandId}")]
        public async Task<IEnumerable<int>> ShowMinimalParam(int LandId){
            var temp = this._context.Regions.Include(x=>x.Plant).Where(x=>x.LandId==LandId).Select(x=>x.PlantId).ToList();
            return (await this._context.Parameters
            .Include(x =>x.ParentParam).ThenInclude(x=>x.Plant).ThenInclude(x=>x.Regions)
            // .ThenInclude(x=>x.RegionPlants).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            // .Where(x=>x.Plant.RegionPlants.Any(y=>y.Region.LandId ==LandId))
            .Where(x=>temp.Contains(x.ParentParam.PlantId))
            .ToListAsync()).Select(y => y.ParentParam.ParentTypesId)
            .Distinct()
            .ToList(); //mengirim id 
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<ParamOverview>> ShowParamOverview(int LandId, [FromQuery]ParamOverv query){
            var listplantid = this._context.Regions.Include(x=>x.Plant).Where(x=>x.LandId==LandId).Select(x=>x.PlantId).ToList();
            var parametersnameofplant= (await this._context.Parameters
            .Include(x => x.ParentParam).ThenInclude(x =>x.Plant).ThenInclude(x=>x.Regions)
            .Include(x => x.ParentParam).ThenInclude(x => x.ParentTypes)
            // .ThenInclude(x=>x.RegionPlants).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            // .Where(x=>x.Plant.RegionPlants.Any(y=>y.Region.LandId ==LandId))
            .Where(x=>listplantid.Contains(x.ParentParam.Plant.Id))
            .ToListAsync()).Select(y =>y.ParentParam.ParentTypes.Name.Trim()).ToList();
            var datas = this._context.Datas
            // .ThenInclude(x=>x.ParentParameters).ThenInclude(y => y.Parameters)
                            .Include(x=>x.Sensor).ThenInclude(x=>x.MikroController)
                            .Include(x => x.ParentParam).ThenInclude(x=>x.Plant)
                            .Include(x => x.ParentParam).ThenInclude(x=>x.Plant).ThenInclude(x=>x.ParentParameters).ThenInclude(y => y.ParentTypes)
                            .Where(x=>parametersnameofplant.Contains(x.ParentParam.ParentTypes.Name.Trim()))
                            .Where(x=>query.Ids == null ? false : query.Ids.Contains(x.Sensor.MikroController.Id))
                            .Where(x=>query.GNames==null? false: query.GNames.Contains(x.ParentParam.ParentTypes.Name.Trim()))
                            .GroupBy(x=>x.ParentParamId)
                            .Select(g=>g.OrderBy(f=>f.CreatedAt).Last())
                            .ToList();
                            // .GroupBy(x=>x.ParameterId, (key,g)=>g.OrderBy(v=>v.CreatedAt).Last());
                            // .Select(x=>new Data{
                            //     CreatedAt=x.CreatedAt,
                            //     Id=x.Id,
                            //     Parameter=x.Parameter,
                            //     ParameterId=x.ParameterId,
                            //     Sensor=x.Sensor,
                            //     SensorId=x.SensorId,
                            //     ValueParameter=x.ValueParameter
                            // })
                            // .ToList();
         var rescandidate = datas.Select(x=> new ParamOverview{
                               GroupName=x.ParentParam.ParentTypesId,
                               PlantName=x.ParentParam.Plant.Name,
                               PlantId=x.ParentParam.PlantId,
                               MicroId=x.Sensor.MikroController.Id,
                               Value=x.ValueParameter,
                               Descriptions=new List<DescriptionReadParameterPlantDto>()
                            }).ToList();
            
            for (int i = 0; i < rescandidate.Count(); i++)
            {
                var t = datas.Where(x=>
                            x.ParentParam.PlantId == rescandidate.ElementAt(i).PlantId && 
                            rescandidate.ElementAt(i).MicroId == x.Sensor.MikrocontrollerId
                        ).OrderBy(x=>x.CreatedAt).Select(x=>x.ParentParam.Parameters).LastOrDefault();
                var k = new List<DescriptionReadParameterPlantDto>();
                for (int j = 0; j < t?.Count(); j++)
                {
                    var jj = t.ElementAt(j);
                     var ttt = new DescriptionReadParameterPlantDto{
                        Color=jj.Color,
                        Description=jj.Description,
                        Id=jj.Id,
                        MaxValue=jj.MaxValue,
                        MinValue=jj.MinValue
                    };
                    if( rescandidate.ElementAt(i).GroupName == t.ElementAt(j).ParentParam.ParentTypesId){ //sensor.name bisa diganti type jika groupname juga dari type
                        rescandidate.ElementAt(i).Descriptions.Add(ttt);
                    }
                }
            }
            return rescandidate.Where(x=>x.Descriptions.Count()>0);
        }

        [HttpPost]
        public async Task<CreateParentParamResponse> CreateParam([FromBody] CreateParameter model)
        {
            List<int> res = new List<int>();

            List<Parameter> objs = new List<Parameter>();
            for (int i = 0; i < model.Descriptions.Count(); i++)
            {
                objs.Add(new Parameter{

                    Color=model.Descriptions.ElementAt(i).Color,
                    Description=model.Descriptions.ElementAt(i).Description,
                    MaxValue=model.Descriptions.ElementAt(i).MaxValue,
                    MinValue=model.Descriptions.ElementAt(i).MinValue,
                });
            }
            ParentParameter p = new ParentParameter{
                ParentTypesId=model.GroupName,
                Parameters=objs
            };

            await _context.ParentParameters.AddAsync(p);
            await _context.SaveChangesAsync();
            for (int i = 0; i < objs.Count(); i++)
            {
                res.Add(objs.ElementAt(i).Id);
            }

            return new CreateParentParamResponse{
                Id=p.Id,
                ParamIds=res
            };
        }
        [HttpPost]
        public async Task<int> CreateDescriptionParam([FromBody] CreateDescriptionParameter model)
        {
            var obj_baru = await _context.Parameters.AddAsync(
                new Parameter{
                    Color=model.Color,
                    Description=model.Description,
                    ParentParamId=model.ParentParamId,
                    MaxValue=model.MaxValue,
                    MinValue=model.MinValue,
                }
            );
            await _context.SaveChangesAsync();
            return obj_baru.Entity.Id;
        }

        [HttpPut("{id}")]
        public async Task UpdateParam(int Id , [FromBody] UpdateParameter model)
        {
            var result =  await _context.ParentParameters.Where(x=>Id==x.Id).FirstOrDefaultAsync();
            Console.WriteLine(result.ParentTypesId);
            result.ParentTypesId = model.ParentTypeId;
            await _context.SaveChangesAsync();
        }
        [HttpPut("{id}")]
        public async Task UpdateDescriptionParam(int Id , [FromBody] UpdateDescriptionParameter model)
        {
            var result =  await _context.Parameters.FindAsync(Id);
            result.Description = model.Description;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            result.Color = model.Color;
            await _context.SaveChangesAsync();
        }
        [HttpDelete("{Id}")]
        public async Task DeleteParam(int Id)
        {
            var result =  await _context.ParentParameters.Where(x=>x.Id==Id).FirstOrDefaultAsync()!;
            
            _context.ParentParameters.Remove(result!);
            
        }
        [HttpDelete("{id}")]
        public async Task DeleteDescriptionParam(int Id)
        {
            var result =  await _context.Parameters.FindAsync(Id);
            _context.Parameters.Remove(result!);
        }
    }
    public class ParentParamItemDto{
        public int Id {get;set;}
        public String Name {get;set;}
    }
    public class ParamOverview: ParameterReadPlantDto{

        public int GroupName {get;set;}
        public int PlantId {get;set;}
        public string PlantName{get;set;}
        public int MicroId {get;set;}
        public decimal Value{get;set;}
    }
    public class UpdateParameter{
        public int ParentTypeId {get;set;}
        
    }
    public class UpdateDescriptionParameter{
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
        
    }

    public class CreateDescriptionParameter{
        public int PlantId {get;set;}
        public int ParentParamId {get;set;}
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
    }
    public class CreateParameter{
        public int PlantId {get;set;}
        public int GroupName {get;set;} //parent typpe did
        
        public List<DescriptionCreateParameter> Descriptions {get;set;}
        
    }
    public class CreateParentParamResponse{
        public int Id{get;set;}
        public List<int> ParamIds{get;set;}
    }
    public class DescriptionCreateParameter{
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
    }
    public class DeleteParameter{
        public List<int> Ids {get;set;}
    }
     public class ParamOverv{
        public List<int>? Ids {get;set;}
        public List<string>? GNames {get;set;}
    }
}
