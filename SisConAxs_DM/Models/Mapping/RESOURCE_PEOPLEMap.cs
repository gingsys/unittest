using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class RESOURCE_PEOPLEMap : EntityTypeConfiguration<ResourcePeople>
    {
        public RESOURCE_PEOPLEMap()
        {
            // Primary Key
            this.HasKey(t => t.PresID);

            // Properties
            this.Property(t => t.PeopleID);

            this.Property(t => t.ResourceID);

            this.Property(t => t.PresStrValue)
                .HasMaxLength(2000);

            this.Property(t => t.PresActive);

            this.Property(t => t.PresTemporal);

            this.Property(t => t.PeopleDepartment);

            this.Property(t => t.PresAttribute1)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute2)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute3)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute4)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute5)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute6)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute7)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute8)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute9)
                .HasMaxLength(2000);

            this.Property(t => t.PresAttribute10)
                .HasMaxLength(2000);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RESOURCE_PEOPLE", "AXSCONTROL");
            this.Property(t => t.PresID).HasColumnName("PRES_ID");
            this.Property(t => t.PeopleID).HasColumnName("PEOPLE_ID");
            this.Property(t => t.ResourceID).HasColumnName("RESOURCE_ID");
            this.Property(t => t.ResourceFullName).HasColumnName("PRES_RESOURCE_FULLNAME");
            this.Property(t => t.AddedRequestID).HasColumnName("ADDED_REQUEST_ID");
            this.Property(t => t.AddedRequestDetID).HasColumnName("ADDED_REQUEST_DET_ID");
            this.Property(t => t.RemovedRequestID).HasColumnName("REMOVED_REQUEST_ID");
            this.Property(t => t.RemovedRequestDetID).HasColumnName("REMOVED_REQUEST_DET_ID");
            this.Property(t => t.PresIntValue).HasColumnName("PRES_INT_VALUE");
            this.Property(t => t.PresStrValue).HasColumnName("PRES_STR_VALUE");
            this.Property(t => t.PresDateValue).HasColumnName("PRES_DATE_VALUE");
            this.Property(t => t.PresDateStart).HasColumnName("PRES_DATE_START");
            this.Property(t => t.PresDateEnd).HasColumnName("PRES_DATE_END");
            this.Property(t => t.PresActive).HasColumnName("PRES_ACTIVE");
            this.Property(t => t.PresTemporal).HasColumnName("PRES_TEMPORAL");
            this.Property(t => t.PresValidityFrom).HasColumnName("PRES_VALIDITY_FROM");
            this.Property(t => t.PresValidityUntil).HasColumnName("PRES_VALIDITY_UNTIL");
            this.Property(t => t.PeopleDepartment).HasColumnName("PEOPLE_DEPARTMENT");
            this.Property(t => t.PresAdditional).HasColumnName("PRES_ADDITIONAL");
            this.Property(t => t.PresAdditionalStrValue).HasColumnName("PRES_ADDITIONAL_STR_VALUE");
            this.Property(t => t.PresAdditionalIntValue).HasColumnName("PRES_ADDITIONAL_INT_VALUE");
            this.Property(t => t.PresDisplayValue).HasColumnName("PRES_DISPLAY_VALUE");

            this.Property(t => t.PresAttribute1).HasColumnName("PRES_ATTRIBUTE1");
            this.Property(t => t.PresAttribute2).HasColumnName("PRES_ATTRIBUTE2");
            this.Property(t => t.PresAttribute3).HasColumnName("PRES_ATTRIBUTE3");
            this.Property(t => t.PresAttribute4).HasColumnName("PRES_ATTRIBUTE4");
            this.Property(t => t.PresAttribute5).HasColumnName("PRES_ATTRIBUTE5");
            this.Property(t => t.PresAttribute6).HasColumnName("PRES_ATTRIBUTE6");
            this.Property(t => t.PresAttribute7).HasColumnName("PRES_ATTRIBUTE7");
            this.Property(t => t.PresAttribute8).HasColumnName("PRES_ATTRIBUTE8");
            this.Property(t => t.PresAttribute9).HasColumnName("PRES_ATTRIBUTE9");
            this.Property(t => t.PresAttribute10).HasColumnName("PRES_ATTRIBUTE10");

            this.Property(t => t.PresCompany).HasColumnName("PRES_COMPANY");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");

            // Relationships
            this.HasRequired(t => t.AccessResources)
                .WithMany(t => t.ResourcePeople)
                .HasForeignKey(d => d.ResourceID);
            this.HasRequired(t => t.People)
                .WithMany(t => t.ResourcePeople)
                .HasForeignKey(d => d.PeopleID);

            this.HasRequired(t => t.AddedAccessRequestDetails)
                .WithMany(t => t.AddedResourcePeople)
                .HasForeignKey(d => d.AddedRequestDetID);
            this.HasRequired(t => t.RemovedAccessRequestDetails)
                .WithMany(t => t.RemovedResourcePeople)
                .HasForeignKey(d => d.RemovedRequestDetID);
        }
    }
}
