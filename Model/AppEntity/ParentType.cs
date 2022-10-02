using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Commons;

namespace backend.Model.AppEntity
{
    public class ParentType{

      public ParentType()
        {
            this.Parameters = new HashSet<Parameter>();
            // this.RegionPlants = new HashSet<RegionPlant>();
        }
        public int Id {get;set;}
        public string Name {get;set;}

        public string? Description {get;set;}

        public virtual ICollection<Parameter> Parameters{get;set;} 
    }
}
