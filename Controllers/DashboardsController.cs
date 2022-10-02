// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using backend.Model.AppEntity;
// using backend.Persistences;
// using backend.Commons;

// namespace backend.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]/[action]")]
//     public class DashboardsController : ControllerBase
//     {
//         private readonly AppDbContext context;

//         public DashboardsController(AppDbContext context)
//         {
//             this.context = context;
//         }
//         [HttpGet]
//         public async Task<IEnumerable<DashboardDtoItem>> ShowLand()
//         {
//             return (await context.Lands.Where(x=>x.DeletedAt==null).Include(x => x.Region).ThenInclude(x => x.MiniPcs).ThenInclude(x => x.Mikrokontrollers).ToListAsync())
//             .Select(x =>
//             {
//                 return new DashboardDtoItem
//                 {
//                     Id = x.Id,
//                     Name = x.Name,
//                     Code = x.Code,
//                     Address = x.Address,
//                     Photo = x.Photo,
//                     CordinateLand = x.CordinateLand,
//                     NRegion=x.Region.Count(),
//                     NMicrocontroller = x.Region.Sum(y=>y.MiniPcs.Count()), //micro
//                 };
//             }).ToList();
//     }
//     public class DashboardDtoItem{
//         public string LandName;
//         public string RegionName;

//         public string MicroName;
//         public string SensorName;
        

//     }
// }
