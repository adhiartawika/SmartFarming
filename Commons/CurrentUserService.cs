using Microsoft.AspNetCore.Authentication;
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
    public interface ICurrentUserService
    {
        int? UserId { get; }
    }
    public class CurrentUserService : ICurrentUserService
    {
        private IHttpContextAccessor _httpContextAccessor;
        //private IHttpClientFactory _httpClientFactory;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            //this._httpClientFactory = httpClientFactory;
            //this.getUserInfo();
            var temp = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            UserId = int.Parse(temp??"0");
        }

        private async Task getUserInfo()
        {
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
            {
                //var accessToken = await _httpContextAccessor?.HttpContext?.GetTokenAsync("access_token");
                var accessToken = _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "app-authorization").Value;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken);
                    var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
                    //var id = tokenS.Claims.First(claim => claim.Type == "sub").Value;
                    var id = tokenS.Subject;
                    if (id != null)
                    {
                        this.UserId = int.Parse(id);
                    }
                    else
                    {
                        this.UserId = null;
                    }
                }
                else
                {
                    this.UserId = null;
                }
            }

        }

        public int? UserId { get; private set; }
    }
}
