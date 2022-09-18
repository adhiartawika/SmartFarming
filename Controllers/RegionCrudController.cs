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
            return (await this.context.Regions.Where(x=>x.DeletedAt==null).Include(x => x.Mikrokontroller).Include(x => x.Land).ToListAsync()).Select(x => new RegionsItemDto
            {
                Id = x.Id,
                Name = x.Name,
                RegionDescription = x.RegionDescription,
                CordinateRegion = x.CordinateRegion,
                NMicrocontroller = x.Mikrokontroller.Count(),
                LandId = x.LandId,
                LandName = x.Land.Name
            });
        }
        [HttpGet("{LandId}")]
        public async Task<IEnumerable<RegionItemMinimalDto>> ShowRegionMinimal(int LandId)
        {
            return (await context.Regions.Where(x=>x.DeletedAt==null).Where(x=>x.LandId==LandId).ToListAsync())
            .Select(x =>
            {
                return new RegionItemMinimalDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    LandId = x.LandId,
                    RegionDescription = x.RegionDescription
                };
            }).ToList();
        }
        [HttpGet]
        public async Task<RegionSearchResponse> Search([FromQuery] SearchRequest query)
        {
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Regions.Where(x=>x.DeletedAt==null).Include(x => x.Mikrokontroller).Include(x => x.Land).Where(x => x.Name.ToLower().Contains(query.Search));
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name).ToListAsync()).Select(x =>
            {
                return new RegionsItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CordinateRegion = x.CordinateRegion,
                    NMicrocontroller = x.Mikrokontroller.Count(),
                    RegionDescription = x.RegionDescription,
                    LandId = x.LandId,
                    LandName = x.Land.Name
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

            var obj_baru = await this.context.Regions.AddAsync(
                new Region { Name = model.Name, RegionDescription = model.RegionDescription, CordinateRegion = model.CordinateRegion, LandId = model.LandId });

            await this.context.SaveChangesAsync();
            return obj_baru.Entity.Id;
        }
        [HttpPut("{RegionId}")]
        public async Task UpdateRegion(int RegionId, [FromBody] UpdateRegionDto model)
        {
            var result = await this.context.Regions.Where(x=>x.DeletedAt==null).FirstOrDefaultAsync(x=>x.Id==RegionId);
            result.Name = model.Name;
            result.RegionDescription = model.RegionDescription;
            result.CordinateRegion = model.CordinateRegion;
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
        public int NMicrocontroller { get; set; }
        public int LandId { get; set; }
        public string LandName { get; set; }

    }
    public class RegionItemMinimalDto
    {
        public int Id { get; set; }

        public string RegionDescription { get; set; }
        public string Name { get; set; }
        public int LandId { get; set; }

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


    }
    public class UpdateRegionDto
    {
        public string Name { get; set; }

        public string RegionDescription { get; set; }
        public string? CordinateRegion { get; set; }

    }
}