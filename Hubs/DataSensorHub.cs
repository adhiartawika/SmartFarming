using backend.Commons;
using backend.Model.AppEntity;
using backend.Persistences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenHouseControlWebApp.Hubs
{
    [AllowAnonymous]
    public class GreenHouseHub:Hub
    {
        private readonly AppDbContext appContext;
        private readonly ICurrentIoTService currentIoTService;
        private readonly IDateTime dateTime;
        private static Dictionary<string, string> connGroup = new Dictionary<string, string>();
        public GreenHouseHub(AppDbContext appContext, ICurrentIoTService currentIoTService, IDateTime dateTime)
        {
            this.appContext = appContext;
            this.currentIoTService = currentIoTService;
            this.dateTime = dateTime;
        }
        public async  Task JoinRoom(string roomId)
        {
            if (roomId.Contains("-_-iot"))//
            {
                var id = int.Parse(roomId.Split("-_-")[0]);
                //var id = this.currentIoTService.IoTId;
                var gh = this.appContext.Mikrokontrollers.Where(x => x.IotId == id).FirstOrDefault();
                if (gh != null)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus { 
                            MikrokontrollerId=gh.Id,       
                            IdIoT=id,
                            IsActive=true,
                            CreatedAt= this.dateTime.Now
                        });
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = true });
                }
                GreenHouseHub.connGroup.Add(Context.ConnectionId, id+"_gh");
                await Groups.AddToGroupAsync(Context.ConnectionId, id + "_gh");
            }
            else
            {

                GreenHouseHub.connGroup.Add(Context.ConnectionId, roomId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
        }

        public async  Task LeaveRoom(string roomId)
        {
            if (roomId.Contains("-_-iot"))
            {
                var id = int.Parse(roomId.Split("-_-")[0]);
                //var id = this.currentIoTService.IoTId;
                var gh = this.appContext.Mikrokontrollers.Where(x => x.IotId == id).FirstOrDefault();
                if (gh != null)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus
                        {
                            MikrokontrollerId = gh.Id,
                            IdIoT = id,
                            IsActive = false,
                            CreatedAt = this.dateTime.Now
                        });
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = false });
                }
                GreenHouseHub.connGroup.Add(Context.ConnectionId, id + "_gh");
                await Groups.AddToGroupAsync(Context.ConnectionId, id + "_gh");
            }
            else
            {
                GreenHouseHub.connGroup.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                string roomId = null;
                var tryVal = GreenHouseHub.connGroup.TryGetValue(Context.ConnectionId, out roomId);
                if (tryVal && roomId.Contains("_gh"))
                {
                    var id = int.Parse(roomId.Split("_")[0]);
                    //var id = this.currentIoTService.IoTId;
                    var gh = this.appContext.Mikrokontrollers.Where(x => x.IotId == id).FirstOrDefault();
                    if (gh != null)
                    {
                        this.appContext.IotStatus.Add(
                            new IotStatus
                            {
                                MikrokontrollerId = gh.Id,
                                IdIoT = id,
                                IsActive = false,
                                CreatedAt = this.dateTime.Now
                            });
                        await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                        await Clients.Group(gh.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = false });
                    }
                    GreenHouseHub.connGroup.Remove(Context.ConnectionId);

                }
                else if (tryVal)
                {
                    GreenHouseHub.connGroup.Remove(Context.ConnectionId);
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                await base.OnDisconnectedAsync(exception);
            }
            catch(Exception ex)
            {
                await base.OnDisconnectedAsync(exception);
            }
        }
    }

    public class IoTChangeStatusDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set;}
    }
}
