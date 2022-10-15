using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DatasCrudController : ControllerBase
    {
        private readonly AppDbContext context;

        public DatasCrudController(AppDbContext context)
        {
            this.context = context;
        }
        // [HttpGet]
        // public async Task<IEnumerable<DataItemDto>> ShowData(){
        //     return (await this.context.Datas.ToListAsync()).Select(y => new DataItemDto{
        //         Id = y.Id,
        //         ParameterId = y.ParentParam.Id,
        //         MinValue = y.ParentParam.MinValue,
        //         MaxValue = y..MaxValue,
        //         ValueParameter = y.ValueParameter
        //     });
        // }
        [HttpPost]
        public async Task AddData([FromBody] AddDataDto model)
        {
            for (int i = 0; i < model.sensor.Count(); i++)
            {
                var temp = this.context.Sensors
                            .Include(x => x.MikroController).ThenInclude(x => x.MiniPc).ThenInclude(x => x.Region).ThenInclude(x => x.Plant).ThenInclude(x => x.ParentParameters).ThenInclude(x => x.Parameters)
                            .Where(x => x.Id == model.sensor.ElementAt(i).id)
                            //.Select(x => x.MikroController.MiniPc.Region.Plant.Id)
                            .FirstOrDefault();

                // foreach(var obj in temp){
                //     Console.Write(obj+" -> ");
                // }
                // var temp2 = this.context.ParentParameters
                //             .Include(x => x.Plant)
                //             .Include(x => x.ParentTypes)
                //             .Where(x => temp.Contains(x.Plant.Id))
                //             .Where(x => x.ParentTypes.Id == model.sensor.ElementAt(i).parenttypeId)
                //             .Select(x => x.Id).FirstOrDefault();
                
                // Console.WriteLine(temp2);
                this.context.Add(new Data
                {
                    CreatedAt = DateTime.Now,
                    SensorId = model.sensor.ElementAt(i).id,
                    ValueParameter = model.sensor.ElementAt(i).value,
                    ParentParamId = temp.ParentParamId,
                });
                await this.context.SaveChangesAsync();
            }

        }

        [HttpPut("{dataId}")]
        public async Task UpdateData(int dataId, [FromBody] UpdateDataDto model)
        {
            var result = await this.context.Datas.FindAsync(dataId);
            result.ParentParamId = model.ParameterId;
            result.ValueParameter = model.ValueParameter;
            await this.context.SaveChangesAsync();
        }
        [HttpDelete("{dataId}")]
        public async Task DeleteData(int dataId)
        {
            var result = await this.context.Datas.FindAsync(dataId);
            this.context.Datas.Remove(result);
            await this.context.SaveChangesAsync();
        }
        // [HttpGet]
        // public async Task<IEnumerable<DataDayModeMc>> GraphParameterMc([FromQuery] DataDayModeMc query)
        // {
        //     var temp = DateTime.Parse(query.ChosenDate.ToString());
        //     temp = temp.AddDays(1);
        //     var dte = await this.context.Datas.Include(x => x.Parameter)
        //                         .Where(x => query.ChosenParameterIds.Contains(x.ParameterId))
        //                         .Where(x=>x.CreatedAt >= query.ChosenDate && x.CreatedAt<=temp)
        //                         //.Where(x => x.CreatedAt.Year == query.ChosenDate.Year && x.CreatedAt.Month == query.ChosenDate.Month && x.CreatedAt.Day == query.ChosenDate.Day).OrderBy(x=>x.CreatedAt)
        //                         .GroupBy(x => new { ParameterId = x.ParameterId, CreatedAt = x.CreatedAt }).Select(x=>new GreenHouseGraphParameterDto { ParameterId = x.Key.ParameterId, CreatedAt = x.Key.CreatedAt, Value = x.Average(y=>y.ValueParameter)}).ToListAsync();
        //     return dte;
        // }
    }
    public class DataItemDto
    {
        public int Id { get; set; }
        public int ParameterId { get; set; }

        public string GroupName { get; set; }
        public double MinValue { get; set; }

        public double MaxValue { get; set; }
        public decimal ValueParameter { get; set; }
    }
    public class AddDataDto
    {
        public int Id { get; set; }
        public List<AddDataSensor> sensor { get; set; }
    }
    public class AddDataSensor
    {
        public int id { get; set; }
        public decimal value { get; set; }
        public int parenttypeId { get; set; }
    }
    public class UpdateDataDto
    {
        public int ParameterId { get; set; }
        public decimal ValueParameter { get; set; }
    }

    public class DataDayModeMc
    {
        public int MicrocontrollerId { get; set; }
        public DateTime ChosenDate { get; set; }
        public IEnumerable<string>? ChosenParameterIds { get; set; }
    }
    public class DataDayModeMcResponse
    {
        public int MicrocontrollerId { get; set; }
        public string MicrocontrollerName { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public DateTime ChosenDate { get; set; }
        public List<DataParamsDay>? Data { get; set; }
    }

    public class DataParamsDay{
        public string GroupName{get;set;}
        public List<DaftarDataParam>? Data{get;set;}
    }

    public class DaftarDataParam{
        public DateTime Tanggal{get;set;}
        public decimal Value{get;set;}
    }

    public class DataDayModeParam
    {
        public List<int>? MicrocontrollerId { get; set; }
        public DateTime ChosenDate { get; set; }
        public string ChosenParameterIds { get; set; }
    }
    public class DataDayModeParamResponse
    {
        public List<DaftarMicroControllerParam>? MicrocontrollerData{ get; set; }
        public DateTime ChosenDate { get; set; }
        
    }

    public class DaftarMicroControllerParam{
        public int MicrocontrollerId { get; set; }
        public string MicrocontrollerName { get; set; }
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public DataParamsDay ParamsData{get;set;}
    }
}