using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_REQUEST_DETAILSMap : EntityTypeConfiguration<AccessRequestDetails>
    {
        public ACCESS_REQUEST_DETAILSMap()
        {
            // Primary Key
            this.HasKey(t => t.RequestDetID);

            // Properties
            this.Property(t => t.RequestDetStrValue)
                .HasMaxLength(200);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_REQUEST_DETAILS", "AXSCONTROL");
            this.Property(t => t.RequestID).HasColumnName("REQUEST_ID");
            this.Property(t => t.RequestDetID).HasColumnName("REQUEST_DET_ID");
            this.Property(t => t.ResourceID).HasColumnName("REQUEST_DET_RESOURCE_ID");
            this.Property(t => t.RequestDetStatus).HasColumnName("REQUEST_DET_STATUS");
            this.Property(t => t.RequestDetStep).HasColumnName("REQUEST_DET_STEP");
            this.Property(t => t.RequestDetType).HasColumnName("REQUEST_DET_TYPE");
            this.Property(t => t.RequestDetStrValue).HasColumnName("REQUEST_DET_STR_VALUE");
            this.Property(t => t.RequestDetIntValue).HasColumnName("REQUEST_DET_INT_VALUE");
            this.Property(t => t.ReqDetTemporal).HasColumnName("REQUEST_DET_TEMPORAL");
            this.Property(t => t.ReqDetValidityFrom).HasColumnName("REQUEST_DET_VALIDITY_FROM");
            this.Property(t => t.ReqDetValidityUntil).HasColumnName("REQUEST_DET_VALIDITY_UNTIL");
            this.Property(t => t.ReqDetSendAtEnd).HasColumnName("REQUEST_DET_SEND_AT_END");
            this.Property(t => t.RequestDetAdditional).HasColumnName("REQUEST_DET_ADDITIONAL");
            this.Property(t => t.RequestDetAdditionalStrValue).HasColumnName("REQUEST_DET_ADDITIONAL_STR_VALUE");
            this.Property(t => t.RequestDetAdditionalIntValue).HasColumnName("REQUEST_DET_ADDITIONAL_INT_VALUE");
            this.Property(t => t.RequestDetParam01).HasColumnName("REQUEST_DET_PARAM1");

            this.Property(t => t.ResourceFullName).HasColumnName("REQUEST_DET_RESOURCE_FULLNAME");
            this.Property(t => t.RequestDetDisplayValue).HasColumnName("REQUEST_DET_DISPLAY_VALUE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.AccessRequest)
                .WithMany(t => t.AccessRequestDetails)
                .HasForeignKey(d => d.RequestID);
            this.HasRequired(t => t.AccessResources)
                .WithMany(t => t.AccessRequestDetails)
                .HasForeignKey(d => d.ResourceID);
            this.HasRequired(t => t.CommonValuesType)
                .WithMany(t => t.AccessRequestDetailsType)
                .HasForeignKey(d => d.RequestDetType);
            this.HasRequired(t => t.CommonValuesStatus)
                .WithMany(t => t.AccessRequestDetailsStatus)
                .HasForeignKey(d => d.RequestDetStatus);
        }
    }
}
