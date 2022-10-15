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
        public readonly static string USER_REGION = "_USERREGION";

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
        public async Task RPIJoinRoom(string modelId, List<int> espsIds ){
            Console.WriteLine("RPI JOIN ROOM " + modelId);

            if (modelId.Contains(FarmingEndCategory.RPI))//
            {
                var id = int.Parse(modelId.Split("_")[0]);
                // var id = this.currentIoTService.IoTId;
                var gh = this.appContext.MiniPcs.Where(x => x.Id == id)
                            .Include(x => x.Mikrokontrollers)
                            .Include(x=>x.Region).ThenInclude(x=>x.Land)
                            .FirstOrDefault();
                if (id > 0)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus
                        {
                            MicroControllerId = null,
                            MiniPcId = id,
                            IsActive = true,
                            CreatedAt = this.dateTime.Now
                        });
                    List<int> mikroIds = gh.Mikrokontrollers.Select(x=>x.Id).ToList();
                    List<int> validMicroIds = new List<int>();
                    for (int i = 0; i < espsIds.Count(); i++)
                    {   
                        if(mikroIds.Contains(espsIds.ElementAt(i))){
                            validMicroIds.Add(espsIds.ElementAt(i));
                            this.appContext.IotStatus.Add(
                            new IotStatus
                            {
                                MicroControllerId = espsIds.ElementAt(i),
                                MiniPcId = null,
                                IsActive = true,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                    }
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.Region.Id.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = true,ESPIds=validMicroIds });
                }
                FarmingHub.connGroup.Add(Context.ConnectionId, modelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, modelId);
            }
        }

        public async Task RPILeaveRoom(string modelId, List<int> espsIds ){
            if (modelId.Contains(FarmingEndCategory.RPI))//
            {
                var id = int.Parse(modelId.Split("_")[0]);
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
                    for (int i = 0; i < espsIds.Count(); i++)
                    {   
                        if(mikroIds.Contains(espsIds.ElementAt(i))){
                            validMicroIds.Add(espsIds.ElementAt(i));
                            this.appContext.IotStatus.Add(
                            new IotStatus
                            {
                                MicroControllerId = espsIds.ElementAt(i),
                                MiniPcId = null,
                                IsActive = false,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                    }
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.Region.Id.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = true, ESPIds=validMicroIds });
                }
                FarmingHub.connGroup.Add(Context.ConnectionId, modelId);
                await Groups.AddToGroupAsync(Context.ConnectionId, modelId);
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
        
        public async Task UserRegionJoinRoom(string roomId)
        {
            Console.WriteLine("UserRegionJoinRoom "+roomId);
            if (roomId.Contains(FarmingEndCategory.USER_REGION))//
            {
                FarmingHub.connGroup.Add(Context.ConnectionId, roomId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
        }

        public async Task UserRegionLeaveRoom(string roomId)
        {
            if (roomId.Contains(FarmingEndCategory.USER_REGION))
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
                    // var id = this.currentIoTService.IoTId;
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
                        await Clients.Group(gh.Region.Id.ToString()+FarmingEndCategory.USER).SendAsync("RPIStatusChange", new RPIStatusChangeDto { Id = id, IsActive = false, ESPIds=espsId});
                    }
                    FarmingHub.connGroup.Remove(Context.ConnectionId);

                }
                else if (tryVal && roomId!.Contains(FarmingEndCategory.USER))
                {
                    FarmingHub.connGroup.Remove(Context.ConnectionId);
                }
                else if (tryVal && roomId!.Contains(FarmingEndCategory.USER_REGION))
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
            Console.WriteLine("ReqCamera "+ model.Id.ToString()+FarmingEndCategory.RPI);

            await Clients.Group(model.Id.ToString()+FarmingEndCategory.RPI).SendAsync("ReqActivatingCamera", new NegotiatingRTCPCDto { sdp=model.sdp, type = model.type});
            
        }
        public async Task AnswerReqCamera(string modelId, string sdp, string type)
        {
            Console.WriteLine("AnswerReqCamera "+modelId.ToString()+FarmingEndCategory.USER_REGION);
            var mnpc = await this.appContext.MiniPcs.Include(x=>x.Region).FirstOrDefaultAsync(x=>x.Id==int.Parse(modelId));
            await Clients.Group(mnpc!.RegionId.ToString()+FarmingEndCategory.USER_REGION).SendAsync("AnswerReqActivatingCamera", new NegotiatingRTCPCDto { sdp=sdp, type = type});
            // await Clients.Group(modelId.ToString()+FarmingEndCategory.USER_REGION).SendAsync("AnswerReqActivatingCamera", new NegotiatingRTCPCDto { sdp=sdp, type = type});
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
        public string sdp {get;set;}
        public string type {get;set;}
    }
}
