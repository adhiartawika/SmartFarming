using backend.Commons;
// using GreenHouseControlWebApp.Entities.IdEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Land: Auditable
    {
        public Land()
        {
            this.UserDevices = new HashSet<UserDevice>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public Byte[] Photo { get; set; }
        public string CordinateLand {get; set;}
        // public int RegionId {get;set;}
        // public virtual Region Region {get;set;}
        public virtual ICollection<Region> Region{ get; set; }
        public virtual ICollection<UserDevice> UserDevices { get; set; }
    }
}
