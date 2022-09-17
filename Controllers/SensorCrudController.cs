using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SensorCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public SensorCrudController(AppDbContext context){
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SensorItemDto>> ShowSensor(){
            return (await this.context.Sensors.Include(x => x.MikroController).ToListAsync()).Select(y => new SensorItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MicroId = y.MikroController.Id,
                MicroName = y.MikroController.Name
            });
        }

        [HttpPost]
        public async Task<int> AddSensor([FromBody] AddSensorDto model){
            var obj = await this.context.AddAsync(new Sensor{
                Name = model.Name,
                Description = model.Description,
                Type = model.type,
                MikrocontrollerId = model.MicroId
            });
            return await this.context.SaveChangesAsync();
        }

        [HttpPut("{SensorId}")]
        public async Task UpdateSensor(int SensorId,[FromBody] UpdateSensorDto model){
            var result = await this.context.Sensors.FindAsync(SensorId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.Type = model.type;
            result.MikrocontrollerId = model.MicroId;

            await this.context.SaveChangesAsync();
        }

        [HttpDelete("{SensorId}")]
        public async Task DeleteSensor(int SensorId){
            var result = await this.context.Sensors.FindAsync(SensorId);
            this.context.Sensors.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }
    public class SensorItemDto{
        public int Id {get;set;}
        public string Name {get; set;}

        public string Description {get;set;}
        public TypeSensor type {get;set;} 
        public int MicroId {get;set;}
        public string MicroName {get; set;}
    }

    public class AddSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public TypeSensor type {get;set;} 
        public int MicroId {get;set;}

    }

    public class UpdateSensorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public TypeSensor type {get;set;} 
        public int MicroId {get;set;}
    }
}