using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;
using System;
using System.IO;
using System.Web;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DiseaseImageController : ControllerBase
    {
      private readonly AppDbContext context;

        private readonly ICurrentUserService currentUser;
        private readonly IUtilityCurrentUserAces UserAcess;
        private readonly IWebHostEnvironment _env;
        public DiseaseImageController(AppDbContext context,IUtilityCurrentUserAces UserAcess, ICurrentUserService currentUser, IWebHostEnvironment _env){
            this.context = context;
            this.currentUser = currentUser;
            this.UserAcess = UserAcess;
            this._env = _env;
        }
        [HttpPost]
        public async Task<IEnumerable<DiseaseImageDtow>> AddImageDisease([FromForm] AddDiseaseImageDto form){
            List<DiseaseImageDtow> diseaseimg = new List<DiseaseImageDtow>();
            if(form.DiseaseImage != null && form.DiseaseImage.Count > 0){
                for( int i = 0; i < form.DiseaseImage.Count(); i++){
                    var special = Guid.NewGuid().ToString();
                    string webRoot = this._env.WebRootPath;
                    var filePath = Path.Combine(webRoot, "Images", special+form.DiseaseImage[i].FileName);
                    var savePath = Path.Combine("Images",special+form.DiseaseImage[i].FileName);
                    using( var file = new FileStream(filePath, FileMode.Create))
                    {
                        form.DiseaseImage[i].CopyTo(file);
                    }
                    var data = new DiseaseImage{
                        Path = filePath
                    };
                    var temp = this.context.DiseaseImages.Add(data);
                    await this.context.SaveChangesAsync(new CancellationToken());
                    diseaseimg.Add(new DiseaseImageDtow{
                        Id = temp.Entity.Id,
                        Path = savePath
                    });
                }
            }
            return diseaseimg;
        }
        // [HttpPut("{DiseaseMonitorId:int?}")]
        // public async Task<IActionResult> UpdateImageDisease([FromForm] UpdateDiseaseImageDto form, int DiseaseMonitorId){
        //     var result = await this.context.DiseaseImage.FirstOrDefaultAsync(x=>x.VirusMonitorId==DiseaseMonitorId);
        //     if(form.DiseaseImage != null && form.DiseaseImage.Count > 0){
        //         foreach(var item in form.DiseaseImage){
        //             var special = Guid.NewGuid().ToString();
        //             var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Utility\Images", special+item.FileName);

        //             using( var file = new FileStream(filePath, FileMode.Create))
        //             {
        //                 item.CopyTo(file);
        //             }
        //             diseaseImgtoList.Add(filePath);
        //         }
        //         result.ImageDisease = string.Join(",",diseaseImgtoList);
        //     }
        //     result.ImageDisease = result.ImageDisease;
        //     var res = await this.context.SaveChangesAsync(new CancellationToken());
        //     return new OkObjectResult(new AppResponse { message = "Berhasil" });  
        // }
        
        [HttpGet("{DiseaseMonitorId:int?}")]
        public async Task<IEnumerable<DiseaseImageDto>> ShowImageDisease( int DiseaseMonitorId){
            var result = this.context.DiseaseImages.Where(x=>x.VirusMonitorId==DiseaseMonitorId).Select(x => new DiseaseImageDto{
                Id = x.Id,
                path = x.Path
            }).ToList();
            // var diseaseImgtoList = result.ImageDisease.Split(',').ToList();
            // string webRoot = this._env.WebRootPath;
            // string content = this._env.ContentRootPath;
            // string path ="";
            // path = Path.Combine(webRoot , "CSS");
            // List<DiseaseImageDto> Disease = new List<DiseaseImageDto>();
            // for(int i = 0; i < diseaseImgtoList.Count(); i++){

            //     byte[] readImg = System.IO.File.ReadAllBytes(diseaseImgtoList[i]);
            //     Disease.Add( new DiseaseImageDto{
            //         DiseaseImage  = readImg,
            //         Id = i,
            //     });
                
            // }
            // return Disease;
            return result;
        }
        // public async Task UpdateDiseaseImage([FromForm] DiseaseImage model ){
        //     var diseaseImg = this.context.VirusMonitors.Where(x => x.id == DiseaseMonitorId).Select(x => x.ImageDisease).FirstOrDefault();
        //     var diseaseImgtoList = diseaseImg.Split(',').ToList();
        //     if(Model.Photo.Count > 0){
        //         foreach(var item in form.ImageDisease){
        //             var special = Guid.NewGuid().ToString();
        //             var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"Utility\Images", special+item.FileName);

        //             using( var file = new FileStream(filePath, FileMode.Create))
        //             {
        //                 item.CopyTo(file);
        //             }
        //             obj.Add(filePath);
        //         }
        //     }
        // }
    }
    public class DiseaseImageDto{
        public int Id {get;set;}
        public string path {get;set;}
    }
    public class DiseaseImageDtow{
        public int Id {get;set;}
        public string Path {get;set;}
    }
    public class AddDiseaseImageDto{
        public List<IFormFile> DiseaseImage {get;set;}
    }
    public class UpdateDiseaseImageDto{
        public List<IFormFile> DiseaseImage {get;set;}
    }
}