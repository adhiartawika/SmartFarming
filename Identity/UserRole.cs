using backend.Commons;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Identity
{
    public class UserRole
    {
        [Required]
        public int IdRole {get;set;}
        public string RoleName {get;set;}
    }
}