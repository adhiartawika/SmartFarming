using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LandCrudController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;
        public LandCrudController(AppDbContext context,  ICurrentUserService currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
        }
        [HttpGet]
        public async Task<IEnumerable<LandItemDto>> ShowLand()
        {
            return (await context.Lands.ToListAsync()).Select(x => new LandItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Address = x.Address,
                Photo = x.Photo,
                CordinateLand = x.CordinateLand   
            }).ToList();
        }
        [HttpPost]
        public async Task<IActionResult> AddLand([FromForm] CreateLandDto form)
        {
        var model = new Land
            {
                Name = form.Name,
                Address = form.Address,
                Code = form.Code,
                CordinateLand = form.CordinateLand
            };
            if (form.Photo != null)
            {
                if (form.Photo.Length > 500000)
                {
                    return new BadRequestObjectResult(new AppResponse { Message = "Foto terlalu besar" });
                }
                if (form.Photo.Length > 0)
                {
                    ////Getting FileName
                    //var fileName = Path.GetFileName(form.Photo.FileName);
                    ////Getting file Extension
                    //var fileExtension = Path.GetExtension(fileName);
                    //// concatenating  FileName + FileExtension
                    //var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    using (var target = new MemoryStream())
                    {
                        form.Photo.CopyTo(target);
                        model.Photo = target.ToArray();
                    }
                }
            }
            else
            {
                model.Photo = new Byte[] { };
            }
            try
            {

                var temp = this.context.Lands.Add(model);
                var res = await this.context.SaveChangesAsync(new CancellationToken());
                return new OkObjectResult(new CreateResponse<int> { Id = temp.Entity.Id, Message = "Berhasil menambah greenhouse baru" });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new AppResponse { Message = "Gagal menambah greenhouse baru" });
            }
        }
        [HttpPut("{LandId}")]
        public async Task<IActionResult> UpdateLand(int LandId ,[FromForm] UpdateLandDto form)
        {
            var result =  await context.Lands.FindAsync(LandId);
            result.Name = form.Name;
            result.Code = form.Code;
            result.Address = form.Address;
            result.CordinateLand = form.CordinateLand;
            if (form.Photo != null)
            {
                if (form.Photo.Length > 500000)
                {
                    return new BadRequestObjectResult(new AppResponse { Message = "Foto terlalu besar" });
                }
                if (form.Photo.Length > 0)
                {
                    using (var target = new MemoryStream())
                    {
                        form.Photo.CopyTo(target);
                        result.Photo = target.ToArray();
                    }
                }
            }
            else
            {
                result.Photo = new Byte[] { };
            }
            try
            {
                var res = await this.context.SaveChangesAsync(new CancellationToken());
                return new OkObjectResult(new CreateResponse<int> { Message = "Berhasil menambah greenhouse baru" });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new AppResponse { Message = "Gagal menambah greenhouse baru" });
            }

            await context.SaveChangesAsync();
        }
        [HttpDelete("{LandId}")]
        public async Task DeletePlant(int LandId)
        {
            var result = await this.context.Lands.FindAsync(LandId);
            this.context.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }

    public class LandItemDto
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Code {get;set;}
        public string Address {get;set;}
        public byte [] Photo { get; set; }
        public string CordinateLand {get;set;}
    }
    public class CreateLandDto
    {
        public string Name {get;set;}
        public string Code {get;set;}
        public string Address {get;set;}
        public IFormFile Photo { get; set; }
        public string CordinateLand {get;set;}
    }
    public class UpdateLandDto
    {
        public string Name {get;set;}
        public string Code {get;set;}
        public string Address {get;set;}
        public IFormFile Photo { get; set; }
        public string CordinateLand {get;set;}
    }
}