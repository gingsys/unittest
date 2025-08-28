using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_APPROVE_HIERARCHYMap : EntityTypeConfiguration<WorkflowApproveHierarchy>
    {
        public WORKFLOW_APPROVE_HIERARCHYMap()
        {
            // Primary Key
            this.HasKey(t => t.WfApproveHierarchyID);

            // Properties
            this.Property(t => t.WfApproveHierarchyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WfApproveHierarchyName)
                .IsRequired()
                .HasMaxLength(200);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WORKFLOW_APPROVE_HIERARCHY", "SITWF");
            this.Property(t => t.WfApproveHierarchyID).HasColumnName("WF_HIERARCHY_ID");
            this.Property(t => t.WfApproveHierarchyName).HasColumnName("WF_HIERARCHY_NAME");
            this.Property(t => t.WfApproveHierarchyDepartment).HasColumnName("WF_HIERARCHY_DEPARTMENT");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            this.Property(t => t.WfApproveHierarchyCompany).HasColumnName("WF_HIERARCHY_COMPANY");

            this.HasOptional(t => t.Department)
                .WithMany(t => t.WorkflowApproveHierarchy)
                .HasForeignKey(d => d.WfApproveHierarchyDepartment);
        }
    }
}
