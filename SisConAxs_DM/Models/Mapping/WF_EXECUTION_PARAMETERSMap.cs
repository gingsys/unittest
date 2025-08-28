using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SisConAxs_DM.Models;

namespace SisConAxs_DM.Models.Mapping
{
    public class WF_EXECUTION_PARAMETERSMap : EntityTypeConfiguration<WFExecutionParameters>
    {
        public WF_EXECUTION_PARAMETERSMap()
        {
            // Primary Key
            this.HasKey(t => t.WfExecParamID);

            // Properties
            this.Property(t => t.WfExecParamName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.WfExecParamStrValue)
                .HasMaxLength(2000);

            this.Property(t => t.CreateUser)
                .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WF_EXECUTION_PARAMETERS", "SITWF");
            this.Property(t => t.WfExecParamID).HasColumnName("WF_EXEC_PARAM_ID");
            this.Property(t => t.WfExecID).HasColumnName("WF_EXEC_ID");
            this.Property(t => t.WfExecParamName).HasColumnName("WF_EXEC_PARAM_NAME");
            this.Property(t => t.WfExecParamIntValue).HasColumnName("WF_EXEC_PARAM_INT_VALUE");
            this.Property(t => t.WfExecParamStrValue).HasColumnName("WF_EXEC_PARAM_STR_VALUE");
            this.Property(t => t.WfExecParamDateValue).HasColumnName("WF_EXEC_PARAM_DATE_VALUE");
            this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.WorkflowExecution)
                .WithMany(t => t.WFExecutionParameters)
                .HasForeignKey(d => d.WfExecID);

        }
    }
}
