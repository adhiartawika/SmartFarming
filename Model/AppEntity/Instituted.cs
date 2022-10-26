using backend.Commons;
// using GreenHouseControlWebApp.Entities.IdEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Instituted: Auditable
    {
        public int Id { get; set; }
        public int Nama { get; set; }
        public string LandId { get; set; }
        public virtual Land Land {get; set;}
        public string PlantId { get; set; }
        public virtual Plant Plant {get;set;}
    }
}
