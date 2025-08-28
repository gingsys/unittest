using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class INTEGRATION_ORACLE_LOGMap : EntityTypeConfiguration<IntegrationOracleLog>
    {
        public INTEGRATION_ORACLE_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.LogID);

            // Properties
            this.Property(t => t.LogType)
                .IsRequired()
                .HasMaxLength(32);
            this.Property(t => t.LogData1)
                .HasMaxLength(512);
            this.Property(t => t.LogData2)
                .HasMaxLength(512);
            this.Property(t => t.LogMessage);
            this.Property(t => t.LogActive);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ORACLE_LOG", "INTEGRATION");
            this.Property(t => t.LogID).HasColumnName("OLOG_ID");
            this.Property(t => t.LogType).HasColumnName("OLOG_TYPE");
            this.Property(t => t.LogData1).HasColumnName("OLOG_DATA1");
            this.Property(t => t.LogData2).HasColumnName("OLOG_DATA2");
            this.Property(t => t.LogMessage).HasColumnName("OLOG_MESSAGE");
            this.Property(t => t.LogActive).HasColumnName("OLOG_ACTIVE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
        }
    }
}
