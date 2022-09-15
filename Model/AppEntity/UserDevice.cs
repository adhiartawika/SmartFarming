using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class UserDevice : TimeStampDataAuditable
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LandId { get; set; }
        public Land Land { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
        public string Os { get; set; }
        public string DeviceKey { get; set; }
    }
}
