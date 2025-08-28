using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO.Response
{
    public class ErrorResponseDTO
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Object Details { get; set; }
    }
}
