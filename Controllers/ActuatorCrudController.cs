using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ActuatorCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public ActuatorCrudController(AppDbContext context){
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ActuatorItemDto>> ShowActuator(){
            return (await this.context.Actuators.Include(x => x.MikroController).ToListAsync()).Select(y => new ActuatorItemDto{
                Id = y.Id,
                Name = y.Name,
                Description = y.Description,
                MicroId = y.MikroController.Id,
                MicroName = y.MikroController.Name
            });
        }

        [HttpPost]
        public async Task<int> AddActuator([FromBody] AddActuatorDto model){
            var obj = await this.context.AddAsync(new Actuator{
                Name = model.Name,
                Description = model.Description,
                Type = model.type,
                MikrocontrollerId = model.MicroId
            });
            return await this.context.SaveChangesAsync();
        }

        [HttpPut("{ActuatorId}")]
        public async Task UpdateActuator(int ActuatorId,[FromBody] UpdateActuatorDto model){
            var result = await this.context.Actuators.FindAsync(ActuatorId);
            result.Name = model.Name;
            result.Description = model.Description;
            result.Type = model.type;
            result.MikrocontrollerId = model.MicroId;

            await this.context.SaveChangesAsync();
        }

        [HttpDelete("{ActuatorId}")]
        public async Task DeleteActuator(int ActuatorId){
            var result = await this.context.Actuators.FindAsync(ActuatorId);
            this.context.Actuators.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }
    public class ActuatorItemDto{
        public int Id {get;set;}
        public string Name {get; set;}

        public string Description {get;set;}
        public Typeactuator type {get;set;} 
        public int MicroId {get;set;}
        public string MicroName {get; set;}
    }

    public class AddActuatorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public Typeactuator type {get;set;} 
        public int MicroId {get;set;}

    }

    public class UpdateActuatorDto{
        public string Name {get;set;}
        public string Description{get;set;}
        public Typeactuator type {get;set;} 
        public int MicroId {get;set;}
    }
}