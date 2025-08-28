using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class COMPANY_PARAMETERSMap : EntityTypeConfiguration<CompanyParameter>
    {
        public COMPANY_PARAMETERSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CompanyID, t.CompanyParameterID });

            // Properties
            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("COMPANY_PARAMETERS", "AXSCONTROL");
            this.Property(t => t.CompanyID).HasColumnName("COMPANY_ID");
            this.Property(t => t.CompanyParameterID).HasColumnName("COMPANY_PARAM_ID");
            this.Property(t => t.Value).HasColumnName("COMPANY_PARAM_VALUE");
            //this.Property(t => t.ValueInt).HasColumnName("COMPANY_PARAM_VAL_INT");
            //this.Property(t => t.ValueDate).HasColumnName("COMPANY_PARAM_VAL_DATE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            //this.HasRequired(t => t.Company)
            //    .WithMany(t => t.CompanyParameters)
            //    .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.ParameterName)
                .WithMany(t => t.CompanyParameters)
                .HasForeignKey(d => d.CompanyParameterID);
        }
    }
}
