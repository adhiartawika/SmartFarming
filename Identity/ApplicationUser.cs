using backend.Commons;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Identity
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string Name { get; set; }
        public string Otp { get; set; }
        public DateTime? OtpExpired { get; set; }
    }
}
