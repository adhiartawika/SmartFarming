using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImageThreadingController:ControllerBase
    {
        public ImageThreadingController(){

        }


        [HttpPost]
        public async Task ShowDataStream(IFormFile image1){
            Console.WriteLine("hello");
            using (var memoryStream = new MemoryStream()){
                await image1.CopyToAsync(memoryStream);
                // Upload the file if less than 2 MB
                var file = new AppDto()
                {
                    Content = memoryStream.ToArray()
                };

                Console.WriteLine(file);
            }
        }
        
    }
    public class AppDto{
        public int Id {get;set;}
        public byte[] Content { get; set; }
    }
}