using AutoMapper;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;

namespace SisConAxs_DM.Repository
{
    public class EntityMappings
    {
        public void Initialize() {
            // Model to DTO ---------------------------------------------------------------------------------- //
            Mapper.CreateMap<Company, CompanyDTO>();
            Mapper.CreateMap<CompanyParameter, CompanyParameterDTO>();
            Mapper.CreateMap<Calendar, CalendarDTO>();

            Mapper.CreateMap<RequestTemplate, RequestTemplateDTO>();
            Mapper.CreateMap<RequestTemplateDetail, RequestTemplateDetailDTO>();
            Mapper.CreateMap<AccessRequests, AccessRequestDTO>();
            Mapper.CreateMap<AccessRequestDetails, AccessRequestDetailsDTO>();

            Mapper.CreateMap<AccessResources, AccessResourcesDTO>();
            Mapper.CreateMap<AccessResourceParameter, AccessResourceParameterDTO>()
                  .ForMember(dest => dest.ResourceParameterDisplay, m => m.MapFrom(s => s.ParameterName.CommonValueDisplay));
            Mapper.CreateMap<AccessTypes, AccessTypesDTO>();
            Mapper.CreateMap<AccessTypeValues, AccessTypeValuesDTO>();
            Mapper.CreateMap<AccessUsers, AccessUserDTO>()
                .ForMember(map => map.UserRoleSysAdmin, opt => opt.Ignore());
            Mapper.CreateMap<AccessUserCompanies, AccessUserCompanyDTO>();
            Mapper.CreateMap<ResourceCategories, ResourceCategoriesDTO>();
            Mapper.CreateMap<Metadata, MetadataDTO>();
            Mapper.CreateMap<CommonValueSets, CommonValueSetsDTO>();
            Mapper.CreateMap<CommonValues, CommonValuesDTO>();
            Mapper.CreateMap<Workflow, WorkflowDTO>()
                .ForMember(dest => dest.WfApproveHierarchyName, m => m.MapFrom(s => s.WorkflowApproveHierarchy.WfApproveHierarchyName))
                ;
            Mapper.CreateMap<WorkflowApproveHierarchy, WorkflowApproveHierarchyDTO>();
            Mapper.CreateMap<WorkflowHierarchyMembers, WorkflowHierarchyMembersDTO>();
            Mapper.CreateMap<WorkflowItems, WorkflowItemsDTO>()
                .ForMember(dest => dest.WfItemTypeName, m => m.MapFrom(s => s.CommonValues.CommonValueName))
                ;
            Mapper.CreateMap<WorkflowItemNext, WorkflowItemNextDTO>(); 
            Mapper.CreateMap<People, PeopleDTO>();            
            Mapper.CreateMap<SystemConfig, SystemConfigDTO>();
            Mapper.CreateMap<SystemConfig, NotifConfigDTO>()
                .ForMember(dest => dest.NotifConfID, m => m.MapFrom(s => s.SysConfID))
                .ForMember(dest => dest.NotifConfName, m => m.MapFrom(s => s.SysConfName))
                .ForMember(dest => dest.NotifConfDesc, m => m.MapFrom(s => s.SysConfDesc))
                .ForMember(dest => dest.NotifConfHost, m => m.MapFrom(s => s.SysConfValue1))
                .ForMember(dest => dest.NotifConfPort, m => m.MapFrom(s => s.SysConfValue2))
                .ForMember(dest => dest.NotifConfSSL, m => m.MapFrom(s => s.SysConfValue5))
                .ForMember(dest => dest.NotifConfUser, m => m.MapFrom(s => s.SysConfValue3))
                .ForMember(dest => dest.NotifConfLock, m => m.MapFrom(s => s.SysConfValue4));

            Mapper.CreateMap<IntegrationOracleLog, IntegrationOracleLogDTO>();
            Mapper.CreateMap<IntegrationSAPLog, IntegrationSAPLogDTO>();
            Mapper.CreateMap<IntegrationAADLog, IntegrationAADLogDTO>();

            // DTO to Model ---------------------------------------------------------------------------------- //
            Mapper.CreateMap<CompanyDTO, Company>();
            Mapper.CreateMap<CompanyParameterDTO, CompanyParameter>();
            Mapper.CreateMap<CalendarDTO, Calendar>();

            Mapper.CreateMap<RequestTemplateDTO, RequestTemplate>();
            Mapper.CreateMap<RequestTemplateDetailDTO, RequestTemplateDetail>();
            Mapper.CreateMap<AccessRequestDTO, AccessRequests>()
                .ForMember(map => map.ReqStatus, opt => opt.Ignore());
            Mapper.CreateMap<AccessRequestDetailsDTO, AccessRequestDetails>();

            Mapper.CreateMap<AccessResourcesDTO, AccessResources>();
            Mapper.CreateMap<AccessResourceParameterDTO, AccessResourceParameter>();
                  //.ForMember(map => map.ParameterName, opt => opt.Ignore());
            Mapper.CreateMap<AccessTypesDTO, AccessTypes>()
                .ForMember(map => map.AccessTypeValues, opt => opt.Ignore());
            Mapper.CreateMap<AccessTypeValuesDTO, AccessTypeValues>();
            Mapper.CreateMap<AccessUserDTO, AccessUsers>()
                .ForMember(map => map.UserRoleSysAdmin, opt => opt.Ignore());
            Mapper.CreateMap<AccessUserCompanyDTO, AccessUserCompanies>();
            Mapper.CreateMap<ResourceCategoriesDTO, ResourceCategories>();
            Mapper.CreateMap<MetadataDTO, Metadata>();
            Mapper.CreateMap<CommonValueSetsDTO, CommonValueSets>()
                .ForMember(map => map.CommonValues, opt => opt.Ignore())
                .ForMember(map => map.CommonValueSetSystemValue, opt => opt.Ignore())
                .ForMember(map => map.CommonValueSetRestrictedByCompany, opt => opt.Ignore());
            Mapper.CreateMap<CommonValuesDTO, CommonValues>();
            Mapper.CreateMap<WorkflowDTO, Workflow>()
                .ForMember(map => map.WorkflowItems, opt => opt.Ignore());
            Mapper.CreateMap<WorkflowApproveHierarchyDTO, WorkflowApproveHierarchy>()
                .ForMember(map => map.WorkflowHierarchyMembers, opt => opt.Ignore());
            Mapper.CreateMap<WorkflowHierarchyMembersDTO, WorkflowHierarchyMembers>();
            Mapper.CreateMap<WorkflowItemsDTO, WorkflowItems>()
                .ForMember(map => map.WorkflowItemNext, opt => opt.Ignore())
                .ForMember(map => map.WorkflowItemNextParents, opt => opt.Ignore());
            Mapper.CreateMap<WorkflowItemNextDTO, WorkflowItemNext>();
            Mapper.CreateMap<PeopleDTO, People>();
            Mapper.CreateMap<SystemConfigDTO, SystemConfig>();
            Mapper.CreateMap<NotifConfigDTO, SystemConfig>()
                .ForMember(dest => dest.SysConfID, m => m.MapFrom(s => s.NotifConfID))
                .ForMember(dest => dest.SysConfName, m => m.MapFrom(s => s.NotifConfName))
                .ForMember(dest => dest.SysConfDesc, m => m.MapFrom(s => s.NotifConfDesc))
                .ForMember(dest => dest.SysConfValue1, m => m.MapFrom(s => s.NotifConfHost))
                .ForMember(dest => dest.SysConfValue2, m => m.MapFrom(s => s.NotifConfPort))
                .ForMember(dest => dest.SysConfValue3, m => m.MapFrom(s => s.NotifConfUser))
                .ForMember(dest => dest.SysConfValue4, m => m.MapFrom(s => s.NotifConfLock))
                .ForMember(dest => dest.SysConfValue5, m => m.MapFrom(s => s.NotifConfSSL));

            Mapper.CreateMap<IntegrationOracleLogDTO, IntegrationOracleLog>()
                .ForMember(map => map.CreateDate, opt => opt.Ignore());
            Mapper.CreateMap<IntegrationSAPLogDTO, IntegrationSAPLog>()
                .ForMember(map => map.CreateDate, opt => opt.Ignore())
                .ForMember(map => map.CreateUser, opt => opt.Ignore());
            Mapper.CreateMap<IntegrationAADLogDTO, IntegrationAADLog>()
                .ForMember(map => map.CreateDate, opt => opt.Ignore())
                .ForMember(map => map.CreateUser, opt => opt.Ignore());

            // Copy objects
            Mapper.CreateMap<ResourcePeople, ResourcePeople>();
        }
    }
}
