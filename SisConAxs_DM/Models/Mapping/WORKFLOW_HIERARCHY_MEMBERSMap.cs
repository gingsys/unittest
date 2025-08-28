using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_HIERARCHY_MEMBERSMap : EntityTypeConfiguration<WorkflowHierarchyMembers>
    {
        public WORKFLOW_HIERARCHY_MEMBERSMap()
        {
            // Primary Key
            this.HasKey(t => t.WfHierarchyMemberID);

            // Properties
            this.Property(t => t.WfHierarchyMemberID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WfApproveHierarchyID)
                .IsRequired();

            this.Property(t => t.WfHierarchyMemberDepartment)
                .IsRequired();

            this.Property(t => t.WfHierarchyMemberPosition)
                .IsRequired();

            this.Property(t => t.WfHierarchyMemberOrder)
                .IsRequired();

            this.Property(t => t.WfHierarchyMemberDescription)
                .HasMaxLength(200);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WORKFLOW_HIERARCHY_MEMBERS", "SITWF");
            this.Property(t => t.WfApproveHierarchyID).HasColumnName("WF_HIERARCHY_ID");
            this.Property(t => t.WfHierarchyMemberID).HasColumnName("WF_HIERARCHY_MEMBER_ID");
            this.Property(t => t.WfHierarchyMemberCompany).HasColumnName("WF_HIERARCHY_MEMBER_COMPANY");
            this.Property(t => t.WfHierarchyMemberDepartment).HasColumnName("WF_HIERARCHY_MEMBER_DEPARTMENT");
            this.Property(t => t.WfHierarchyMemberPosition).HasColumnName("WF_HIERARCHY_MEMBER_POSITION");
            this.Property(t => t.WfHierarchyMemberOrder).HasColumnName("WF_HIERARCHY_MEMBER_ORDER");
            this.Property(t => t.WfHierarchyMemberDescription).HasColumnName("WF_HIERARCHY_MEMBER_DESCRIPTION");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.WorkflowApproveHierarchy)
                .WithMany(t => t.WorkflowHierarchyMembers)
                .HasForeignKey(d => d.WfApproveHierarchyID);

            this.HasRequired(t => t.CommonValues)
                .WithMany(t => t.WorkflowHierarchyMembers)
                .HasForeignKey(d => d.WfHierarchyMemberDepartment);
            this.HasRequired(t => t.CommonValues1)
                .WithMany(t => t.WorkflowHierarchyMembers1)
                .HasForeignKey(d => d.WfHierarchyMemberPosition);
        }
    }
}
