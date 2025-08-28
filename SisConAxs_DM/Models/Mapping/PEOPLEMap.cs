using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class PEOPLEMap : EntityTypeConfiguration<People>
    {
        public PEOPLEMap()
        {
            // Primary Key
            this.HasKey(t => t.PeopleID);

            // Properties
            this.Property(t => t.PeopleInternalID)
                .HasMaxLength(40);

            this.Property(t => t.PeopleLastName)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.PeopleFirstName)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.PeopleLastName2)
                .HasMaxLength(255);

            this.Property(t => t.PeopleFirstName2)
                .HasMaxLength(255);

            this.Property(t => t.PeopleDocNum)
                .HasMaxLength(50);

            this.Property(t => t.PeopleAddress1)
                .HasMaxLength(200);

            this.Property(t => t.PeopleAddress2)
                .HasMaxLength(200);

            this.Property(t => t.PeoplePhone1)
                .HasMaxLength(20);

            this.Property(t => t.PeoplePhone2)
                .HasMaxLength(20);

            this.Property(t => t.PeopleEmail)
                .HasMaxLength(100);

            this.Property(t => t.PeopleBirthday);

            this.Property(t => t.PeopleGender);

            this.Property(t => t.PeopleAttribute1)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute2)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute3)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute4)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute5)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute6)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute7)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute8)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute9)
                .HasMaxLength(2000);

            this.Property(t => t.PeopleAttribute10)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);
            this.Property(t => t.CreateDate);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PEOPLE", "HHRR");
            this.Property(t => t.PeopleID).HasColumnName("PEOPLE_ID");
            this.Property(t => t.PeopleOrgID).HasColumnName("PEOPLE_ORG_ID");
            this.Property(t => t.PeopleInternalID).HasColumnName("PEOPLE_INTERNAL_ID");
            this.Property(t => t.PeopleLastName).HasColumnName("PEOPLE_LAST_NAME");
            this.Property(t => t.PeopleFirstName).HasColumnName("PEOPLE_FIRST_NAME");
            this.Property(t => t.PeopleLastName2).HasColumnName("PEOPLE_LAST_NAME2");
            this.Property(t => t.PeopleFirstName2).HasColumnName("PEOPLE_FIRST_NAME2");
            this.Property(t => t.PeopleDocType).HasColumnName("PEOPLE_DOC_TYPE");
            this.Property(t => t.PeopleDocNum).HasColumnName("PEOPLE_DOC_NUM");
            this.Property(t => t.PeopleAddress1).HasColumnName("PEOPLE_ADDRESS1");
            this.Property(t => t.PeopleAddress2).HasColumnName("PEOPLE_ADDRESS2");
            this.Property(t => t.PeoplePhone1).HasColumnName("PEOPLE_PHONE1");
            this.Property(t => t.PeoplePhone2).HasColumnName("PEOPLE_PHONE2");
            this.Property(t => t.PeopleEmail).HasColumnName("PEOPLE_EMAIL");
            this.Property(t => t.PeopleBirthday).HasColumnName("PEOPLE_BIRTHDAY");
            this.Property(t => t.PeopleGender).HasColumnName("PEOPLE_GENDER");

            this.Property(t => t.PeopleTypeClasificacion).HasColumnName("PEOPLE_TYPE_CLASIFICACION");
            this.Property(t => t.PeopleEmployeeType).HasColumnName("PEOPLE_EMPLOYEE_TYPE");

            this.Property(t => t.PeopleAddressGeolocation).HasColumnName("PEOPLE_ADDRESS_GEOLOCATION");
            this.Property(t => t.PeopleImage).HasColumnName("PEOPLE_IMAGE");
            this.Property(t => t.PeopleDepartment).HasColumnName("PEOPLE_DEPARTMENT");
            this.Property(t => t.PeoplePosition).HasColumnName("PEOPLE_POSITION");
            this.Property(t => t.PeopleAttribute1).HasColumnName("PEOPLE_ATTRIBUTE1");
            this.Property(t => t.PeopleAttribute2).HasColumnName("PEOPLE_ATTRIBUTE2");
            this.Property(t => t.PeopleProject).HasColumnName("PEOPLE_PROJECT");
            this.Property(t => t.PeopleAttribute3).HasColumnName("PEOPLE_ATTRIBUTE3");
            this.Property(t => t.PeopleAttribute4).HasColumnName("PEOPLE_ATTRIBUTE4");
            this.Property(t => t.PeopleAttribute5).HasColumnName("PEOPLE_ATTRIBUTE5");
            this.Property(t => t.PeopleAttribute6).HasColumnName("PEOPLE_ATTRIBUTE6");
            this.Property(t => t.PeopleAttribute7).HasColumnName("PEOPLE_ATTRIBUTE7");
            this.Property(t => t.PeopleAttribute8).HasColumnName("PEOPLE_ATTRIBUTE8");
            this.Property(t => t.PeopleAttribute9).HasColumnName("PEOPLE_ATTRIBUTE9");
            this.Property(t => t.PeopleAttribute10).HasColumnName("PEOPLE_ATTRIBUTE10");
            this.Property(t => t.PeopleIsSourceSAP).HasColumnName("PEOPLE_IS_SOURCE_SAP");
            this.Property(t => t.PeopleStartDate).HasColumnName("PEOPLE_START_DATE");
            this.Property(t => t.PeopleStatus).HasColumnName("PEOPLE_STATUS");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.UserID).HasColumnName("USER_ID");

            this.Property(t => t.PeopleCompany).HasColumnName("PEOPLE_COMPANY");

            this.HasRequired(t => t.Company)
                .WithMany(t => t.People)
                .HasForeignKey(d => d.PeopleCompany);

            this.HasOptional(t => t.Department)
                .WithMany(t => t.People)
                .HasForeignKey(d => d.PeopleDepartment);

            this.HasOptional(t => t.DocType)
                .WithMany(t => t.People1)
                .HasForeignKey(d => d.PeopleDocType);

            this.HasOptional(t => t.TypeClasificacion)
                .WithMany(t => t.People3)
                .HasForeignKey(d => d.PeopleTypeClasificacion);

            this.HasOptional(t => t.Position)
                .WithMany(t => t.People2)
                .HasForeignKey(d => d.PeoplePosition);

            this.HasOptional(t => t.EmployeeType)
                .WithMany(t => t.PeopleEmployeeType)
                .HasForeignKey(d => d.PeopleEmployeeType);
        }
    }
}
