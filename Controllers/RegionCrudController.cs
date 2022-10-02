using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RegionCrudController : ControllerBase
    {
        private readonly AppDbContext context;
        public RegionCrudController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<RegionsItemDto>> ShowRegion()
        {
            return (await this.context.Regions.Where(x=>x.DeletedAt==null).Include(x => x.MiniPcs).ThenInclude(x => x.Mikrokontrollers).Include(x => x.Land).ToListAsync()).Select(x => new RegionsItemDto
            {
                Id = x.Id,
                Name = x.Name,
                RegionDescription = x.RegionDescription,
                CordinateRegion = x.CordinateRegion,
                NMiniPc = x.MiniPcs.Count(),
                LandId = x.LandId,
                LandName = x.Land.Name, 
                NMicrocontroller = x.MiniPcs.Select( t => t.Mikrokontrollers).Count()
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<RegionItemMinimalDto>> ShowRegionMinimal(int LandId)
        {
            return (await context.Regions
            // Include(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x=>x.Plant)
            .Where(x=>x.DeletedAt==null).Where(x=>x.LandId==LandId).ToListAsync())
            .Select(x =>
            {
                return new RegionItemMinimalDto
                {
                    Id = x.Id,
                    // Name = x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",
                    Name=x.Plant.Name,
                    LandId = x.LandId,
                    RegionDescription = x.RegionDescription,
                    PlantId=x.PlantId,
                    PlantName=x.Plant.Name
                    // PlantId= x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Id  :0,
                    // PlantName= x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",
                };
            }).ToList();
        }
        [HttpGet]
        public async Task<RegionSearchResponse> Search([FromQuery] SearchRequest query)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Regions.Where(x=>x.DeletedAt==null)
            // .Include(x=>x.RegionPlant).ThenInclude(x=>x.Plant)
            .Include(x=>x.Plant)
            .Include(x => x.MiniPcs).ThenInclude(x => x.Mikrokontrollers)
            .Include(x => x.MiniPcs).Include(x => x.Land).Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                // var lastPlant = x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :null;
                return new RegionsItemDto
                {
                    Id = x.Id,
                    Name = x.Name,//x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",
                    CordinateRegion = x.CordinateRegion,
                    NMiniPc = x.MiniPcs.Count(),
                    RegionDescription = x.RegionDescription,
                    LandId = x.LandId,
                    LandName = x.Land.Name,
                    PlantId=x.PlantId,
                    PlantName=x.Plant.Name,
                    NMicrocontroller = x.MiniPcs.Select( t => t.Mikrokontrollers).Count()
                    // PlantId= x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Id  :0,
                    // PlantName= x.RegionPlant !=null && x.RegionPlant.Count()>0? x.RegionPlant.OrderBy(x=>x.CreatedAt).LastOrDefault()!.Plant.Name  :"-",
                };
            }).ToList();
            return new RegionSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpPost]
        public async Task<int> AddRegion([FromBody] CreateRegionDto model)
        {
            var reg = new Region { 
                    Name = model.Name, 
                    RegionDescription = model.RegionDescription, 
                    CordinateRegion = model.CordinateRegion, 
                    LandId = model.LandId,
                    PlantId=model.PlantId
                    };
            // reg.RegionPlant.Add(new RegionPlant{PlantId=model.PlantId});
            var obj_baru = await this.context.Regions.AddAsync(reg);

            await this.context.SaveChangesAsync();
            int id = obj_baru.Entity.Id;
            return id;
            // new RegionPlant{PlantId=model.PlantId,RegionId=obj_baru.Entity.Id} 
        }
        [HttpPut("{RegionId}")]
        public async Task UpdateRegion(int RegionId, [FromBody] UpdateRegionDto model)
        {
            var result = await this.context.Regions.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==RegionId);
            result.Name = model.Name;
            result.RegionDescription = model.RegionDescription;
            result.CordinateRegion = model.CordinateRegion;
            // result.RegionPlant.Add(new RegionPlant{PlantId=model.PlantId});
            result.PlantId=model.PlantId;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{RegionId}")]
        public async Task DeleteRegion(int RegionId)
        {
            var result = await this.context.Regions.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==RegionId);
            this.context.Regions.Remove(result!);
            await this.context.SaveChangesAsync();
        }
    }

    public class RegionsItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegionDescription { get; set; }
        public string CordinateRegion { get; set; }
        public int NMiniPc { get; set; }
        public int LandId { get; set; }
        public string LandName { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        
        public int NMicrocontroller {get;set;}

    }
    public class RegionItemMinimalDto
    {
        public int Id { get; set; }

        public string RegionDescription { get; set; }
        public string Name { get; set; }
        public int LandId { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }



    }
    public class RegionSearchResponse : SearchResponse<RegionsItemDto>
    {

    }
    // public class RegionPlantItemDto{
    //     public int Id { get; set; }
    //     public string Name { get; set; }
    //     public string LatinName { get; set; }
    //     public string Description { get; set; }

    // }
    public class CreateRegionDto
    {
        public string Name { get; set; }

        public string RegionDescription { get; set; }
        public string? CordinateRegion { get; set; }

        public int LandId { get; set; }
        public int PlantId { get; set; }


    }
    public class UpdateRegionDto
    {
        public string Name { get; set; }

        public string RegionDescription { get; set; }
        public string? CordinateRegion { get; set; }
        public int PlantId { get; set; }
        

    }
}