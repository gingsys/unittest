using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class INTEGRATION_SAP_LOGMap : EntityTypeConfiguration<IntegrationSAPLog>
    {
        public INTEGRATION_SAP_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.LogID);

            // Properties
            this.Property(t => t.LogType)
                .IsRequired();
            this.Property(t => t.LogResult)
                .IsRequired();

            this.Property(t => t.LogTypeDate)
                .IsRequired();
            this.Property(t => t.LogDocNumber)
                .IsRequired();
            this.Property(t => t.LogNames)
                .IsRequired();
            this.Property(t => t.LogLastnames)
                .IsRequired();
            this.Property(t => t.LogCompanyName)
                .IsRequired();
            this.Property(t => t.LogMessage);
            //this.Property(t => t.LogLastChange);

            this.Property(t => t.CreateUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SAP_LOG", "INTEGRATION");
            this.Property(t => t.LogID).HasColumnName("SLOG_ID");
            this.Property(t => t.LogType).HasColumnName("SLOG_TYPE");
            this.Property(t => t.LogResult).HasColumnName("SLOG_RESULT");

            this.Property(t => t.LogTypeDate).HasColumnName("SLOG_TYPE_DATE");
            this.Property(t => t.LogDocNumber).HasColumnName("SLOG_DOC_NUMBER");
            this.Property(t => t.LogNames).HasColumnName("SLOG_NAMES");
            this.Property(t => t.LogLastnames).HasColumnName("SLOG_LASTNAMES");
            this.Property(t => t.LogCompanyName).HasColumnName("SLOG_COMPANY_NAME");
            this.Property(t => t.LogMessage).HasColumnName("SLOG_MESSAGE");
            //this.Property(t => t.LogLastChange).HasColumnName("SLOG_LAST_CHANGE");

            this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
        }
    }
}
