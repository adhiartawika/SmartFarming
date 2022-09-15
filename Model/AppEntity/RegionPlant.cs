using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class RegionPlant:Auditable{

        public int Id {get;set;}

        public int RegionId {get;set;}

        public virtual Region Region {get; set;}

        public int PlantId {get; set;}

        public virtual Plant Plant {get;set;}
        
    }
}