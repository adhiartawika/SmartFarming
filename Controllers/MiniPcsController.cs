using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Model.IdEntity;
using backend.Persistences;
using backend.Commons;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class MiniPcsCrudController : ControllerBase
    {
        private readonly AppDbContext context;

        public MiniPcsCrudController(AppDbContext context){
            this.context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<MiniPcItemDto>> ShowMiniPc(){
            return (await this.context.MiniPcs
            .Include(x =>x.Region)
            .Include(x => x.Mikrokontrollers).ToListAsync()).Select(y => new MiniPcItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                RegionId = y.Region.Id,
                RegionName = y.Region.Name,
                Code = y.Code,
                Secret = y.Secret,
                MicroItemDto = y.Mikrokontrollers.Select(x => new MicroControllerItemDto{
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            });
        }
        // [HttpPost]
        // public async Task<int> AddMiniPc([FromBody] AddMiniPcDto model){
        //     var obj = await this.context.AddAsync(new MiniPc{
        //         Name = model.Name,
        //         Description = model.Description,
        //         RegionId = model.RegionId,
                
        //     });
        //     return await this.context.SaveChangesAsync();
        // }
        [HttpPost]
        public async Task<int> AddMini([FromBody] CreateMiniDto model)
        {
            List<IdIoT> idIoTs = new List<IdIoT>();
            for (int i = 0; i < model.IdentityIot.Count(); i++)
            {
                idIoTs.Add(new IdIoT
                {
                    Name = model.IdentityIot.ElementAt(i).Name,
                    Code = model.IdentityIot.ElementAt(i).Code,
                    Secret = model.IdentityIot.ElementAt(i).Secret //membuat kondisi untuk mengecek kode apa bila sudah ada berikan exception
                });
            }
            var obj_baru = await this.context.MiniPcs.AddAsync(new MiniPc 
            { 
                Name = model.Name, 
                Description = model.Description, 
                RegionId = model.RegionId,
                Code = idIoTs.Select(c => c.Code).ToList().First(),
                Secret = idIoTs.Select(c => c.Secret).ToList().First(),
                IdIoTs =idIoTs
            });
            await this.context.SaveChangesAsync();
            return obj_baru.Entity.Id;
        }
        [HttpPut("{MiniPcId}")]
        public async Task UpdateMiniPc( int MiniPcId ,[FromBody] UpdateMiniPcDto model){
            var result = await this.context.MiniPcs.FindAsync(MiniPcId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.RegionId = model.RegionId;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{MicroId}")]
        public async Task DeleteMiniPc(int MicroId)
        {
            var result = await this.context.Mikrokontrollers.FindAsync(MicroId);
            this.context.Mikrokontrollers.Remove(result!);
            await this.context.SaveChangesAsync();
        }
    }
    public class MiniPcIdIotDto
    {

        public string Name { get; set; }
        public string Code { get; set; }
        public string Secret { get; set; }

        // ParentTypeId = PH
        // Desc = ['asam', 'optimal', 'mungkin optimal', 'basa']
        // values = [0,7,9,13]
        // values = [7,9,13,14]
    }
    public class CreateMiniDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int RegionId {get;set;}
        public int IotId {get;set;}
        public List<MiniPcIdIotDto> IdentityIot { get; set; }

    }
    public class MicroControllerItemDto{
        public int Id {get;set;}
        public string Name {get;set;}
    }
    public class MiniPcItemDto{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public string Secret {get;set;}
        public string Code {get;set;}
        public int RegionId {get; set;}
        public string RegionName {get; set;}

        public List<MicroControllerItemDto> MicroItemDto {get;set;}

    }
    public class AddMiniPcDto{
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
        public int IotId {get; set;}
    }
    public class UpdateMiniPcDto{
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
    }
}