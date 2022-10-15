using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Commons;

namespace backend.Model.AppEntity
{
    public class ParentParameter:Auditable{

        public ParentParameter(){
            this.Parameters = new HashSet<Parameter>();
        }
        public int Id {get;set;}
        public int PlantId{get;set;}
        public virtual Plant Plant{get;set;}
        public int ParentTypesId {get;set;}
        public virtual ParentType ParentTypes {get;set;}
        public virtual ICollection<Parameter> Parameters {get;set;}
    }  
}
