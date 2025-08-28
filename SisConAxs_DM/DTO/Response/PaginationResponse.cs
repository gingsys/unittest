using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO.Response
{
    public class PaginationResponse<T>
    {
        public List<T> Rows { get; set; } = new List<T>();
        public int TotalRows { get; set; }
        public Dictionary<string, object> Additional { get; set; } = new Dictionary<string, object>();
    }
}
