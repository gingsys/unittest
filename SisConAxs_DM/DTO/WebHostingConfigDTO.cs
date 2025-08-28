using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class WebHostingConfigDTO
    {
        public WebHostingConfigDTO() {
            this.NotifConfName = "WEBHOSTING_CONFIG";
            this.NotifConfDesc = "Configuración del Servidor Web.";
        }

        public int WebHostingConfID { get; set; }
        public string NotifConfName { get; set; }
        public string NotifConfDesc { get; set; }
        public string WebHostingURL { get; set; }
        public string WebHostingPort { get; set; }
    }
}
