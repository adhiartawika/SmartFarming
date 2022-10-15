using backend.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Model.AppEntity;
namespace backend.Model.IdEntity
{
    public class IdIoT:Auditable
    {
        
        public IdIoT()
        {
            this.MiniPcs = new HashSet<MiniPc>();
        }
        public int Id { get; set; }
        public string Name { get; set; } //name dari minipcs
        public string Code { get; set; } //auto degenerate dari frontend
        public string? Description {get; set;}
        public string Secret { get; set; } //secret generate dari frontend

        public ICollection<MiniPc> MiniPcs {get;set;}
    }
}
