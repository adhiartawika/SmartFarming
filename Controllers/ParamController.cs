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

        [HttpGet("{LandId}")]
        public async Task<IEnumerable<string>> ShowMinimalParam(int LandId){
            var temp = this._context.Regions.Include(x=>x.Plant).Where(x=>x.LandId==LandId).Select(x=>x.PlantId).ToList();
            return (await this._context.Parameters
            .Include(x =>x.Plant).ThenInclude(x=>x.Regions)
            // .ThenInclude(x=>x.RegionPlants).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            // .Where(x=>x.Plant.RegionPlants.Any(y=>y.Region.LandId ==LandId))
            .Where(x=>temp.Contains(x.PlantId))
            .ToListAsync()).Select(y => y.GroupName)
            .Distinct()
            .ToList();
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<ParamOverview>> ShowParamOverview(int LandId, [FromQuery]ParamOverv query){
            var temp = this._context.Regions.Include(x=>x.Plant).Where(x=>x.LandId==LandId).Select(x=>x.PlantId).ToList();
            var temp2= (await this._context.Parameters
            .Include(x =>x.Plant).ThenInclude(x=>x.Regions)
            // .ThenInclude(x=>x.RegionPlants).ThenInclude(x=>x.Region).ThenInclude(x=>x.Land)
            // .Where(x=>x.Plant.RegionPlants.Any(y=>y.Region.LandId ==LandId))
            .Where(x=>temp.Contains(x.PlantId))
            .ToListAsync()).Select(y =>y.GroupName)
            .ToList();
            var temp3 = this._context.Datas
                            .Include(x=>x.Sensor).ThenInclude(x=>x.MikroController)
                            .Include(x=>x.Parameter).ThenInclude(x=>x.Plant)
                            .Where(x=>temp2.Contains(x.Parameter.GroupName))
                            .Where(x=>query.Ids.Contains(x.Sensor.MikrocontrollerId))
                            .Where(x=>query.GNames.Contains(x.Parameter.GroupName))
                            .ToList();
            var temp4 = temp3.Select(x=> new ParamOverview{
                                GroupName=x.Parameter.GroupName,
                               PlantName=x.Parameter.Plant.Name,
                               PlantId=x.Parameter.PlantId,
                               MicroId=x.Sensor.MikrocontrollerId,
                               Value=x.ValueParameter
                            });
            for (int i = 0; i < temp4.Count(); i++)
            {
                var t = temp3.Where(x=>x.Parameter.PlantId == temp4.ElementAt(i).PlantId && temp4.ElementAt(i).MicroId == x.Sensor.MikrocontrollerId).OrderBy(x=>x.Parameter.MinValue).ToList();
                var k = new List<DescriptionReadParameterPlantDto>();
                for (int j = 0; j < t.Count(); j++)
                {
                    var jj = t.ElementAt(j);
                    k.Add(new DescriptionReadParameterPlantDto{
                        Color=jj.Parameter.Color,
                        Description=jj.Parameter.Description,
                        Id=jj.Parameter.Id,
                        MaxValue=jj.Parameter.MaxValue,
                        MinValue=jj.Parameter.MinValue
                    });

                }
                temp4.ElementAt(i).Descriptions=k;
            }
            return temp4;
        }

        [HttpPost]
        public async Task<List<int>> CreateParam([FromBody] CreateParameter model)
        {
            List<int> res = new List<int>();

            List<Parameter> objs = new List<Parameter>();
            for (int i = 0; i < model.Descriptions.Count(); i++)
            {
                objs.Add(new Parameter{

                    Color=model.Descriptions.ElementAt(i).Color,
                    Description=model.Descriptions.ElementAt(i).Description,
                    GroupName=model.GroupName,
                    MaxValue=model.Descriptions.ElementAt(i).MaxValue,
                    MinValue=model.Descriptions.ElementAt(i).MinValue,
                    PlantId=model.PlantId
                });
            }

            await _context.Parameters.AddRangeAsync(objs);
            await _context.SaveChangesAsync();
            for (int i = 0; i < objs.Count(); i++)
            {
                res.Add(objs.ElementAt(i).Id);
            }
            return res;
        }
        [HttpPost]
        public async Task<int> CreateDescriptionParam([FromBody] CreateDescriptionParameter model)
        {
            var obj_baru = await _context.Parameters.AddAsync(
                new Parameter{
                    Color=model.Color,
                    Description=model.Description,
                    GroupName=model.GroupName,
                    MaxValue=model.MaxValue,
                    MinValue=model.MinValue,
                    PlantId=model.PlantId
                }
            );
            await _context.SaveChangesAsync();
            return obj_baru.Entity.Id;
        }

        [HttpPut]
        public async Task UpdateParam(int Id , [FromBody] UpdateParameter model)
        {
            var result =  await _context.Parameters.Where(x=>model.Ids.Contains(x.Id)).ToListAsync();
            for (int i = 0; i < result.Count(); i++)
            {
                result.ElementAt(i).GroupName = model.GroupName;
            }
            await _context.SaveChangesAsync();
        }
        [HttpPut("{id}")]
        public async Task UpdateDescriptionParam(int Id , [FromBody] UpdateDescriptionParameter model)
        {
            var result =  await _context.Parameters.FindAsync(Id);
            result.Description = model.Description;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            result.Color =model.Color;
            await _context.SaveChangesAsync();
        }
        [HttpDelete("{PlantId}")]
        public async Task DeleteParam(int PlantId , [FromBody] DeleteParameter model)
        {
            var result =  await _context.Parameters.Where(x=>x.PlantId == PlantId && model.Ids.Contains(x.Id)).ToListAsync()!;
            if(result!=null && result.Count() > 0){ 
                _context.Parameters.RemoveRange(result);
            }
        }
        [HttpDelete("{id}")]
        public async Task DeleteDescriptionParam(int Id)
        {
            var result =  await _context.Parameters.FindAsync(Id);
            _context.Parameters.Remove(result!);
        }
    }
    public class ParamOverview: ParameterReadPlantDto{
        public int PlantId {get;set;}
        public string PlantName{get;set;}
        public int MicroId {get;set;}
        public decimal Value{get;set;}
    }
    public class UpdateParameter{
        public string GroupName {get;set;}
        public List<int> Ids {get;set;}
    }
    public class UpdateDescriptionParameter{
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
        
    }

    public class CreateDescriptionParameter{
        public int PlantId {get;set;}
        public string GroupName {get;set;}
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
    }
    public class CreateParameter{
        public int PlantId {get;set;}
        public string GroupName {get;set;}
        public List<DescriptionCreateParameter> Descriptions {get;set;}
        
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
        public List<int> Ids {get;set;}
        public List<string> GNames {get;set;}
    }
}
