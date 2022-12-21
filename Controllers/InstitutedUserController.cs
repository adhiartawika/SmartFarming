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
    public class InstitutedUserController:ControllerBase{
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;
        public InstitutedUserController(AppDbContext context, ICurrentUserService currentUser){
            this.context = context;
            this.currentUser = currentUser;
        }   
        [HttpGet("{Id:int?}")]
        public async Task<AccountSearchResponse> Search([FromQuery] SearchRequest query, int id = -1)
        {
            var all_user = this.context.Users.Include(x => x.instituted).ToList();
            var user_in_instituted = this.context.Users.Include(x => x.instituted).Where(x => this.currentUser.UserId == x.Id).Select(x => x.instituted.Nama).FirstOrDefault();
            var arrayId = from obj in all_user where user_in_instituted == obj.instituted.Nama select obj.Id;
            query.Search = query.Search == null ? "" : query.Search.ToLower();
            var q = this.context.Users.Include(x => x.Roles)
            .Where(x => this.currentUser.RoleId == 1 ? true : this.currentUser.RoleId == 2 ? arrayId.Contains(x.Id): this.currentUser.RoleId == 3 ? arrayId.Contains(x.Id):false)
            .Where(x => x.Name.ToLower().Contains(query.Search));
            var userss = this.context.Users.ToList();
            var res = (await q.Skip(((query.Page - 1) < 0 ? 0 : query.Page - 1) * query.N).Take(query.N).OrderBy(x=>x.Name.ToLower()).ToListAsync()).Select(x =>
            {
                return new AccountItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    RoleId = x.Roles.Select(x => x.Id).First(),
                    Email = x.Email,
                };
            }).ToList();
            return new AccountSearchResponse
            {
                Data = res,
                NTotal = q.Count()
            };
        }
        [HttpGet]
        public async Task<IEnumerable<InstitutedDtoItem>> GetInstituteName(){
            List<InstitutedDtoItem> institutedname = new List<InstitutedDtoItem>();
            foreach(var j in this.context.Instituteds){
                InstitutedDtoItem temp = new InstitutedDtoItem{
                    Id = j.Id,
                    Name = j.Nama,
                    Alamat = j.Alamat
                };
                institutedname.Add(temp);
            }
            // return obj_mikro;
            return institutedname;
        }
    }
}