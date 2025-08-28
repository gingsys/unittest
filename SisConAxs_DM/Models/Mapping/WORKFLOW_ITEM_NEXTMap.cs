using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class WORKFLOW_ITEM_NEXTMap : EntityTypeConfiguration<WorkflowItemNext>
    {
        public WORKFLOW_ITEM_NEXTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.WfParentItemID, t.WfItemNextItemID });

            // Properties
            this.Property(t => t.WfParentItemID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.WfItemNextItemID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("WORKFLOW_ITEM_NEXT", "SITWF");
            this.Property(t => t.WfParentItemID).HasColumnName("WF_PARENT_ITEM_ID");
            this.Property(t => t.WfItemNextItemID).HasColumnName("WF_ITEM_NEXT_ITEM_ID");
            this.Property(t => t.WfItemNextType).HasColumnName("WF_ITEM_NEXT_TYPE");
            this.Property(t => t.WfItemNextStep).HasColumnName("WF_ITEM_NEXT_STEP");
            this.Property(t => t.WfItemNextWfID).HasColumnName("WF_ITEM_NEXT_WF_ID");

            // Relationships
            this.HasRequired(t => t.WorkflowItem)
                .WithMany(t => t.WorkflowItemNext)
                .HasForeignKey(d => d.WfItemNextItemID);

            this.HasRequired(t => t.WorkflowParentItem)
                .WithMany(t => t.WorkflowItemNextParents)
                .HasForeignKey(d => d.WfParentItemID);
        }
    }
}
