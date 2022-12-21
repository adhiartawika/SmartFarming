using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class DiseaseImage
    {
        public int Id { get; set; } 
        public string Path {get;set;}
        public int? VirusMonitorId  {get;set;}
        public virtual VirusMonitor? VirusMonitor {get;set;}
    }
}
