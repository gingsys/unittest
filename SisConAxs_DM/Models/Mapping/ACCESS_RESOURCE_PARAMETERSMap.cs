using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_RESOURCE_PARAMETERSMap : EntityTypeConfiguration<AccessResourceParameter>
    {
        public ACCESS_RESOURCE_PARAMETERSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ResourceID, t.ResourceParameterID });

            // Properties
            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_RESOURCE_PARAMETERS", "AXSCONTROL");
            this.Property(t => t.ResourceID).HasColumnName("RESOURCE_ID");
            this.Property(t => t.ResourceParameterID).HasColumnName("RESOURCE_PARAM_ID");
            this.Property(t => t.ResourceParameterMetadataID).HasColumnName("RESOURCE_PARAM_METADATA_ID");
            this.Property(t => t.Value).HasColumnName("RESOURCE_PARAM_VALUE");
            //this.Property(t => t.ValueInt).HasColumnName("RESOURCE_PARAM_VAL_INT");
            //this.Property(t => t.ValueDate).HasColumnName("RESOURCE_PARAM_VAL_DATE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.AccessResource)
                .WithMany(t => t.ResourceParameters)
                .HasForeignKey(d => d.ResourceID);

            this.HasRequired(t => t.ParameterName)
                .WithMany(t => t.AccessResourceParameters)
                .HasForeignKey(d => d.ResourceParameterID);
        }
    }
}
