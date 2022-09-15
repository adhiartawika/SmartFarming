using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Region: Auditable
    {
        public Region()
        {
            this.RegionPlant = new HashSet<RegionPlant>();
            this.Mikrokontroller = new HashSet<Mikrokontroller>();
        }
        public int Id{get;set;}
        public string Name{get;set;}
        public string RegionDescription {get;set;}
        public string CordinateRegion {get;set;}
        public int  LandId { get; set; }
        public virtual Land Land { get; set; }  
        public virtual ICollection<RegionPlant> RegionPlant { get; set; }
        public virtual ICollection<Mikrokontroller> Mikrokontroller { get; set; }
    }
}