using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class CALENDARMap : EntityTypeConfiguration<Calendar>
    {
        public CALENDARMap()
        {
            // Primary Key
            this.HasKey(t => t.CalID);

            // Properties
            this.Property(t => t.CalDescription)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CALENDAR", "SITCORE");
            this.Property(t => t.CalID).HasColumnName("CAL_ID");
            this.Property(t => t.CalDate).HasColumnName("CAL_DATE");
            this.Property(t => t.CalDescription).HasColumnName("CAL_DESCRIPTION");
            this.Property(t => t.CalIdCountry).HasColumnName("CAL_ID_COUNTRY");
            this.Property(t => t.CalAnual).HasColumnName("CAL_ANUAL");
            this.Property(t => t.CalActive).HasColumnName("CAL_ACTIVE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
        }
    }
}
