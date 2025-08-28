using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class COMPANYMap : EntityTypeConfiguration<Company>
    {
        public COMPANYMap()
        {
            // Primary Key
            this.HasKey(t => t.CompanyID);

            // Properties
            this.Property(t => t.CompanyTaxpayerID)
                .IsRequired()
                .HasMaxLength(64);
            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(255);
            this.Property(t => t.CompanyDisplay)
                .IsRequired()
                .HasMaxLength(255);
            this.Property(t => t.CompanyCountry)
                .IsRequired();
            //this.Property(t => t.CompanyAD)
            //    .IsRequired()
            //    .HasMaxLength(255);
            this.Property(t => t.CompanyAddress)
                .HasMaxLength(1024);
            //this.Property(t => t.CompanyActive)
            //    .IsRequired();

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("COMPANY", "AXSCONTROL");
            this.Property(t => t.CompanyID).HasColumnName("COMPANY_ID");
            this.Property(t => t.CompanyTaxpayerID).HasColumnName("COMPANY_TAXPAYER_ID");
            this.Property(t => t.CompanyName).HasColumnName("COMPANY_NAME");
            this.Property(t => t.CompanyDisplay).HasColumnName("COMPANY_DISPLAY");
            this.Property(t => t.CompanyCountry).HasColumnName("COMPANY_COUNTRY");
            //this.Property(t => t.CompanyAD).HasColumnName("COMPANY_AD");
            this.Property(t => t.CompanyAddress).HasColumnName("COMPANY_ADDRESS");
            this.Property(t => t.CompanyActive).HasColumnName("COMPANY_ACTIVE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
        }
    }
}
