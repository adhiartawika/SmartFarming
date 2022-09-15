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
    }

    public enum TypeSensor{
        ph = 0,
        moisture = 1,
        humidity = 2,
        servo = 3,
        camera = 4,
    }
}