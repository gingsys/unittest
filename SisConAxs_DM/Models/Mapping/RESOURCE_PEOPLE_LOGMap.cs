using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class RESOURCE_PEOPLE_LOGMap : EntityTypeConfiguration<ResourcePeopleLog>
    {
        public RESOURCE_PEOPLE_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.ResourcePeopleLogID);

            // Properties
            this.Property(t => t.ResourcePeopleID);
            this.Property(t => t.Action);
            this.Property(t => t.Source)
                .HasMaxLength(64);
            this.Property(t => t.Description)
                .HasMaxLength(2048);
            //this.Property(t => t.CreateUser);
            //this.Property(t => t.CreateDate);
            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50); ;
            //this.Property(t => t.EditDate);

            // Table & Column Mappings
            this.ToTable("RESOURCE_PEOPLE_LOG", "AXSCONTROL");
            this.Property(t => t.ResourcePeopleLogID).HasColumnName("RPL_ID");
            this.Property(t => t.ResourcePeopleID).HasColumnName("PRES_ID");
            this.Property(t => t.Action).HasColumnName("RPL_ACTION");
            this.Property(t => t.Source).HasColumnName("RPL_SOURCE");
            this.Property(t => t.Description).HasColumnName("RPL_DESCRIPTION");
            this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.ResourcePeople)
                .WithMany(t => t.ResourcePeopleLog)
                .HasForeignKey(d => d.ResourcePeopleID);
        }
    }
}
