using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class INTEGRATION_AAD_LOGMap : EntityTypeConfiguration<IntegrationAADLog>
    {
        public INTEGRATION_AAD_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.LogID);

            // Properties
            this.Property(t => t.LogResult)
                .IsRequired();

            this.Property(t => t.LogDocNumber)
                .IsRequired();
            this.Property(t => t.LogUserName)
                .IsRequired();
            this.Property(t => t.LogNames)
                .IsRequired();
            this.Property(t => t.LogLastnames)
                .IsRequired();
            this.Property(t => t.LogCompanyName)
                .IsRequired();
            this.Property(t => t.LogEmail)
                .IsRequired();
            this.Property(t => t.LogMessage);
            //this.Property(t => t.LogLastChange);

            this.Property(t => t.CreateUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AAD_LOG", "INTEGRATION");
            this.Property(t => t.LogID).HasColumnName("ALOG_ID");
            this.Property(t => t.LogResult).HasColumnName("ALOG_RESULT");

            this.Property(t => t.LogDocNumber).HasColumnName("ALOG_DOC_NUMBER");
            this.Property(t => t.LogUserName).HasColumnName("ALOG_USERNAME");
            this.Property(t => t.LogNames).HasColumnName("ALOG_NAMES");
            this.Property(t => t.LogLastnames).HasColumnName("ALOG_LASTNAMES");
            this.Property(t => t.LogCompanyName).HasColumnName("ALOG_COMPANY_NAME");
            this.Property(t => t.LogEmail).HasColumnName("ALOG_EMAIL");
            this.Property(t => t.LogMessage).HasColumnName("ALOG_MESSAGE");
            //this.Property(t => t.LogLastChange).HasColumnName("SLOG_LAST_CHANGE");

            this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
        }
    }
}
