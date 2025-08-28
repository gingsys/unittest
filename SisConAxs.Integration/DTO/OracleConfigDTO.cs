using SisConAxs_DM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.DTO
{
    public class OracleConfigDTO
    {
        public string Address { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static OracleConfigDTO FromSysConfig(SystemConfigDTO dto)
        {
            return new OracleConfigDTO()
            {
                Address = dto.SysConfValue1,
                Username = dto.SysConfValue2,
                Password = dto.SysConfValue3
            };
        }
    }
}
