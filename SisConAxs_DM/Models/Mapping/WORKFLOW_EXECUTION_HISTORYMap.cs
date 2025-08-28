using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SisConAxs_DM.Models;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_EXECUTION_HISTORYMap : EntityTypeConfiguration<WorkflowExecutionHistory>
    {
        public WORKFLOW_EXECUTION_HISTORYMap()
        {
            // Primary Key
            this.HasKey(t => t.WfExecHistoryID);

            // Properties
            this.Property(t => t.WfExecHistoryID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

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
            this.ToTable("WORKFLOW_EXECUTION_HISTORY", "SITWF");
            this.Property(t => t.WfExecHistoryID).HasColumnName("WF_EXEC_HISTORY_ID");
            this.Property(t => t.WfExecID).HasColumnName("WF_EXEC_ID");
            this.Property(t => t.WfExecWfID).HasColumnName("WF_EXEC_WF_ID");
            this.Property(t => t.WfExecCurrentStep).HasColumnName("WF_EXEC_CURRENT_STEP");
            this.Property(t => t.WfResponse).HasColumnName("WF_RESPONSE");
            this.Property(t => t.WfExecStartDate).HasColumnName("WF_EXEC_START_DATE");
            this.Property(t => t.WfExecStartedBy).HasColumnName("WF_EXEC_STARTED_BY");
            this.Property(t => t.WfExecParentObject).HasColumnName("WF_EXEC_PARENT_OBJECT");
            this.Property(t => t.WfExecObjectID).HasColumnName("WF_EXEC_OBJECT_ID");
            this.Property(t => t.WfExecObjectName).HasColumnName("WF_EXEC_OBJECT_NAME");
            this.Property(t => t.WfExecObjectStatus).HasColumnName("WF_EXEC_OBJECT_STATUS");
            this.Property(t => t.WfExecHistoryMessage).HasColumnName("WF_EXEC_HISTORY_MESSAGE");
            this.Property(t => t.WfExecApproverName).HasColumnName("WF_EXEC_APPROVER_NAME");
            this.Property(t => t.WfExecExecutorMail).HasColumnName("WF_EXEC_EXECUTOR_MAIL");
            this.Property(t => t.wfExecApproverID).HasColumnName("WF_EXEC_APPROVER_ID");
            this.Property(t => t.WfExecApproverArea).HasColumnName("WF_EXEC_APPROVER_AREA");
            this.Property(t => t.WfExecApproverPosition).HasColumnName("WF_EXEC_APPROVER_POSITION");

            this.Property(t => t.WfExecCompany).HasColumnName("WF_EXEC_COMPANY");

            // Relationships
            this.HasRequired(t => t.Workflow)
                .WithMany(t => t.WorkflowExecutionHistory)
                .HasForeignKey(d => d.WfExecWfID);
            this.HasOptional(t => t.WorkflowItems)
                .WithMany(t => t.WorkflowExecutionHistory)
                .HasForeignKey(d => d.WfExecCurrentStep);

        }
    }
}
