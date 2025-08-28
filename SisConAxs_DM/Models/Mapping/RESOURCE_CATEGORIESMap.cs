using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class RESOURCE_CATEGORIESMap : EntityTypeConfiguration<ResourceCategories>
    {
        public RESOURCE_CATEGORIESMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryID);

            // Properties
            this.Property(t => t.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CategoryDescription)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RESOURCE_CATEGORIES", "AXSCONTROL");
            this.Property(t => t.CategoryID).HasColumnName("CATEGORY_ID");
            this.Property(t => t.CategoryName).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.CategoryDescription).HasColumnName("CATEGORY_DESCRIPTION");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
        }
    }
}
