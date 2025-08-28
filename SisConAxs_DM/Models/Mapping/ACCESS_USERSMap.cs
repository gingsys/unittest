using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_USERSMap : EntityTypeConfiguration<AccessUsers>
    {
        public ACCESS_USERSMap()
        {
            // Primary Key
            this.HasKey(t => t.UserID);

            // Properties
            this.Property(t => t.UserInternalID)
                .HasMaxLength(40);

            this.Property(t => t.UserLastName)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.UserFirstName)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.UserName3)
                .HasMaxLength(40);

            this.Property(t => t.UserName4)
                .HasMaxLength(40);

            this.Property(t => t.UserDocNum)
                .HasMaxLength(50);

            this.Property(t => t.UserAddress1)
                .HasMaxLength(200);

            this.Property(t => t.UserAddress2)
                .HasMaxLength(200);

            this.Property(t => t.UserPhone1)
                .HasMaxLength(20);

            this.Property(t => t.UserPhone2)
                .HasMaxLength(20);

            this.Property(t => t.UserEMail)
                .HasMaxLength(100);

            //this.Property(t => t.CreateUser)
            //    .IsRequired()
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UserPassword)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ACCESS_USERS", "AXSCONTROL");
            this.Property(t => t.UserID).HasColumnName("USER_ID");
            this.Property(t => t.UserInternalID).HasColumnName("USER_INTERNAL_ID");
            this.Property(t => t.UserLastName).HasColumnName("USER_LAST_NAME");
            this.Property(t => t.UserFirstName).HasColumnName("USER_FIRST_NAME");
            this.Property(t => t.UserName3).HasColumnName("USER_NAME_3");
            this.Property(t => t.UserName4).HasColumnName("USER_NAME_4");
            this.Property(t => t.UserDocNum).HasColumnName("USER_DOC_NUM");
            this.Property(t => t.UserAddress1).HasColumnName("USER_ADDRESS1");
            this.Property(t => t.UserAddress2).HasColumnName("USER_ADDRESS2");
            this.Property(t => t.UserPhone1).HasColumnName("USER_PHONE1");
            this.Property(t => t.UserPhone2).HasColumnName("USER_PHONE2");
            this.Property(t => t.UserEMail).HasColumnName("USER_EMAIL");
            this.Property(t => t.UserAddressGeoLocation).HasColumnName("USER_ADDRESS_GEOLOCATION");
            this.Property(t => t.UserPassword).HasColumnName("USER_PASSWORD");
            this.Property(t => t.UserRole1).HasColumnName("USER_ROLE1");
            this.Property(t => t.UserRole2).HasColumnName("USER_ROLE2");
            this.Property(t => t.UserRole3).HasColumnName("USER_ROLE3");
            this.Property(t => t.UserRole4).HasColumnName("USER_ROLE4");
            this.Property(t => t.UserRole5).HasColumnName("USER_ROLE5");
            this.Property(t => t.UserRole6).HasColumnName("USER_ROLE6");
            this.Property(t => t.UserRoleSysAdmin).HasColumnName("USER_ROLE_SYSADMIN");
            this.Property(t => t.UserStatus).HasColumnName("USER_STATUS");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.LastAccessUser).HasColumnName("LAST_ACCESS_USER");
        }
    }
}
