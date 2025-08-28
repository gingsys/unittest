using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models
{
    public class IntegrationSAPLog
    {
        public int LogID { get; set; }
        public int LogType { get; set; }          // ALTA, BAJA, MODIFICACION <- valores vienen de COMMON_VALUES
        public string LogResult { get; set; }     // EJECUCIÓN COMPLETA / ERROR / SIN CAMBIOS (No tiene recursos, usuario ni persona) / EJECUCIÓN PARCIAL

        public DateTime LogTypeDate { get; set; }
        public string LogDocNumber { get; set; }
        public string LogNames { get; set; }
        public string LogLastnames { get; set; }
        public string LogCompanyName { get; set; }
        public string LogMessage { get; set; }
        //public bool LogLastChange { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
