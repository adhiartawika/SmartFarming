using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Commons
{
    public class AppResponse
    {
        public string Message { get; set; }
    }
    public class CreateResponse<T>: AppResponse
    {
        public T Id { get; set; }
    }
    public class SearchRequest
    {
        public string? Search { get; set; }
        public int Page { get; set; }
        public int N { get; set; }
    }
    public class SearchResponse<T>
    {
        public ICollection<T> Data { get; set; }
        public int NTotal { get; set; }
    }
    
}
