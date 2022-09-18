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

        public MikroCrudController(AppDbContext context){

            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<MicroItemDto>> ShowMicro(){
            return (await this.context.Mikrokontrollers.Include(x =>x.Region).ToListAsync()).Select(y => new MicroItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name
            });
        }

        [HttpPost]
        public async Task<int> AddMicro([FromBody] AddMicroDto model){
            var AddMicro = await this.context.Mikrokontrollers.AddAsync(new Mikrokontroller{
                Name = model.Name,
                Description = model.Description,
                RegionId = model.RegionId
            });
            return await this.context.SaveChangesAsync();
        }

        [HttpPut("MicroId")]
        public async Task UpdateMicro( int MicroId ,[FromBody] UpdateMicroDto model){
            var result = await this.context.Mikrokontrollers.FindAsync(MicroId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.RegionId = model.RegionId;
            await this.context.SaveChangesAsync();
        }
    }

    public class AddMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int RegionId{get; set;}

    }
    public class UpdateMicroDto{
        public string Name {get; set;}
        public string Description {get; set;}

        public int RegionId{get; set;}

    }
    public class MicroItemDto{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}
    }
}