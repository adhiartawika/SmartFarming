using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Model.AppEntity;
using backend.Persistences;
using backend.Commons;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CameraController:ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IHubContext<DataParamSensorHub> _hubContext;


        public CameraController(AppDbContext context, IHubContext<DataParamSensorHub> hubContext){
            this.context = context;
            this._hubContext = hubContext;
        }

        [HttpPost]
        public async Task ActivateCamera([FromBody] NegotiatingRTCPCWithId model){
            await this._hubContext.Clients.Group(model.Id.ToString()+"_iot").SendAsync("req_camera",model.Data);
        }

    }

    public class NegotiatingRTCPCWithId
    {
        public int Id {get;set;}
        public NegotiatingRTCPC Data {get;set;}
    }

    public class NegotiatingRTCPC
    {
        public string sdp {get;set;}
        public string type {get;set;}
    }
}