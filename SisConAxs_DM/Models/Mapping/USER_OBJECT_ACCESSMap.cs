using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class USER_OBJECT_ACCESSMap : EntityTypeConfiguration<USER_OBJECT_ACCESS>
    {
        public USER_OBJECT_ACCESSMap()
        {
            // Primary Key
            this.HasKey(t => t.UO_ID);

            // Properties
            this.Property(t => t.UO_ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CREATE_USER)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EDIT_USER)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("USER_OBJECT_ACCESS", "AXSCONTROL");
            this.Property(t => t.UO_ID).HasColumnName("UO_ID");
            this.Property(t => t.UO_USER_ID).HasColumnName("UO_USER_ID");
            this.Property(t => t.UO_OBJECT_ID).HasColumnName("UO_OBJECT_ID");
            this.Property(t => t.UO_ACCESS_VALUE).HasColumnName("UO_ACCESS_VALUE");
            this.Property(t => t.UO_UP_DATE).HasColumnName("UO_UP_DATE");
            this.Property(t => t.UO_UP_WORKFLOW).HasColumnName("UO_UP_WORKFLOW");
            this.Property(t => t.UO_DOWN_DATE).HasColumnName("UO_DOWN_DATE");
            this.Property(t => t.UO_DOWN_WORKFLOW).HasColumnName("UO_DOWN_WORKFLOW");
            this.Property(t => t.UO_ACTIVE).HasColumnName("UO_ACTIVE");
            this.Property(t => t.CREATE_USER).HasColumnName("CREATE_USER");
            this.Property(t => t.CREATE_DATE).HasColumnName("CREATE_DATE");
            this.Property(t => t.EDIT_USER).HasColumnName("EDIT_USER");
            this.Property(t => t.EDIT_DATE).HasColumnName("EDIT_DATE");

            // Relationships
            //this.HasRequired(t => t.AccessResources)
            //    .WithMany(t => t.USER_OBJECT_ACCESS)
            //    .HasForeignKey(d => d.UO_OBJECT_ID);

            //this.HasRequired(t => t.AccessTypeValues)
            //    .WithOptional(t => t.USER_OBJECT_ACCESS);
            //this.HasRequired(t => t.AccessUsers)
            //    .WithMany(t => t.USER_OBJECT_ACCESS)
            //    .HasForeignKey(d => d.UO_USER_ID);
            
            //this.HasOptional(t => t.WorkflowExecution)
            //    .WithMany(t => t.USER_OBJECT_ACCESS)
            //    .HasForeignKey(d => d.UO_DOWN_WORKFLOW);
            //this.HasRequired(t => t.WorkflowExecution1)
            //    .WithMany(t => t.USER_OBJECT_ACCESS1)
            //    .HasForeignKey(d => d.UO_UP_WORKFLOW);

        }
    }
}
