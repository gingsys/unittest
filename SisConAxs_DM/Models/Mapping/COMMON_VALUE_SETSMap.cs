using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class COMMON_VALUE_SETSMap : EntityTypeConfiguration<CommonValueSets>
    {
        public COMMON_VALUE_SETSMap()
        {
            // Primary Key
            this.HasKey(t => t.CommonValueSetID);

            // Properties
            this.Property(t => t.CommonValueSetID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CommonValueSetName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.CommonValueSetDesc)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            //this.Property(t => t.CommonValueSetSystemValue);
            

            // Table & Column Mappings
            this.ToTable("COMMON_VALUE_SETS", "SITCORE");
            this.Property(t => t.CommonValueSetID).HasColumnName("COMMON_VALUE_SET_ID");
            this.Property(t => t.CommonValueSetName).HasColumnName("COMMON_VALUE_SET_NAME");
            this.Property(t => t.CommonValueSetDesc).HasColumnName("COMMON_VALUE_SET_DESC");
            this.Property(t => t.CommonValueSetSeqSeed).HasColumnName("COMMON_VALUE_SET_SEQ_SEED");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.CommonValueSetSystemValue).HasColumnName("COMMON_VALUE_SET_SYSTEM_VALUE");
            this.Property(t => t.CommonValueSetRestrictedByCompany).HasColumnName("COMMON_VALUE_SET_RESTRICTED_BY_COMPANY");
        }
    }
}
