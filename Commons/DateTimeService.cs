using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
