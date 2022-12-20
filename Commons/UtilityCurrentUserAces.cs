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
    public interface IUtilityCurrentUserAces{
        int? instId {get;}
    }
    public class UtilityCurrentUserAces : IUtilityCurrentUserAces
    {
        private readonly AppDbContext context;
        private readonly ICurrentUserService currentUser;
        public UtilityCurrentUserAces(AppDbContext context, ICurrentUserService currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
            var user_in_instituted = this.context.Users.Where(x => this.currentUser.UserId == x.Id).Select(x => x.instituted.Id).FirstOrDefault();
            this.instId = user_in_instituted;
        }
        public int? instId {get; private set;}

    }
}