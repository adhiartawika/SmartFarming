using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DatasCrudController:ControllerBase
    {
        private readonly AppDbContext context;

        public DatasCrudController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<DataItemDto>> ShowData(){
            return (await this.context.Datas.Include(x => x.Parameter).ToListAsync()).Select(y => new DataItemDto{
                Id = y.Id,
                ParameterId = y.Parameter.Id,
                GroupName = y.Parameter.GroupName,
                MinValue = y.Parameter.MinValue,
                MaxValue = y.Parameter.MaxValue,
                ValueParameter = y.ValueParameter
            });
        }
        [HttpPost]
        public async Task<int> AddData([FromBody] AddDataDto model)
        {
            var obj_baru = await this.context.Datas.AddAsync(new Data { ParameterId = model.ParameterId, ValueParameter = model.ValueParameter});
            return await this.context.SaveChangesAsync();
        }

    [HttpPut("{dataId}")]
        public async Task UpdateData(int dataId,[FromBody] UpdateDataDto model)
        {
            var result = await this.context.Datas.FindAsync(dataId);
            result.ParameterId = model.ParameterId;
            result.ValueParameter = model.ValueParameter;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{dataId}")]
        public async Task DeleteSensor(int dataId){
            var result = await this.context.Datas.FindAsync(dataId);
            this.context.Datas.Remove(result);
            await this.context.SaveChangesAsync();
        }
    }
    public class  DataItemDto{
        public int Id {get; set;}
        public int ParameterId {get;set;}

        public string GroupName {get; set;}
        public double MinValue {get; set;}

        public double MaxValue {get; set;}
        public decimal ValueParameter {get; set;}
    }
    public class  AddDataDto{
        public int MicroId {get;set;}
        public int ParameterId {get;set;}
        public decimal ValueParameter {get; set;}
    }
    public class  UpdateDataDto{
        public int ParameterId {get;set;}
        public decimal ValueParameter {get; set;}
    }
}