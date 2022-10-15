using backend.Commons;
using backend.Model.IdEntity;
using backend.Model.AppEntity;
using backend.Hubs;
using backend.Persistences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IoTController : ControllerBase
    {
        private readonly AppDbContext context;
        private IConfiguration config;
        private readonly IHubContext<FarmingHub> hub;
        private readonly ICurrentIoTService currentIOTService;
        private readonly INotificationService notifService;

        public IoTController(
            AppDbContext context,
            IConfiguration config,
            IHubContext<FarmingHub> hub,
            ICurrentIoTService currentIOTService,
            INotificationService notifService
            )
        {
            this.context = context;
            this.config = config;
            this.hub = hub;
            this.currentIOTService = currentIOTService;
            this.notifService = notifService;

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] IoTLoginForm form)
        {
            var iot = await this.context.IdentityIoTs.Include(x=>x.MiniPcs.OrderByDescending(v=>v.CreatedAt).Take(1)).Where(x => x.Code == form.Code && x.Secret == form.Password).FirstOrDefaultAsync();

            if (iot != null)
            {
                var claims = new Claim[]
                   {
                        new Claim(JwtRegisteredClaimNames.Sub,iot.Id.ToString()),
                        new Claim("id_region",iot.MiniPcs.FirstOrDefault().RegionId.ToString())
                   };
                var secret = this.config.GetSection("JwtSettings").GetValue<string>("IoTSymKey");//["JwtSettings:IoTSymKey"];
                Console.WriteLine(secret);
                var secretByte = Encoding.UTF8.GetBytes(secret);
                var key = new SymmetricSecurityKey(secretByte);
                var algorithm = SecurityAlgorithms.HmacSha256;
                var signinCredentials = new SigningCredentials(key, algorithm);
                var token = new JwtSecurityToken(null, null, claims, null, null, signinCredentials);
                var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
                return new OkObjectResult(new LoginResponse { Message = "Masuk.", AccessToken = tokenJson });
            }
            return new BadRequestObjectResult(new AppResponse { Message = "IoT tidak ditemukan." });
        }
    }
    public class IoTSubmitDataForm
    {
        public IEnumerable<IoTSubmitResultData> Datas { get; set; }

    }
    public class IoTSubmitResultData
    {
        public int GreenHousePlantId { get; set; }
        public decimal RFanTime { get; set; }
        public bool IsRFanTimeNew { get; set; }
        public decimal RFanMode { get; set; }
        public bool IsRFanModeNew { get; set; }
        public decimal RServoTime { get; set; }
        public decimal rNotifState { get; set; }
        public bool IsRServoTimeNew { get; set; }
        public decimal RWaterPumpTime { get; set; }
        public bool IsRWaterPumpTimeNew { get; set; }
        public decimal RLampState { get; set; }
        public bool IsRLampStateNew { get; set; }
        public DateTime CreatedInIoT { get; set; }

        public IEnumerable<IoTSubmitDetailData> Details { get; set; }
    }
    public class IoTSubmitDetailData
    {
        public string Code{ get; set; }
        public int Index { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedInIoT { get; set; }

    }
    public class IoTLoginForm
    {
        public string Code { get; set; }
        public string Password { get; set; }
    }
    public class IoTSubmitDetailDataBroadcast
    {
        public string Code { get; set; }
        public decimal Value { get; set; }
    }
}
