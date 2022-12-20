using backend.Commons;
// using GreenHouseControlWebApp.Entities.IdEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Identity;

namespace backend.Model.AppEntity
{
    public class Instituted
    {
        public Instituted(){
            this.User = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Nama { get; set; }
        
        public string Alamat {get;set;}
        public ICollection<ApplicationUser> User {get;set;}
    }
}
