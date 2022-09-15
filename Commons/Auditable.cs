using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Commons
{
    public abstract class TimeStampDataAuditable
    {
        public DateTime CreatedAt { get; set; }
    }
    
    public abstract class TimeStampData{
        public DateTime CreatedAt { get; set; }
    }
    public abstract class TimeStampDataIot:TimeStampData
    {
        
        public int IdIoT { get; set; }
        
    }
    public abstract class Auditable
    {
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public int? DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
