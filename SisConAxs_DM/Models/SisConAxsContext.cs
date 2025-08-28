using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SisConAxs_DM.Models.Mapping;

namespace SisConAxs_DM.Models
{
    public partial class SisConAxsContext : DbContext
    {
        static SisConAxsContext()
        {
            Database.SetInitializer<SisConAxsContext>(null);
        }

        public SisConAxsContext() : base("Name=SisConAxsContext")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyParameter> CompanyParameters { get; set; }
        public DbSet<AccessResources> AccessResources { get; set; }
        public DbSet<AccessResourceParameter> AccessResourceParameters { get; set; }
        public DbSet<RequestTemplate> RequestTemplate { get; set; }
        public DbSet<RequestTemplateDetail> RequestTemplateDetail { get; set; }
        public DbSet<AccessRequests> AccessRequests { get; set; }
        public DbSet<AccessRequestDetails> AccessRequestDetails { get; set; }
        public DbSet<AccessTypeValues> AccessTypeValues { get; set; }
        public DbSet<AccessTypes> AccessTypes { get; set; }
        public DbSet<AccessUsers> AccessUsers { get; set; }
        public DbSet<AccessUserCompanies> AccessUserCompanies { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<CommonValueSets> CommonValueSets { get; set; }
        public DbSet<CommonValues> CommonValues { get; set; }
        public DbSet<ResourceCategories> ResourceCategories { get; set; }
        public DbSet<ResourcePeople> ResourcePeople { get; set; }
        public DbSet<ResourcePeopleLog> ResourcePeopleLog { get; set; }
        //public DbSet<USER_OBJECT_ACCESS> USER_OBJECT_ACCESS { get; set; }
        public DbSet<WorkflowExecution> WorkflowExecution { get; set; }
        public DbSet<WorkflowExecutionHistory> WorkflowExecutionHistory { get; set; }
        public DbSet<WFExecutionParameters> WFExecutionParameters { get; set; }
        public DbSet<WorkflowApproveHierarchy> WorkflowApproveHierarchy { get; set; }
        public DbSet<WorkflowHierarchyMembers> WorkflowHierarchyMembers { get; set; }
        public DbSet<WorkflowItems> WorkflowItems { get; set; }
        public DbSet<WorkflowItemNext> WorkflowItemNext { get; set; }
        public DbSet<Workflow> Workflow { get; set; }
        public DbSet<People> People { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<SystemLog> SystemLog { get; set; }
        public DbSet<IntegrationOracleLog> IntegrationOracleLog { get; set; }
        public DbSet<IntegrationSAPLog> IntegrationSAPLog { get; set; }
        public DbSet<IntegrationAADLog> IntegrationAADLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new COMPANYMap());
            //modelBuilder.Configurations.Add(new COMPANY_PARAMETERSMap());
            modelBuilder.Configurations.Add(new ACCESS_RESOURCESMap());
            modelBuilder.Configurations.Add(new ACCESS_RESOURCE_PARAMETERSMap());
            modelBuilder.Configurations.Add(new REQUEST_TEMPLATEMap());
            modelBuilder.Configurations.Add(new REQUEST_TEMPLATE_DETAILSMap());
            modelBuilder.Configurations.Add(new ACCESS_REQUESTSMap());
            modelBuilder.Configurations.Add(new ACCESS_REQUEST_DETAILSMap());
            modelBuilder.Configurations.Add(new ACCESS_TYPE_VALUESMap());
            modelBuilder.Configurations.Add(new ACCESS_TYPESMap());
            modelBuilder.Configurations.Add(new ACCESS_USERSMap());
            modelBuilder.Configurations.Add(new ACCESS_USER_COMPANIESMap());
            modelBuilder.Configurations.Add(new METADATAMap());
            modelBuilder.Configurations.Add(new COMMON_VALUE_SETSMap());
            modelBuilder.Configurations.Add(new COMMON_VALUESMap());
            modelBuilder.Configurations.Add(new RESOURCE_CATEGORIESMap());
            modelBuilder.Configurations.Add(new RESOURCE_PEOPLEMap());
            modelBuilder.Configurations.Add(new RESOURCE_PEOPLE_LOGMap());
            //modelBuilder.Configurations.Add(new USER_OBJECT_ACCESSMap());
            modelBuilder.Configurations.Add(new WORKFLOW_APPROVE_HIERARCHYMap());
            modelBuilder.Configurations.Add(new WORKFLOW_HIERARCHY_MEMBERSMap());
            modelBuilder.Configurations.Add(new WORKFLOW_EXECUTIONMap());
            modelBuilder.Configurations.Add(new WORKFLOW_EXECUTION_HISTORYMap());
            modelBuilder.Configurations.Add(new WF_EXECUTION_PARAMETERSMap());
            modelBuilder.Configurations.Add(new WORKFLOW_ITEMSMap());
            modelBuilder.Configurations.Add(new WORKFLOW_ITEM_NEXTMap());
            modelBuilder.Configurations.Add(new WORKFLOWMap());
            modelBuilder.Configurations.Add(new PEOPLEMap());
            modelBuilder.Configurations.Add(new SYSTEM_CONFIGMap());
            modelBuilder.Configurations.Add(new CALENDARMap());
            modelBuilder.Configurations.Add(new SYSTEM_LOGMap());
            modelBuilder.Configurations.Add(new INTEGRATION_ORACLE_LOGMap());
            modelBuilder.Configurations.Add(new INTEGRATION_SAP_LOGMap());
            modelBuilder.Configurations.Add(new INTEGRATION_AAD_LOGMap());
        }
    }
}
