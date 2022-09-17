using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;

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
        public async Task<IEnumerable<RegionsItemDto>> ShowRegion(){
            return (await this.context.Regions.ToListAsync()).Select(x => new RegionsItemDto{
                Id = x.Id,
                Name = x.Name,
                RegionDescription = x.RegionDescription,
                CordinateRegion = x.CordinateRegion,
            });
        }
        [HttpPost]
        public async Task AddRegion([FromBody] CreateRegionDto model){

            var obj_baru = await this.context.Regions.AddAsync(new Region { Name = model.Name, RegionDescription = model.RegionDescription, CordinateRegion = model.CordinateRegion, LandId = model.landId });

            await this.context.SaveChangesAsync();
        }
        [HttpPut("{RegionId}")]
        public async Task UpdateRegion(int RegionId ,[FromBody] UpdateRegionDto model)
        {
            var result =  await this.context.Regions.FindAsync(RegionId);
            result.Name = model.Name;
            result.RegionDescription = model.RegionDescription;
            result.CordinateRegion = model.CordinateRegion;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{RegionId}")]
        public async Task DeleteRegion(int RegionId)
        {
            var result = await this.context.Regions.FindAsync(RegionId);
            this.context.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }

    public class RegionsItemDto{
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegionDescription { get; set; }
        public string CordinateRegion { get; set; }
    }

    public class RegionPlantItemDto{
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description { get; set; }

    }
    public class CreateRegionDto{
        public int Id { get; set; }
        public string Name { get; set; }

        public string RegionDescription { get; set; }
        public string CordinateRegion { get; set; }
        
        public int landId {get; set;}
        
    }
    public class UpdateRegionDto{
        public int Id { get; set; }
        public string Name { get; set; }

        public string RegionDescription { get; set; }
        public string CordinateRegion { get; set; }
    }
}