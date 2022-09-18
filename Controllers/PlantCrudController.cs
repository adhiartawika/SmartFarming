using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlantCrudController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlantCrudController(AppDbContext context)
        {

            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ReadPlantDto>> ShowPlants()
        {
            return (await _context.Plants.Where(x=>x.DeletedAt==null).Include(x=>x.Parameters).ToListAsync()).Select(x => {
                List<ParameterReadPlantDto> Parameters = x.Parameters.Where(x=>x.DeletedAt==null).GroupBy(y=>y.GroupName).Select(z=> new ParameterReadPlantDto{
                    GroupName=z.Key,
                    Descriptions=z.Select(c=> new DescriptionReadParameterPlantDto{
                        Color=c.Color,
                        Description=c.Description,
                        MaxValue=c.MaxValue,
                        MinValue=c.MinValue,
                        Id=c.Id
                    }).ToList()
                }).ToList();
                return new ReadPlantDto
                {
                    Description = x.Description,
                    Id = x.Id,
                    LatinName = x.LatinName,
                    Name = x.Name,
                    Parameters=Parameters
                };
            });
        }
        [HttpGet]
        public async Task<PlantSearchResponse> Search([FromQuery] SearchRequest query)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this._context.Plants.Where(x=>x.DeletedAt==null).Include(x=>x.Parameters).Where(x => x.Name.ToLower().Contains(query.Search) || x.LatinName.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x => {
                List<ParameterReadPlantDto> Parameters = x.Parameters.Where(x=>x.DeletedAt==null).OrderBy(x=>x.MinValue).GroupBy(y=>y.GroupName).Select(z=> new ParameterReadPlantDto{
                    GroupName=z.Key,
                    Descriptions=z.Select(c=> new DescriptionReadParameterPlantDto{
                        Color=c.Color,
                        Description=c.Description,
                        MaxValue=c.MaxValue,
                        MinValue=c.MinValue,
                        Id=c.Id
                    }).ToList()
                }).ToList();
                return new ReadPlantDto
                {
                    Description = x.Description,
                    Id = x.Id,
                    LatinName = x.LatinName,
                    Name = x.Name,
                    Parameters=Parameters
                };
            }).ToList();
            return new PlantSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        // [HttpGet]
        // public async Task<IActionResult> GetPlant(int id){
        //     return (await _context.Plants.FirstOrDefault(a => a.Id == id)).Select(x => new PlantItemDto
        //     {
        //         Description = x.Description,
        //         Id = x.Id,
        //         LatinName = x.LatinName,
        //         Name = x.Name,
        //         Parameters=x.Parameters.Select(y=> new PlantParameterItemDto{
        //             GroupName = y.GroupName,
        //             Description = y.Description,
        //             MinValue = y.MinValue,
        //             MaxValue = y.MaxValue
        //         }).ToList()
        //     });
        // }
        [HttpPost]
        public async Task<int> AddPlant([FromBody] CreatePlantDto model)
        {
            List<Parameter> parameters = new List<Parameter>();
            
            for (int i = 0; i < model.Parameters.Count(); i++)
            {
                for (int j = 0; j < model.Parameters.ElementAt(i).Descriptions.Count(); j++)
                {
                    parameters.Add(new Parameter
                    {
                        GroupName = model.Parameters.ElementAt(i).GroupName,
                        Description = model.Parameters.ElementAt(i).Descriptions.ElementAt(j).Description,
                        MinValue = model.Parameters.ElementAt(i).Descriptions.ElementAt(j).MinValue,
                        MaxValue = model.Parameters.ElementAt(i).Descriptions.ElementAt(j).MaxValue,
                        Color = model.Parameters.ElementAt(i).Descriptions.ElementAt(j).Color,
                    });
                }
            }
            var obj_baru = await _context.Plants.AddAsync(new Plant { Name = model.Name, LatinName = model.LatinName, Description = model.Description, Parameters = parameters });
            await _context.SaveChangesAsync();
            return obj_baru.Entity.Id;
        }
        [HttpPut("{PlantId}")]
        public async Task UpdatePlant(int PlantId ,[FromBody] UpdatePlantDto model)
        {
            var result =  await _context.Plants.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==PlantId);
            result.Name = model.Name;
            result.LatinName = model.LatinName;
            result.Description = model.Description;
            await _context.SaveChangesAsync();
        }
        [HttpDelete("{PlantId}")]
        public async Task DeletePlant(int PlantId)
        {
            var result = await _context.Plants.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==PlantId);
            _context.Plants.Remove(result!);
            await _context.SaveChangesAsync();
        }
    }
    public class PlantSearchResponse: SearchResponse<ReadPlantDto>{

    }
    public class DescriptionReadParameterPlantDto{
        public int Id {get;set;}

        public string Description {get;set;}

        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}
    }
    public class ParameterReadPlantDto{
        public string GroupName {get;set;}

        public List<DescriptionReadParameterPlantDto> Descriptions {get;set;}

    }
    public class ReadPlantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }
        public List<ParameterReadPlantDto> Parameters {get;set;}
    }
    public class DescriptionPlantParameterDto{
        public string Description { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string Color { get; set; }
    }
    public class PlantParameterDto
    {
        public string GroupName { get; set; }
        public List<DescriptionPlantParameterDto> Descriptions {get;set;}

        // GroupName = PH
        // Desc = ['asam', 'optimal', 'mungkin optimal', 'basa']
        // values = [0,7,9,13]
        // values = [7,9,13,14]
    }
    public class CreatePlantDto
    {
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }

        public List<PlantParameterDto> Parameters { get; set; }
    }
    public class UpdatePlantDto
    {
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }

        
    }
}

// PlantCrud
// get: GetPlants() :List<PlantItemDto>
// post: AddPlant(CreatePlantDto) :void



