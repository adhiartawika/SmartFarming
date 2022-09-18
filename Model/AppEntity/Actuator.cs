
using backend.Commons;

namespace backend.Model.AppEntity
{

    public class Actuator:Auditable{
        public int Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public Typeactuator Type {get; set;}
        public int MikrocontrollerId {get; set;}
        public virtual Mikrokontroller MikroController {get; set;}
    }
    public enum Typeactuator{

            Servo = 0,

            Waterpump = 1,
            
            Camera = 2,
    }
}

