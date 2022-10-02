using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Model.IdEntity;

namespace backend.Model.AppEntity
{
    
    public class MiniPc:Auditable
    {
        public MiniPc()
        {
            this.IotStatus = new HashSet<IotStatus>();
            // this.Sensor = new HashSet<Sensor>();
            this.Mikrokontrollers = new HashSet<Mikrokontroller>();
            // this.IdIoTs = new HashSet<IdIoT>();
        }
        public int Id { get; set; }
        public string Name {get; set;}
        public string Description {get; set;}
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public string Secret {get;set;}
        public string Code {get; set;}
        public virtual ICollection<IdIoT> IdIoTs {get;set;}
        public virtual ICollection<Mikrokontroller> Mikrokontrollers{get; set;}
        public virtual ICollection<IotStatus> IotStatus { get; set; }
    }
}
