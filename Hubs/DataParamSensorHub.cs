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
    [AllowAnonymous]
    public class DataParamSensorHub : Hub
    {
        private readonly AppDbContext appContext;
        // private readonly ICurrentIoTService currentIoTService;
        private readonly IDateTime dateTime;
        private static Dictionary<string, string> connGroup = new Dictionary<string, string>();
        public DataParamSensorHub(AppDbContext appContext,  IDateTime dateTime)
        {
            this.appContext = appContext;
            // this.currentIoTService = currentIoTService;
            this.dateTime = dateTime;
        }
        public async Task JoinRoom(string roomId)
        {
            Console.WriteLine(roomId);
            if (roomId.Contains("-_-iot"))//
            {
                var id = int.Parse(roomId.Split("-_-")[0]);
                //var id = this.currentIoTService.IoTId;
                var gh = this.appContext.Mikrokontrollers.Where(x => x.Id == id).Include(x => x.MiniPcs).FirstOrDefault();
                if (gh != null)
                {
                    this.appContext.IotStatus.Add(
                        new IotStatus
                        {
                            MicroControllerId = gh.Id,
                            MiniPcId = null,
                            IsActive = true,
                            CreatedAt = this.dateTime.Now
                        });
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.MiniPcs.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = true });
                }
                DataParamSensorHub.connGroup.Add(Context.ConnectionId, id + "_gh");
                await Groups.AddToGroupAsync(Context.ConnectionId, id + "_gh");
            }
            else
            {

                DataParamSensorHub.connGroup.Add(Context.ConnectionId, roomId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            if (roomId.Contains("-_-iot"))
            {
                var id = int.Parse(roomId.Split("-_-")[0]);
                //var id = this.currentIoTService.IoTId;
                var gh = this.appContext.Mikrokontrollers.Where(x => x.Id == id).FirstOrDefault();
                if (gh != null)
                {
                    List<IotStatus> tempIotStatus = new List<IotStatus>();
                    tempIotStatus.Add(new IotStatus
                    {
                        MicroControllerId = gh.Id,
                        MiniPcId = null,
                        IsActive = false,
                        CreatedAt = this.dateTime.Now
                    });
                    var tempEsps = this.appContext.Mikrokontrollers.Include(x => x.MiniPcs).ThenInclude(x => x.Region).Where(x => x.Id == gh.MiniPcs.RegionId).ToList();
                    for (int i = 0; i < tempEsps.Count(); i++)
                    {
                        tempIotStatus.Add(new IotStatus
                        {
                            MicroControllerId = tempEsps.ElementAt(i).Id,
                            MiniPcId = null,
                            IsActive = false,
                            CreatedAt = this.dateTime.Now
                        });
                    }
                    this.appContext.IotStatus.AddRange(tempIotStatus);
                    await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                    await Clients.Group(gh.MiniPcs.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = false });
                }
                DataParamSensorHub.connGroup.Add(Context.ConnectionId, id + "_gh");
                await Groups.AddToGroupAsync(Context.ConnectionId, id + "_gh");
            }
            else
            {
                DataParamSensorHub.connGroup.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                string? roomId = null;
                var tryVal = DataParamSensorHub.connGroup.TryGetValue(Context.ConnectionId, out roomId);
                if (tryVal && roomId!.Contains("_gh"))
                {
                    var id = int.Parse(roomId.Split("_")[0]);
                    //var id = this.currentIoTService.IoTId;
                    var gh = this.appContext.Mikrokontrollers.Where(x => x.Id == id).FirstOrDefault();
                    if (gh != null)
                    {
                        List<IotStatus> tempIotStatus = new List<IotStatus>();
                        tempIotStatus.Add(new IotStatus
                        {
                            MicroControllerId = gh.Id,
                            MiniPcId = null,
                            IsActive = false,
                            CreatedAt = this.dateTime.Now
                        });
                        var tempEsps = this.appContext.Mikrokontrollers.Include(x => x.MiniPcs).ThenInclude(x => x.Region).Where(x => x.Id == gh.MiniPcs.RegionId).ToList();
                        for (int i = 0; i < tempEsps.Count(); i++)
                        {
                            tempIotStatus.Add(new IotStatus
                            {
                                MicroControllerId = tempEsps.ElementAt(i).Id,
                                MiniPcId = null,
                                IsActive = false,
                                CreatedAt = this.dateTime.Now
                            });
                        }
                        this.appContext.IotStatus.AddRange(tempIotStatus);
                        await this.appContext.SaveChangesAsync(new System.Threading.CancellationToken());
                        await Clients.Group(gh.MiniPcs.RegionId.ToString()).SendAsync("IoTChangeStatus", new IoTChangeStatusDto { Id = id, IsActive = false });
                    }
                    DataParamSensorHub.connGroup.Remove(Context.ConnectionId);

                }
                else if (tryVal)
                {
                    DataParamSensorHub.connGroup.Remove(Context.ConnectionId);
                }
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId!);
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                await base.OnDisconnectedAsync(exception);
            }
        }
    }

    public class IoTChangeStatusDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
