using backend.Commons;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Identity
{
    public class UserRole:IdentityRole<int>
    {
        public UserRole(){
            this.User = new HashSet<ApplicationUser>();
        }
        public string RoleName { get; set; }
        public ICollection<ApplicationUser> User {get;set;}
    }
}