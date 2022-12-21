using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Model.AppEntity;

namespace backend.Model.AppEntity
{
    public class Plant:Auditable
    {
        public Plant()
        {
            this.ParentParameters = new HashSet<ParentParameter>();
            this.Regions = new HashSet<Region>();
            // this.RegionPlants = new HashSet<RegionPlant>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public string Description {get; set; }
    
        public virtual ICollection<ParentParameter> ParentParameters { get; set; }
        // public virtual ICollection<RegionPlant> RegionPlants {get;set;}
        public virtual ICollection<Region> Regions {get;set;}

    }
}
