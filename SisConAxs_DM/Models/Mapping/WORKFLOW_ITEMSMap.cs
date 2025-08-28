using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_ITEMSMap : EntityTypeConfiguration<WorkflowItems>
    {
        public WORKFLOW_ITEMSMap()
        {
            // Primary Key
            this.HasKey(t => t.WfItemId);

            // Properties
            this.Property(t => t.WfItemId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WfItemName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.WfItemSubject)
                .HasMaxLength(2000);

            this.Property(t => t.WfItemEnterCondition)
                .HasMaxLength(50);

            this.Property(t => t.WfItemEnterParams)
                .HasMaxLength(2000);

            this.Property(t => t.WfItemExitValues)
                .HasMaxLength(50);

            this.Property(t => t.WfItemDestMail)
                .HasMaxLength(2000);

            this.Property(t => t.WfItemCcMail)
                .HasMaxLength(2000);
            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WORKFLOW_ITEMS", "SITWF");
            this.Property(t => t.WfItemId).HasColumnName("WF_ITEM_ID");
            this.Property(t => t.WfItemWfID).HasColumnName("WF_ITEM_WF_ID");
            this.Property(t => t.WfItemName).HasColumnName("WF_ITEM_NAME");
            this.Property(t => t.WfItemType).HasColumnName("WF_ITEM_TYPE");
            this.Property(t => t.WfItemSubject).HasColumnName("WF_ITEM_SUBJECT");
            this.Property(t => t.WfItemMessage).HasColumnName("WF_ITEM_MESSAGE");
            this.Property(t => t.WfItemStep).HasColumnName("WF_ITEM_STEP");
            this.Property(t => t.WfItemEnterCondition).HasColumnName("WF_ITEM_ENTER_CONDITION");
            this.Property(t => t.WfItemEnterParams).HasColumnName("WF_ITEM_ENTER_PARAMS");
            this.Property(t => t.WfItemPrevSibling).HasColumnName("WF_ITEM_PREV_SIBLING");
            this.Property(t => t.WfItemExitValues).HasColumnName("WF_ITEM_EXIT_VALUES");

            this.Property(t => t.WfItemDestType).HasColumnName("WF_ITEM_DEST_TYPE");
            this.Property(t => t.WfItemDestMail).HasColumnName("WF_ITEM_DEST_MAIL");
            this.Property(t => t.WfItemCcType).HasColumnName("WF_ITEM_CC_TYPE");
            this.Property(t => t.WfItemCcMail).HasColumnName("WF_ITEM_CC_MAIL");
            this.Property(t => t.WfItemTimeoutDueTime).HasColumnName("WF_ITEM_TIMEOUT_DUE_TIME");
            this.Property(t => t.WfItemTimeoutDueUnits).HasColumnName("WF_ITEM_TIMEOUT_DUE_UNITS");
            this.Property(t => t.WfItemActionProperty).HasColumnName("WF_ITEM_ACTION_PROPERTY");
            this.Property(t => t.WfItemActionValue).HasColumnName("WF_ITEM_ACTION_VALUE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.Workflow)
                .WithMany(t => t.WorkflowItems)
                .HasForeignKey(d => d.WfItemWfID);

            this.HasRequired(t => t.CommonValues)
                .WithMany(t => t.WorkflowItems)
                .HasForeignKey(d => d.WfItemType);
        }
    }
}
