using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class METADATAMap : EntityTypeConfiguration<Metadata>
    {
        public METADATAMap()
        {
            // Primary Key
            this.HasKey(t => t.MetadataID);

            // Properties
            this.Property(t => t.MetadataDisplay)
                .HasMaxLength(512);
            this.Property(t => t.MetadataDescription)
                .HasMaxLength(2048);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("METADATA", "SITCORE");
            this.Property(t => t.MetadataID).HasColumnName("MTD_ID");
            this.Property(t => t.MetadataParentID).HasColumnName("MTD_PARENT_ID");
            this.Property(t => t.MetadataDisplay).HasColumnName("MTD_DISPLAY");
            this.Property(t => t.MetadataDescription).HasColumnName("MTD_DESCRIPTION");
            this.Property(t => t.MetadataInt1).HasColumnName("MTD_INT1");
            this.Property(t => t.MetadataInt2).HasColumnName("MTD_INT2");
            this.Property(t => t.MetadataInt3).HasColumnName("MTD_INT3");
            this.Property(t => t.MetadataInt4).HasColumnName("MTD_INT4");
            this.Property(t => t.MetadataInt5).HasColumnName("MTD_INT5");
            this.Property(t => t.MetadataStr1).HasColumnName("MTD_STR1");
            this.Property(t => t.MetadataStr2).HasColumnName("MTD_STR2");
            this.Property(t => t.MetadataStr3).HasColumnName("MTD_STR3");
            this.Property(t => t.MetadataStr4).HasColumnName("MTD_STR4");
            this.Property(t => t.MetadataStr5).HasColumnName("MTD_STR5");
            this.Property(t => t.MetadataDatetime1).HasColumnName("MTD_DATETIME1");
            this.Property(t => t.MetadataDatetime2).HasColumnName("MTD_DATETIME2");
            this.Property(t => t.MetadataDatetime3).HasColumnName("MTD_DATETIME3");
            this.Property(t => t.MetadataActive).HasColumnName("MTD_ACTIVE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            //this.HasRequired(t => t.MetadataParent)
            //    .WithMany(t => t.MetadataChilds)
            //    .HasForeignKey(d => d.MetadataID);
        }
    }
}
