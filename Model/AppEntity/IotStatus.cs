using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class IotStatus: TimeStampData
    {
        public IotStatus()
        {
            
        }
        
        public int Id { get; set; }
        public int MikrokontrollerId { get; set; }
        public virtual Mikrokontroller Mikrokontroller{ get; set; }
        public bool IsActive { get; set; }
        public int IdIoT { get; set; }
    }
}
