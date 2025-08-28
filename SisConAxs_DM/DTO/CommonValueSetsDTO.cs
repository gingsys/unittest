using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public class CommonValueSetsDTO
    {
        public const int SET_TIPO_DOCUMENTO = 10;
        public const int SET_TIPO_SOLICITUD = 18;
        public const int SET_PRIORIDAD_SOLICITUD = 19;
        public const int SET_AREAS = 13;
        public const int SET_PROYECTOS = 26;
        public const int SET_POSICIONES = 12;
        public const int SET_PAISES = 27;
        public const int SET_TIPO_CLASIFICACION = 29;
        public const int SET_TIPO_EMPLEADO = 30;

        public int CommonValueSetID { get; set; }
        public string CommonValueSetName { get; set; }
        public string CommonValueSetDesc { get; set; }

        public ICollection<CommonValuesDTO> CommonValues { get; set; }
    }
}
