using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOWMap : EntityTypeConfiguration<Workflow>
    {
        public WORKFLOWMap()
        {
            // Primary Key
            this.HasKey(t => t.WfID);

            // Properties
            this.Property(t => t.WfName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.WfDescription)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WORKFLOWS", "SITWF");
            this.Property(t => t.WfID).HasColumnName("WF_ID");
            this.Property(t => t.WfApproveHierarchyID).HasColumnName("WF_HIERARCHY_ID");
            this.Property(t => t.WfName).HasColumnName("WF_NAME");
            this.Property(t => t.WfDescription).HasColumnName("WF_DESCRIPTION");
            this.Property(t => t.WfActivo).HasColumnName("WF_ACTIVO");
            this.Property(t => t.WfStartDate).HasColumnName("WF_START_DATE");
            this.Property(t => t.WfEndDate).HasColumnName("WF_END_DATE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            this.Property(t => t.WfCompany).HasColumnName("WF_COMPANY");

            // Relationships
            this.HasOptional(t => t.WorkflowApproveHierarchy)
                .WithMany(t => t.Workflow)
                .HasForeignKey(d => d.WfApproveHierarchyID);
        }
    }
}
