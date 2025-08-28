using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class ACCESS_REQUESTSMap : EntityTypeConfiguration<AccessRequests>
    {
        public ACCESS_REQUESTSMap()
        {
            // Primary Key
            this.HasKey(t => t.RequestID);

            // Properties
            this.Property(t => t.RequestNumber)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            this.Property(t => t.RequestDate)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            this.Property(t => t.RequestNote)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ACCESS_REQUESTS", "AXSCONTROL");
            this.Property(t => t.RequestID).HasColumnName("REQUEST_ID");
            this.Property(t => t.RequestTo).HasColumnName("REQUEST_TO");
            this.Property(t => t.RequestToCompany).HasColumnName("REQUEST_TO_COMPANY");
            this.Property(t => t.RequestToProject).HasColumnName("REQUEST_TO_PROJECT");
            this.Property(t => t.RequestToDepartment).HasColumnName("REQUEST_TO_DEPARTMENT");
            this.Property(t => t.RequestToPosition).HasColumnName("REQUEST_TO_POSITION");
            this.Property(t => t.RequestBy).HasColumnName("REQUEST_BY");
            this.Property(t => t.RequestByProject).HasColumnName("REQUEST_BY_PROJECT");
            this.Property(t => t.RequestByDepartment).HasColumnName("REQUEST_BY_DEPARTMENT");
            this.Property(t => t.RequestByPosition).HasColumnName("REQUEST_BY_POSITION");
            this.Property(t => t.RequestNumber).HasColumnName("REQUEST_NUMBER");
            this.Property(t => t.RequestType).HasColumnName("REQUEST_TYPE");
            this.Property(t => t.RequestPriority).HasColumnName("REQUEST_PRIORITY");
            this.Property(t => t.RequestStatus).HasColumnName("REQUEST_STATUS");
            this.Property(t => t.RequestDate).HasColumnName("REQUEST_DATE");
            this.Property(t => t.RequestDepartment).HasColumnName("REQUEST_DEPARTMENT");
            this.Property(t => t.RequestCompletedDate).HasColumnName("REQUEST_COMPLETED_DATE");
            this.Property(t => t.RequestNote).HasColumnName("REQUEST_NOTE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
            this.Property(t => t.RequestCompany).HasColumnName("REQUEST_COMPANY");
            this.Property(t => t.RequestAttached).HasColumnName("REQUEST_ATTACHED");
            //Campos de auditoria
            this.Property(t => t.AttentionTicket).HasColumnName("ATTENTION_TICKET");
            this.Property(t => t.OracleUser).HasColumnName("ORACLE_USER");
            this.Property(t => t.OracleMenu).HasColumnName("ORACLE_MENU");

            // Relationships
            this.HasRequired(t => t.ReqPriority)
                .WithMany(t => t.AccessRequests)
                .HasForeignKey(d => d.RequestPriority);
            this.HasRequired(t => t.ReqStatus)
                .WithMany(t => t.AccessRequests1)
                .HasForeignKey(d => d.RequestStatus);
            this.HasRequired(t => t.ReqType)
                .WithMany(t => t.AccessRequests2)
                .HasForeignKey(d => d.RequestType);
            this.HasRequired(t => t.PeopleRequestTo)
                .WithMany(t => t.AccessRequests)
                .HasForeignKey(d => d.RequestTo);
        }
    }
}
