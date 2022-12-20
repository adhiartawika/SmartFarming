using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Commons;

namespace backend.Model.AppEntity
{
    public class VirusMonitor:Auditable{
        
        public VirusMonitor(){
            this.LanLatDiseases = new HashSet<LanLatDiseases>();
        }
        public int id {get;set;}
        public int VirusId {get;set;}
        public virtual PlantVirus Virus {get;set;}
        public int RegionId {get;set;}
        public virtual Region Region {get;set;}
        public int MonitorStatusId {get;set;}
        public virtual MonitorStatus MonitorStatus {get;set;}
        public string? ImageDisease { get; set; }
        public ICollection<LanLatDiseases>? LanLatDiseases {get;set;}
    }
}