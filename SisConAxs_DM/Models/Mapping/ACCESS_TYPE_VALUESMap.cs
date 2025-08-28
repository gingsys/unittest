using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_TYPE_VALUESMap : EntityTypeConfiguration<AccessTypeValues>
    {
        public ACCESS_TYPE_VALUESMap()
        {
            // Primary Key
            this.HasKey(t => t.TypeValueID);

            // Properties
            this.Property(t => t.TypeValueID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TypeValueName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TypeValueDisplay)
                .HasMaxLength(100);

            this.Property(t => t.TypeValueCharVal)
                .HasMaxLength(200);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_TYPE_VALUES", "AXSCONTROL");
            this.Property(t => t.AccessTypeID).HasColumnName("ACCESS_TYPE_ID");
            this.Property(t => t.TypeValueID).HasColumnName("TYPE_VALUE_ID");
            this.Property(t => t.TypeValueName).HasColumnName("TYPE_VALUE_NAME");
            this.Property(t => t.TypeValueDisplay).HasColumnName("TYPE_VALUE_DISPLAY");
            this.Property(t => t.TypeValueIntVal).HasColumnName("TYPE_VALUE_INT_VAL");
            this.Property(t => t.TypeValueCharVal).HasColumnName("TYPE_VALUE_CHAR_VAL");
            this.Property(t => t.TypeValueDefault).HasColumnName("TYPE_VALUE_DEFAULT");
            this.Property(t => t.TypeValueAdditional).HasColumnName("TYPE_VALUE_ADDITIONAL");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            ////this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.AccessTypes)
                .WithMany(t => t.AccessTypeValues)
                .HasForeignKey(d => d.AccessTypeID);

        }
    }
}
