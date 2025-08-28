using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class COMMON_VALUESMap : EntityTypeConfiguration<CommonValues>
    {
        public COMMON_VALUESMap()
        {
            // Primary Key
            this.HasKey(t => t.CommonValueID);

            // Properties
            this.Property(t => t.CommonValueID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CommonValueName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.CommonValueDisplay)
                .HasMaxLength(150);

            this.Property(t => t.CommonValueDesc)
                .HasMaxLength(500);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("COMMON_VALUES", "SITCORE");
            this.Property(t => t.CommonValueID).HasColumnName("COMMON_VALUE_ID");
            this.Property(t => t.CommonValueSetID).HasColumnName("COMMON_VALUE_SET_ID");
            this.Property(t => t.CommonValueName).HasColumnName("COMMON_VALUE_NAME");
            this.Property(t => t.CommonValueDisplay).HasColumnName("COMMON_VALUE_DISPLAY");
            this.Property(t => t.CommonValueDesc).HasColumnName("COMMON_VALUE_DESC");
            this.Property(t => t.CommonValueDefault).HasColumnName("COMMON_VALUE_DEFAULT");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            this.Property(t => t.CommonValueCompany).HasColumnName("COMMON_VALUE_COMPANY");

            // Relationships
            this.HasRequired(t => t.CommonValueSets)
                .WithMany(t => t.CommonValues)
                .HasForeignKey(d => d.CommonValueSetID);

        }
    }
}
