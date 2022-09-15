using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface ICurrentIoTService
    {
        int IoTId { get; }
    }
    public class CurrentIoTService : ICurrentIoTService
    {
        private IHttpContextAccessor _httpContextAccessor;
        //private IHttpClientFactory _httpClientFactory;
        public CurrentIoTService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            //this._httpClientFactory = httpClientFactory;
            //this.getIoTInfo();

            var temp = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (temp == null)
            {
                this.getIoTInfo();
            }
            else
            {
                IoTId = int.Parse(temp ?? "0");
            }
            
        }

        private async Task getIoTInfo()
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                var accessToken = _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "iot-authorization").Value;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken);
                    var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
                    //var id = tokenS.Claims.First(claim => claim.Type == "sub").Value;
                    var id = tokenS.Subject;
                    if (id != null)
                    {
                        this.IoTId = int.Parse(id);
                    }
                    else
                    {
                        this.IoTId = 0;
                    }
                }
                else
                {
                    this.IoTId = 0;
                }
            }

        }

        public int IoTId { get; private set; }
    }
}
