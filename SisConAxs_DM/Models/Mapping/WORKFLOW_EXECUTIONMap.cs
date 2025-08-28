using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_EXECUTIONMap : EntityTypeConfiguration<WorkflowExecution>
    {
        public WORKFLOW_EXECUTIONMap()
        {
            // Primary Key
            this.HasKey(t => t.WfExecID);

            // Properties
            this.Property(t => t.WfExecStartedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.WfExecObjectName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.WfExecHistoryMessage)
                .IsRequired()
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("WORKFLOW_EXECUTION", "SITWF");
            this.Property(t => t.WfExecID).HasColumnName("WF_EXEC_ID");
            this.Property(t => t.WfExecWfID).HasColumnName("WF_EXEC_WF_ID");
            this.Property(t => t.WfExecCurrentStep).HasColumnName("WF_EXEC_CURRENT_STEP");
            this.Property(t => t.WfResponse).HasColumnName("WF_RESPONSE");
            this.Property(t => t.WfExecStartDate).HasColumnName("WF_EXEC_START_DATE");
            this.Property(t => t.WfExecStartedBy).HasColumnName("WF_EXEC_STARTED_BY");
            this.Property(t => t.WfExecParentObjectID).HasColumnName("WF_EXEC_PARENT_OBJECT_ID");
            this.Property(t => t.WfExecObjectID).HasColumnName("WF_EXEC_OBJECT_ID");
            this.Property(t => t.WfExecObjectName).HasColumnName("WF_EXEC_OBJECT_NAME");
            this.Property(t => t.WfExecStatus).HasColumnName("WF_EXEC_STATUS");
            this.Property(t => t.WfExecHistoryMessage).HasColumnName("WF_EXEC_HISTORY_MESSAGE");

            this.Property(t => t.WfExecCompany).HasColumnName("WF_EXEC_COMPANY");

            // Relationships
            this.HasRequired(t => t.Workflow)
                .WithMany(t => t.WorkflowExecution)
                .HasForeignKey(d => d.WfExecWfID);
            this.HasRequired(t => t.WorkflowItem)
                .WithMany(t => t.WorkflowExecution)
                .HasForeignKey(d => d.WfExecCurrentStep);

        }
    }
}
