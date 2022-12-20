using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using backend.Commons;
namespace backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class CordinateDiseaseController : ControllerBase
    {
        private readonly AppDbContext context;

        private readonly ICurrentUserService currentUser;
        private readonly IUtilityCurrentUserAces UserAcess;
        public CordinateDiseaseController(AppDbContext context,IUtilityCurrentUserAces UserAcess, ICurrentUserService currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
        }

        [HttpPost]
        public async Task<int> AddDiseaseCordinate([FromBody] AddDiseaseCordinate model)
        {
            if(this.currentUser.RoleId != 3){
                var obj_baru = await this.context.LanLatDiseases.AddAsync(
                    new LanLatDiseases{
                        latitude = model.Latitude,
                        longitude = model.Longtitude,
                        VirusMonitorId = model.DisesaseMonitorId
                    }
                );
                await this.context.SaveChangesAsync();
                return obj_baru.Entity.Id;
            }
            return 0;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiseaseCordinate(int Id , [FromBody] UpdateDiseaseCordinate model)
        {
            if(this.currentUser.RoleId != 3){
                var result =  await this.context.LanLatDiseases.Where(x=>Id==x.Id).FirstOrDefaultAsync();
                if(result != null){
                    result.latitude = model.Latitude;
                    result.longitude = model.Longtitude;
                    return new OkObjectResult(new AppResponse { message="Success"});
                }else{
                    return new BadRequestObjectResult(new AppResponse { message="Cordinate Tidak Ditemukan"});
                }
            }else{
                return new BadRequestObjectResult(new AppResponse { message="Role Tidak Dizinkan"});
            }

        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteDiseaseCordinate(int Id)
        {
            if(this.currentUser.RoleId != 3){
                var result =  await this.context.LanLatDiseases.Where(x=>x.Id==Id).FirstOrDefaultAsync()!;
                this.context.LanLatDiseases.Remove(result);
                await this.context.SaveChangesAsync();
                return new OkObjectResult(new AppResponse { message="Sucess"});
            }
            return new BadRequestObjectResult(new AppResponse { message="Role Tidak Dizinkan"});
            
        }
    }
    public class AddDiseaseCordinate{
        public double Latitude {get;set;}
        public double Longtitude {get;set;}
        public int  DisesaseMonitorId {get;set;}
    }
    public class UpdateDiseaseCordinate{
        public double Latitude {get;set;}
        public double Longtitude {get;set;}
    }
}