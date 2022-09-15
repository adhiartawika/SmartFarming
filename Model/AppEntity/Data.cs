using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Data: TimeStampData
    {
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public virtual Parameter Parameter { get; set; }
        public  decimal ValueParameter { get; set; }
    }
}
