using backend.Commons;
using backend.Model.AppEntity;
using backend.Persistences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Hubs
{
    public class FarmingEndCategory {
        public readonly static string RPI = "_RPI";
        public readonly static string USER = "_USER";

    }
    [AllowAnonymous]
    public class FarmingHub : Hub
    {
        private readonly AppDbContext appContext;
        // private readonly ICurrentIoTService currentIoTService;
        private readonly IDateTime dateTime;
        private static Dictionary<string, string> connGroup = new Dictionary<string, string>();
        public FarmingHub(AppDbContext appContext,  IDateTime dateTime)
        {
            this.appContext = appContext;
            // this.currentIoTService = currentIoTService;
            this.dateTime = dateTime;
        }
        public async Task RPIJoinRoom(RPIJoinRoomDto model){
            if (model.Id.Contains(FarmingEndCategory.RPI))//
            {
                var id = int.Parse(model.Id.Split("_")[0]);
                // var id = this.currentIoTService.IoTId;
                var gh = this.appContext.MiniPcs.Where(x => x.Id == id)
                            .Include(x => x.Mikrokontrollers)
                            .Include(x=>x.Region).ThenInclude(x=>x.Land)
                            .FirstOrDefault();
                if (gh != null)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus
                        {
                            MicroControllerId = null,
                            MiniPcId = gh.Id,
                            IsActive = true,
                            CreatedAt = this.dateTime.Now
                        });
                    List<int> mikroIds = gh.Mikrokontrollers.Select(x=>x.Id).ToList();
                    List<int> validMicroIds = new List<int>();
                    for (int i = 0; i < model.ESPIds.Count(); i++)
                    {   
                        if(mikroIds.Contains(model.ESPIds.ElementAt(i))){
                            validMicroIds.Add(model.ESPIds.ElementAt(i));
                            this.appContext.IotStatus.Add(
                            new IotStatus
                            {
                                MicroControllerId = model.ESPIds.ElementAt(i),
                                MiniPcId = null,
                                IsActive = true,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                    }
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.Region.LandId.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = true,ESPIds=validMicroIds });
                }
                FarmingHub.connGroup.Add(Context.ConnectionId, model.Id);
                await Groups.AddToGroupAsync(Context.ConnectionId, model.Id);
            }
        }

        public async Task RPILeaveRoom(RPILeaveRoomDto model){
            if (model.Id.Contains(FarmingEndCategory.RPI))//
            {
                var id = int.Parse(model.Id.Split("_")[0]);
                // var id = this.currentIoTService.IoTId;
                var gh = this.appContext.MiniPcs.Where(x => x.Id == id)
                            .Include(x => x.Mikrokontrollers)
                            .Include(x=>x.Region).ThenInclude(x=>x.Land)
                            .FirstOrDefault();
                if (gh != null)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus
                        {
                            MicroControllerId = null,
                            MiniPcId = gh.Id,
                            IsActive = false,
                            CreatedAt = this.dateTime.Now
                        });
                    List<int> mikroIds = gh.Mikrokontrollers.Select(x=>x.Id).ToList();
                    List<int> validMicroIds = new List<int>();
                    for (int i = 0; i < model.ESPIds.Count(); i++)
                    {   
                        if(mikroIds.Contains(model.ESPIds.ElementAt(i))){
                            validMicroIds.Add(model.ESPIds.ElementAt(i));
                            this.appContext.IotStatus.Add(
                            new IotStatus
                            {
                                MicroControllerId = model.ESPIds.ElementAt(i),
                                MiniPcId = null,
                                IsActive = false,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                    }
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.Region.LandId.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = true, ESPIds=model.ESPIds });
                }
                FarmingHub.connGroup.Add(Context.ConnectionId, model.Id);
                await Groups.AddToGroupAsync(Context.ConnectionId, model.Id);
            }
        }
        public async Task UserJoinRoom(string roomId)
        {
            Console.WriteLine(roomId);
            if (roomId.Contains(FarmingEndCategory.USER))//
            {
                FarmingHub.connGroup.Add(Context.ConnectionId, roomId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
        }

        public async Task UserLeaveRoom(string roomId)
        {
            if (roomId.Contains(FarmingEndCategory.USER))
            {
                FarmingHub.connGroup.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }
            
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                string? roomId = null;
                var tryVal = FarmingHub.connGroup.TryGetValue(Context.ConnectionId, out roomId);
                if (tryVal && roomId!.Contains(FarmingEndCategory.RPI))
                {
                    var id = int.Parse(roomId.Split("_")[0]);
                    //var id = this.currentIoTService.IoTId;
                    var gh = this.appContext.MiniPcs.Include(x=>x.Mikrokontrollers).Where(x => x.Id == id).FirstOrDefault();
                    if (gh != null)
                    {
                        List<IotStatus> tempIotStatus = new List<IotStatus>();
                        tempIotStatus.Add(new IotStatus
                        {
                            MicroControllerId = null,
                            MiniPcId = gh.Id,
                            IsActive = false,
                            CreatedAt = this.dateTime.Now
                        });
                        List<int> espsId = gh.Mikrokontrollers.Select(x=>x.Id).ToList();
                        for (int i = 0; i < espsId.Count(); i++)
                        {
                            tempIotStatus.Add(new IotStatus
                            {
                                MicroControllerId = espsId.ElementAt(i),
                                MiniPcId = null,
                                IsActive = false,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                        this.appContext.IotStatus.AddRange(tempIotStatus);
                        await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                        await Clients.Group(gh.RegionId.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = false, ESPIds= espsId});
                    }
                    FarmingHub.connGroup.Remove(Context.ConnectionId);

                }
                else if (tryVal && roomId!.Contains(FarmingEndCategory.USER))
                {
                    FarmingHub.connGroup.Remove(Context.ConnectionId);
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId!);
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                await base.OnDisconnectedAsync(exception);
            }
        }
    
        public async Task ReqCamera(NegotiatingRTCPCWithIdDto model)
        {
            await Clients.Group(model.Id.ToString()+FarmingEndCategory.RPI).SendAsync("ReqActivatingCamera", new NegotiatingRTCPCDto { sdp=model.Data.sdp, type = model.Data.type});
        }
        public async Task AnswerReqCamera(NegotiatingRTCPCWithIdDto model)
        {
            var mnpc = await this.appContext.MiniPcs.Include(x=>x.Region).FirstOrDefaultAsync(x=>x.Id==model.Id);
            await Clients.Group(mnpc!.Region.LandId.ToString()+FarmingEndCategory.USER).SendAsync("AnswerReqActivatingCamera", new NegotiatingRTCPCDto { sdp=model.Data.sdp, type = model.Data.type});
        }
    }

    public class RPIStatusChangeDto
    {
        public int Id { get; set; }
        public List<int> ESPIds { get; set; }
        public bool IsActive { get; set; }
    }

    public class RPIJoinRoomDto
    {
        public string Id {get;set;}
        public List<int> ESPIds {get;set;}
    }
    public class RPILeaveRoomDto
    {
        public string Id {get;set;}
        public List<int> ESPIds {get;set;}
    }

    public class NegotiatingRTCPCDto
    {
        public string sdp {get;set;}
        public string type {get;set;}
    }
    public class NegotiatingRTCPCWithIdDto
    {
        public int Id {get;set;}
        public NegotiatingRTCPCDto Data {get;set;}
    }
}
