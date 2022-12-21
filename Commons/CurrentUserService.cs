using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Persistences;

namespace backend.Commons
{
    public interface ICurrentUserService
    {
        int? UserId { get;}
        int? RoleId {get;}
        //roler int
        // int? instId {get;}
    }
    public class CurrentUserService : ICurrentUserService
    {
        private IHttpContextAccessor _httpContextAccessor;
        //private IHttpClientFactory _httpClientFactory;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor )
        {
            this._httpContextAccessor = httpContextAccessor;
            //this._httpClientFactory = httpClientFactory;
            //this.getUserInfo();
            var temp = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var tempRole = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

            UserId = int.Parse(temp??"0");
            RoleId = int.Parse(tempRole??"0");

            // var inst1 = this.context.Users.Where(x => x.Id == UserId).Select(x => x.Id).FirstOrDefault();
            // instId = inst1;
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
                    var roleid = tokenS.Claims.First(claim => claim.Type == "role").Value;
                    if (id != null && roleid != null)
                    {
                        this.UserId = int.Parse(id);
                        this.RoleId = int.Parse(roleid);
                    }
                    else
                    {
                        this.UserId = null;
                        this.RoleId = null;
                    }
                }
                else
                {
                    this.UserId = null;
                }
            }

        }

        public int? UserId { get; private set; }
        public int? RoleId {get; private set;}
        // public int? instId {get; private set;}

    }
}