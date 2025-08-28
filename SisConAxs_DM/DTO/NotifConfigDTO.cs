using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public partial class NotifConfigDTO
    {
        public NotifConfigDTO() {
            this.NotifConfName = "NOTIFICATION_CONFIG";
            this.NotifConfDesc = "Configuración de Correo para envío de Notificaciones.";
        }

        public int NotifConfID { get; set; }
        public string NotifConfName { get; set; }
        public string NotifConfDesc { get; set; }
        public string NotifConfHost { get; set; }
        public string NotifConfPort { get; set; }
        public string NotifConfSSL { get; set; }
        public string NotifConfUser { get; set; }
        public string NotifConfLock { get; set; }

    }
}
