
using backend.Commons;
namespace backend.Model.AppEntity
{
    public class ActuatorStatus:Auditable{
        public int Id {get;set;}
        public int ActuatorId {get; set;}
        public virtual Actuator Actuator {get;set;}
        public bool Status {get;set;}
    }
}
