using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.DTO
{
    public class MetadataDTO
    {
        public const int ORACLE_COMPANIES = 10000;
        public const int ORACLE_PROJECTS = 10001;
        public const int ORACLE_PROFILES = 10002;
        public const int ORACLE_RESPONSABILITIES = 10003;

        public int MetadataID { get; set; }
        public Nullable<int> MetadataParentID { get; set; }
        public string MetadataDisplay { get; set; }
        public string MetadataDescription { get; set; }
        //public int MetadataInt1 { get; set; }
        public string MetadataInt1 { get; set; }
        public int MetadataInt2 { get; set; }
        public int MetadataInt3 { get; set; }
        public int MetadataInt4 { get; set; }
        public int MetadataInt5 { get; set; }
        public string MetadataStr1 { get; set; }
        public string MetadataStr2 { get; set; }
        public string MetadataStr3 { get; set; }
        public string MetadataStr4 { get; set; }
        public string MetadataStr5 { get; set; }
        public Nullable<DateTime> MetadataDatetime1 { get; set; }
        public Nullable<DateTime> MetadataDatetime2 { get; set; }
        public Nullable<DateTime> MetadataDatetime3 { get; set; }
        public int MetadataActive { get; set; } = 1;

        public bool Equals(MetadataDTO to, params string[] ignore)
        {
            MetadataDTO self = this;
            if (self != null && to != null)
            {
                Type type = typeof(MetadataDTO);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }
    }
}
