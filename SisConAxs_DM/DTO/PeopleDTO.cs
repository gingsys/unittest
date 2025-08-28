using System;
using System.Collections.Generic;

namespace SisConAxs_DM.DTO
{
    public partial class PeopleDTO
    {
        public const int DOC_TYPE_DNI = 1;
        public const int DOC_TYPE_PASAPORTE = 2;
        public const int DOC_TYPE_CE = 3;
        public const int DOC_TYPE_DIFERENTE = 4;
        public const int DOC_TYPE_RUT = 682;
        public const int DOC_TYPE_RUC = 1430;
        public const int DOC_TYPE_CEDULA = 1458;

        public const int CLAS_TYPE_COLABORADOR = 2655;
        public const int CLAS_TYPE_PROVEEDOR = 2656;
        public const int CLAS_TYPE_TERCERO = 2657;
        public const int CLAS_TYPE_COLABORADOR_EN_PROCESO = 2829;
        public const int CLAS_TYPE_CLIENTE = 2830;

        public int PeopleID { get; set; }
        public int PeopleOrgID { get; set; }
        public string PeopleInternalID { get; set; }
        public string PeopleLastName { get; set; }
        public string PeopleFirstName { get; set; }
        public string PeopleLastName2 { get; set; }
        public string PeopleFirstName2 { get; set; }
        public Nullable<int> PeopleDocType { get; set; }

        public Nullable<int> PeopleTypeClasificacion { get; set; }
        public string PeopleTypeClasificacionName { get; set; }
        public Nullable<int> PeopleEmployeeType { get; set; }
        public string PeopleEmployeeTypeName { get; set; }

        public string PeopleDocNum { get; set; }
        public string PeopleAddress1 { get; set; }
        public string PeopleAddress2 { get; set; }
        public string PeoplePhone1 { get; set; }
        public string PeoplePhone2 { get; set; }
        public string PeopleEmail { get; set; }
        public Nullable<DateTime> PeopleBirthday { get; set; }
        public string PeopleGender { get; set; }
        public Nullable<int> PeopleDepartment { get; set; }
        public Nullable<int> PeoplePosition { get; set; }
        public string UserID { get; set; }
        public string UserInternalID { get; set; }

        public bool PeopleIsSourceSAP { get; set; } = false;
        public DateTime? PeopleStartDate { get; set; }
        public int PeopleStatus { get; set; }

        public string PeopleFullname
        {
            get
            {
                return (this.PeopleLastName + " " +
                        this.PeopleLastName2 + ", " +
                        this.PeopleFirstName + " " +
                        this.PeopleFirstName2).Trim();
            }
        }

        public string PeopleDocTypeName { get; set; }
        public string PeopleDepartmentName { get; set; }
        public string PeoplePositionName { get; set; }
        public int ForApprove { get; set; }
        public List<string> AssignedItems { get; set; }
        public List<string> PendingApproveItems { get; set; }
        //public System.Data.Entity.Spatial.DbGeography PeopleAddressGeolocation { get; set; }
        //public byte[] PeopleImage { get; set; }
        //public string PeopleAttribute1 { get; set; }
        public string PeopleAttribute2 { get; set; }
        public int? PeopleProject { get; set; }
        public string PeopleAttribute3 { get; set; }
        //public string PeopleAttribute4 { get; set; }
        //public string PeopleAttribute5 { get; set; }
        //public string PeopleAttribute6 { get; set; }
        //public string PeopleAttribute7 { get; set; }
        //public string PeopleAttribute8 { get; set; }
        //public string PeopleAttribute9 { get; set; }
        //public string PeopleAttribute10 { get; set; }

        public int PeopleCompany { get; set; }
        public string PeopleCompanyName { get; set; }

        public string PeopleOrdinalNames
        {
            get { return PeopleFirstName + " " + PeopleLastName; }
        }

        public string PeopleCommonNames
        {
            get { return PeopleOrdinalNames + " " + PeopleLastName2; }
        }
        public string PeopleStatusDesc { get; set; }
        public string PeopleFullFirstName { get; set; }
        public string PeopleFullLastName { get; set; }


        //Campo usado para la validación de carga masiva por excel
        public bool Failed { get; set; }
        public string FailedDescription { get; set; }
    }
}
