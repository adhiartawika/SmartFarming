using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Mikrokontroller:Auditable
    {
        public Mikrokontroller()
        {
            this.IotStatus = new HashSet<IotStatus>();
            this.Sensor = new HashSet<Sensor>();
        }
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public int IotId { get; set; }
        public virtual ICollection<Sensor> Sensor{get; set;}
        public virtual ICollection<IotStatus> IotStatus { get; set; }
    }
}
