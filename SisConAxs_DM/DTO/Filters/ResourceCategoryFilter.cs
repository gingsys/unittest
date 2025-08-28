using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO.Filters
{
    public class ResourceCategoryFilter
    {
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
