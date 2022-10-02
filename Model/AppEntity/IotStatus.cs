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
        public int? MicroControllerId { get; set; }
        public virtual Mikrokontroller MicroController { get; set; }
        public int? MiniPcId { get; set; }
        public virtual MiniPc MiniPc { get; set; }
        public bool IsActive { get; set; }
    }
}
