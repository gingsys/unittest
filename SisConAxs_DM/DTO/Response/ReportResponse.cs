using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO.Response
{
    public class ReportResponse<T>
    {
        public List<T> Rows { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>(); // información adicional
    }
}
