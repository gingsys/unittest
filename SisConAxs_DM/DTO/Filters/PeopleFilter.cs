using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO.Filters
{
    public class PeopleFilter
    {
        public string PeopleInternalID { get; set; }
        public string PeopleFullLastName { get; set; }
        public string PeopleFullFirstName { get; set; }
        public string PeopleDocTypeName { get; set; }

        public int? PeopleTypeClasificacion { get; set; }
        public string PeopleTypeClasificacionName { get; set; }
        public string PeopleEmployeeTypeName { get; set; }
        public string PeopleDocNum { get; set; }
        public string PeopleEmail { get; set; }
        public string PeopleDepartmentName { get; set; }
        public string PeoplePositionName { get; set; }
        public string PeopleCompanyName { get; set; }
        public string PeopleStatusDesc { get; set; }
        //public string filterText { get; set; }
        public int status { get; set; } = -1;
        public bool allCompanies { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
