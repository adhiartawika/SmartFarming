using backend.Model.AppEntity;
using backend.Persistences;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebPush;

namespace backend.Commons
{
    public interface INotificationService
    {
        Task sendNotifGH(int ghId, string title, string message, string url);
        Task<int> add(string key, string browser, string device, string os, int ghId);
        Task<int> remove(int ghId, string browser, string device, string os);
    }
    public class NotificationService : INotificationService
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService currentUser;
        private readonly IConfiguration configuration;

        public NotificationService(IConfiguration configuration, IAppDbContext context, ICurrentUserService currentUser)
        {
            this.configuration = configuration;
            this.context = context;
            this.currentUser = currentUser;
        }
        public async Task<int> add(string key, string browser, string device, string os, int ghId)
        {
            var temp = this.context.UserDevices.FirstOrDefault(x => x.UserId == this.currentUser.UserId && x.Browser == browser && x.Device == device && x.Os == os);
            if (temp != null)
            {
                temp.DeviceKey = key;
                this.context.UserDevices.Update(temp);
            }
            else
            {
                var n = new UserDevice
                {
                    DeviceKey = key,
                    Os = os,
                    Device = device,
                    Browser = browser,
                    UserId= (int)this.currentUser.UserId,
                    LandId=ghId
                };
                this.context.UserDevices.Add(n);
            }
            return await this.context.SaveChangesAsync(new CancellationToken());
        }

        public async Task<int> remove(int ghId, string browser, string device, string os)
        {
            var temp = this.context.UserDevices.FirstOrDefault(x => x.UserId == this.currentUser.UserId && x.LandId == ghId && x.Browser == browser && x.Device == device && x.Os == os);
            if (temp != null)
            {
                this.context.UserDevices.Remove(temp);
                return await this.context.SaveChangesAsync(new CancellationToken());
            }
            else
            {
                return 0;
            }
        }

        public async Task sendNotifGH(int ghId, string title, string message, string url)
        {
            var webPushClient = new WebPushClient();
            string pubKey = this.configuration["WebPushNotification:PublicKey"];
            string priKey = this.configuration["WebPushNotification:PrivateKey"];
            string subKey = this.configuration["WebPushNotification:Subject"];
            var devices = this.context.UserDevices.Where(x => x.LandId == ghId).ToList();
            foreach (var d in devices)
            {
                var clientkey = JsonConvert.DeserializeObject<ClientBrowser>(d.DeviceKey);
                var pushEndpoint = @clientkey.endpoint;
                var p256dh = @clientkey.keys.p256dh;
                var auth = @clientkey.keys.auth;
                var subject = subKey;


                var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
                var vapidDetails = new VapidDetails(subject, pubKey, priKey);
                try
                {
                    var options = new
                    {
                        notification = new
                        {
                            title = title,
                            body = message,
                            vibrate = new[] { 100, 50, 100 },
                            priority = 0,
                            renotify=true,
                            data = new
                            {
                                id = d.LandId,
                                url = url //string.Format("/dashboard/{0}/overview",d.LandId)
                            },

                        }
                    };
                    webPushClient.SendNotification(subscription, JsonConvert.SerializeObject(options), vapidDetails);
                }
                catch (WebPushException exception)
                {
                    var t = this.context.UserDevices.FirstOrDefault(x => x.Id == d.Id);
                    this.context.UserDevices.Remove(t);
                    await this.context.SaveChangesAsync(new CancellationToken());
                    //Console.WriteLine("Http STATUS code" + exception.StatusCode);
                }
            }
        }
    }
    public class ClientBrowser
    {
        public string endpoint { get; set; }
        public Nullable<DateTime> expirationTime { get; set; }
        public ClientBrowserKey keys { get; set; }
    }

    public class ClientBrowserKey
    {
        public string p256dh { get; set; }
        public string auth { get; set; }
    }
}
