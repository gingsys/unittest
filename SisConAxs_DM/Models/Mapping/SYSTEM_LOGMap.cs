using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using SisConAxs_DM.Models;

namespace SisConAxs_DM.Models.Mapping
{
    public class SYSTEM_LOGMap : EntityTypeConfiguration<SystemLog>
    {
        public SYSTEM_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.LogID);

            // Properties
            this.Property(t => t.LogType)
                .HasMaxLength(5)
                .IsRequired();

            // Properties
            this.Property(t => t.LogMessage)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SYSTEM_LOG", "SITCORE");
            this.Property(t => t.LogID).HasColumnName("LOG_ID");
            this.Property(t => t.LogDate).HasColumnName("LOG_DATE");
            this.Property(t => t.LogType).HasColumnName("LOG_TYPE");
            this.Property(t => t.LogMessage).HasColumnName("LOG_MESSAGE");
        }
    }
}
