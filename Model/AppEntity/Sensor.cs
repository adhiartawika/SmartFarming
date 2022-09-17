using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Sensor{
        public int Id {get;set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public TypeSensor Type {get; set;}

        public int MikrocontrollerId {get; set;}
        public virtual Mikrokontroller MikroController {get; set;}
    }

    public enum TypeSensor{
        ph = 0,
        Soilmoisture = 1,

        Airmoisture = 2,
        Soiltemperature = 3,

        AirTemperature = 4,
    }
}