
using backend.Commons;

namespace backend.Model.AppEntity
{

    public class Actuator:Auditable{

        public Actuator(){
            this.Status = new HashSet<ActuatorStatus>();
        }
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int? MikrocontrollerId {get; set;}
        public virtual Mikrokontroller MikroController {get; set;}
        // public int? MiniPcId {get; set;}
        // public virtual MiniPc MiniPc {get; set;}
        public int ActuatorTypeId {get;set;}
        public virtual TypeActuators ActuatorType {get;set;}

        public virtual ICollection<ActuatorStatus> Status {get;set;}
    }
}

