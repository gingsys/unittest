using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs.Integration.DTO
{
    public class OracleCompanyDTO
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public MetadataDTO ToDTO()
        {
            return new MetadataDTO()
            {
                MetadataParentID = MetadataDTO.ORACLE_COMPANIES,
                MetadataStr1 = CompanyID.Trim(),
                MetadataDisplay = CompanyName.Trim(),
                MetadataActive = 1
            };
        }
    }
    public class OracleProjectDTO
    {
        //public int ProjectID { get; set; }
        public string ProjectID { get; set; }

        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCompanyID { get; set; }
        public MetadataDTO ToDTO()
        {
            return new MetadataDTO()
            {
                MetadataParentID = MetadataDTO.ORACLE_PROJECTS,
                MetadataInt1 = ProjectID,
                MetadataStr1 = ProjectCode?.Trim(),
                MetadataStr2 = ProjectCompanyID.Trim(),
                MetadataDisplay = ProjectName.Trim(),
                MetadataActive = 1
            };
        }
    }
    public class OracleProfileDTO
    {
        //public int ProfileID { get; set; }
        public string ProfileID { get; set; }

        public string ProfileCode { get; set; }
        public string ProfileName { get; set; }
        public string ProfileMenuID { get; set; }
        public MetadataDTO ToDTO()
        {
            return new MetadataDTO()
            {
                MetadataParentID = MetadataDTO.ORACLE_PROFILES,
                MetadataInt1 = ProfileID,
                MetadataStr1 = ProfileCode?.Trim(),
                MetadataDisplay = ProfileName.Trim(),
                MetadataActive = 1
            };
        }
    }
    public class OracleResponsabilityDTO
    {
        //public int ResponsabilityID { get; set; }
        public string ResponsabilityID { get; set; }
        public string ResponsabilityName { get; set; }
        public string ResponsabilityCompanyID { get; set; }
        public Nullable<int> ResponsabilityProjectID { get; set; }
        public int ResponsabilityProfileID { get; set; }
        public MetadataDTO ToDTO()
        {
            return new MetadataDTO()
            {
                MetadataParentID = MetadataDTO.ORACLE_RESPONSABILITIES,
                MetadataInt1 = ResponsabilityID,
                MetadataStr1 = ResponsabilityCompanyID.Trim(),
                MetadataInt2 = ResponsabilityProjectID ?? 0,
                MetadataInt3 = ResponsabilityProfileID,
                MetadataDisplay = ResponsabilityName.Trim(),
                MetadataActive = 1
            };
        }
    }
    public class OracleAccessPeopleDTO
    {
        public string AccessUserCode { get; set; }
        public string AccessUserDescription { get; set; }
        //public int AccessResponsabilityID { get; set; }
        public string AccessResponsabilityID { get; set; }

        public Nullable<DateTime> AccessStartDate { get; set; }
        public Nullable<DateTime> AccessEndDate { get; set; }
    }


    public class AccessResourcesOracleDTO : AccessResourcesDTO
    {
        public string ResourceOracleID { get; set; }
    }

    // Query Classes ---------------------------------------------------------------------------------------------
    public class OracleQueryAccessResource
    {
        public PeopleDTO People { get; set; }
        public OracleAccessPeopleDTO Source { get; set; }
        public AccessResourcesDTO Resource { get; set; }
    }
    public class OracleQueryResourcePeople
    {
        //public int OracleResponsabilityID { get; set; }
        public string OracleResponsabilityID { get; set; }

        public string OracleCompanyID { get; set; }
        public string PeopleInternalID { get; set; }
        public People People { get; set; }
        public AccessResources Resource { get; set; }
        public ResourcePeople ResourcePeople { get; set; }
    }
}
