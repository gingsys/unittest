using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class EmpleadoSincronizacionDTO
    {
        public string IdEmpleado { get; set; }
        public string IdOrigen { get; set; }
        public string Origen { get; set; }
        public string IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string NumeroRuc { get; set; }
        public string NombrePaisNacimiento { get; set; }
        public string IdUnidadOrganizativa { get; set; }
        public string NombreUnidadOrganizativa { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NombreTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Genero { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string FechaNacimientoString { get; set; }
        public string Gerencia { get; set; }
        public string Ubigeo { get; set; }
        public string Direccion { get; set; }
        public string LugarTrabajo { get; set; }
        public string EstadoEmpleado { get; set; }
        public string NombreEstadoEmpleado { get; set; }
        public string TipoPersonal { get; set; }
        public string IdPuestoTrabajo { get; set; }
        public string PuestoTrabajo { get; set; }
        public string TelefonoTrabajo { get; set; }
        public string TelefonoPersonal { get; set; }
        public string CorreoPersonal { get; set; }
        public string IdCentroCosto { get; set; }
        public string NombreCentroCosto { get; set; }
        public string IdSubsidiaria { get; set; }
        public string NombreSubsidiaria { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaCese { get; set; }
        public string TipoConsulta { get; set; }
        public string EstadoRegistro { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string IdUnidadNegocio { get; set; }
        public string NombreUnidadNegocio { get; set; }
    }
}
