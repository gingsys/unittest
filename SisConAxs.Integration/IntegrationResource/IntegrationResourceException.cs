using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public class IntegrationResourceException : Exception
    {
        public const int ERROR_RESOURCE_WITHOUT_EXTERNAL_ID = -1;  // Si el recurso no tiene un ID Externo del Servicio asociado
        public const int ERROR_CLIENT_VALIDATION = -2;             // Error en la validación de los datos a enviar
        public const int ERROR_SERVER_VALIDATION = -3;             // Error en la validación de los datos desde el servidor
        public const int ERROR_CONNECTION = -4;                    // Error en la conexión
        public const int ERROR_TIMEOUT = -5;                       // Error excedio tiempo de espera
        public const int ERROR_SERVER = -10;                       // Error desde el servidor
        public const int ERROR_OTHER = -20;                        // Error otros

        public int Code { get; set; }
        //public IntegrationResourceException() : base() { }
        public IntegrationResourceException(int code, string message) : base(message) {
            this.Code = code;
        }
        public IntegrationResourceException(int code, string message, Exception innerException) : base(message, innerException) {
            this.Code = code;
        }
    }
}
