using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_RESOURCESMap : EntityTypeConfiguration<AccessResources>
    {
        public ACCESS_RESOURCESMap()
        {
            // Primary Key
            this.HasKey(t => t.ResourceID);

            // Properties
            this.Property(t => t.ResourceName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ResourceFullName)
                .HasMaxLength(2000);

            this.Property(t => t.ResourceDescription)
                .HasMaxLength(2000);

            this.Property(t => t.ResourceParam1)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam2)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam3)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam4)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam5)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam6)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam7)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam8)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam9)
                .HasMaxLength(500);

            this.Property(t => t.ResourceParam10)
                .HasMaxLength(500);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_RESOURCES", "AXSCONTROL");
            this.Property(t => t.ResourceID).HasColumnName("RESOURCE_ID");
            this.Property(t => t.ResourceCategory).HasColumnName("RESOURCE_CATEGORY");
            this.Property(t => t.ResourceName).HasColumnName("RESOURCE_NAME");
            this.Property(t => t.ResourceFullName).HasColumnName("RESOURCE_FULLNAME");
            this.Property(t => t.ResourceDescription).HasColumnName("RESOURCE_DESCRIPTION");
            this.Property(t => t.ResourceTemporal).HasColumnName("RESOURCE_TEMPORAL");
            this.Property(t => t.ResourceSendAtEnd).HasColumnName("RESOURCE_SEND_AT_END");
            this.Property(t => t.ResourceDepartment).HasColumnName("RESOURCE_DEPARTMENT");
            this.Property(t => t.ResourceParent).HasColumnName("RESOURCE_PARENT");
            this.Property(t => t.ResourceParam1).HasColumnName("RESOURCE_PARAM1");
            this.Property(t => t.ResourceParam2).HasColumnName("RESOURCE_PARAM2");
            this.Property(t => t.ResourceParam3).HasColumnName("RESOURCE_PARAM3");
            this.Property(t => t.ResourceParam4).HasColumnName("RESOURCE_PARAM4");
            this.Property(t => t.ResourceParam5).HasColumnName("RESOURCE_PARAM5");
            this.Property(t => t.ResourceParam6).HasColumnName("RESOURCE_PARAM6");
            this.Property(t => t.ResourceParam7).HasColumnName("RESOURCE_PARAM7");
            this.Property(t => t.ResourceParam8).HasColumnName("RESOURCE_PARAM8");
            this.Property(t => t.ResourceParam9).HasColumnName("RESOURCE_PARAM9");
            this.Property(t => t.ResourceParam10).HasColumnName("RESOURCE_PARAM10");
            this.Property(t => t.ResourceAccessType).HasColumnName("RESOURCE_ACCESS_TYPE");
            this.Property(t => t.ResourceRequired).HasColumnName("RESOURCE_REQUIRED");

            this.Property(t => t.ResourceFlag).HasColumnName("RESOURCE_FLAG");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.ResourceWorkflow).HasColumnName("RESOURCE_WORKFLOW");
            this.Property(t => t.ResourceActive).HasColumnName("RESOURCE_ACTIVE");
            this.Property(t => t.ResourceOnlyAssignable).HasColumnName("RESOURCE_ONLY_ASSIGNABLE");

            this.Property(t => t.ResourceCompany).HasColumnName("RESOURCE_COMPANY");

            // Relationships
            //this.HasOptional(t => t.AccessResourcesParent)
            //    .WithMany(t => t.AccessResourcesChildren)
            //    .HasForeignKey(d => d.ResourceParent);

            this.HasRequired(t => t.AccessTypes)
                .WithMany(t => t.AccessResources)
                .HasForeignKey(d => d.ResourceAccessType);

            this.HasRequired(t => t.ResourceCategories)
                .WithMany(t => t.AccessResources)
                .HasForeignKey(d => d.ResourceCategory);

            this.HasOptional(t => t.Workflows)
                .WithMany(t => t.AccessResources)
                .HasForeignKey(d => d.ResourceWorkflow);

        }
    }
}
