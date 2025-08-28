using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    class ACCESS_USER_COMPANIESMap : EntityTypeConfiguration<AccessUserCompanies>
    {
        public ACCESS_USER_COMPANIESMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CompanyID, t.UserID });

            this.Property(t => t.UserID)
                .IsRequired();
            this.Property(t => t.CompanyID)
                .IsRequired();

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("USER_COMPANY", "AXSCONTROL");
            this.Property(t => t.UserID).HasColumnName("USER_ID");
            this.Property(t => t.CompanyID).HasColumnName("COMPANY_ID");
            this.Property(t => t.UserRole1).HasColumnName("USER_ROLE1");
            this.Property(t => t.UserRole2).HasColumnName("USER_ROLE2");
            this.Property(t => t.UserRole3).HasColumnName("USER_ROLE3");
            this.Property(t => t.UserRole4).HasColumnName("USER_ROLE4");
            this.Property(t => t.UserRole5).HasColumnName("USER_ROLE5");
            this.Property(t => t.UserRole6).HasColumnName("USER_ROLE6");
            this.Property(t => t.UserRole7).HasColumnName("USER_ROLE7");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserCompanies)
                .HasForeignKey(d => d.UserID);
        }
    }
}
