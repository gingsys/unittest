using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_TYPESMap : EntityTypeConfiguration<AccessTypes>
    {
        public ACCESS_TYPESMap()
        {
            // Primary Key
            this.HasKey(t => t.AccessTypeID);

            // Properties
            this.Property(t => t.AccessTypeName)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.AccessTypeType);
                //.IsRequired();

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_TYPES", "AXSCONTROL");
            this.Property(t => t.AccessTypeID).HasColumnName("ACCESS_TYPE_ID");
            this.Property(t => t.AccessTypeName).HasColumnName("ACCESS_TYPE_NAME");
            this.Property(t => t.AccessTypeType).HasColumnName("ACCESS_TYPE_TYPE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.AccessTypeCompany).HasColumnName("ACCESS_TYPE_COMPANY");
        }
    }
}
