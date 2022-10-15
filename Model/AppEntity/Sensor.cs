using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Commons;

namespace backend.Model.AppEntity
{
    public class Sensor:Auditable{
        public Sensor()
        {
            this.Datas = new HashSet<Data>();
            // this.RegionPlants = new HashSet<RegionPlant>();
        }
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int? MikrocontrollerId {get; set;}
        public virtual Mikrokontroller MikroController {get; set;}
        // public int? MiniPcId {get; set;}
        // public virtual MiniPc MiniPc {get; set;}

        public virtual ICollection<Data> Datas { get; set; }
        public int ParentTypeId {get;set;}
        public virtual ParentType ParentType {get;set;}
        public int ParameterId {get; set;}
        public virtual Parameter Parameter {get;set;}
    }

}