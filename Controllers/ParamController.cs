using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ParamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParamController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPut("{id}")]
        public async Task UpdateParam(int Id , [FromBody] UpdateParameterDto model)
        {
            var result =  await _context.Parameters.FindAsync(Id);
            result.GroupName = model.GroupName;
            result.Description = model.Description;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            await _context.SaveChangesAsync();
        }
    }
    public class UpdateParameterDto{
        public string GroupName {get;set;}
        public string Description {get;set;}
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
    }
}
