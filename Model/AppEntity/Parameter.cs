using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.AppEntity
{
    public class Parameter
    {
        public Parameter()
        {
        }
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public int PlantId{get;set;}
        public virtual Plant Plant{get;set;}
    }
}