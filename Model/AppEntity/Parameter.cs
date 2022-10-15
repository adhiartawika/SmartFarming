using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Commons;

namespace backend.Model.AppEntity
{
    public class Parameter:Auditable
    {
        public Parameter()
        {
        }
        public int Id { get; set; }
        public int ParentParamId {get;set;}
        public virtual ParentParameter ParentParam {get;set;}
        public string Description { get; set; }
        public double MinValue {get;set;}
        public double MaxValue {get;set;}
        public string Color {get;set;}

    }
}