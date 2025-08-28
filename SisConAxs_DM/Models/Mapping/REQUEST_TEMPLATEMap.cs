using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class REQUEST_TEMPLATEMap : EntityTypeConfiguration<RequestTemplate>
    {
        /*
        [REQ_TEMPLATE_ID]             INT           IDENTITY (1, 1) NOT NULL,
	    [REQ_TEMPLATE_COMPANY]        INT           NOT NULL,
	    [REQ_TEMPLATE_TYPE]           INT           NOT NULL,
        [REQ_TEMPLATE_EMPLOYEE_TYPE]  INT			NOT NULL,
        [REQ_TEMPLATE_ACTIVE]
         */
        public REQUEST_TEMPLATEMap()
        {
            // Primary Key
            this.HasKey(t => t.ReqTemplateID);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("REQUEST_TEMPLATE", "AXSCONTROL");
            this.Property(t => t.ReqTemplateID).HasColumnName("REQ_TEMPLATE_ID");
            this.Property(t => t.ReqTemplateCompany).HasColumnName("REQ_TEMPLATE_COMPANY");
            this.Property(t => t.ReqTemplateType).HasColumnName("REQ_TEMPLATE_TYPE");
            this.Property(t => t.ReqTemplateEmployeeType).HasColumnName("REQ_TEMPLATE_EMPLOYEE_TYPE");
            this.Property(t => t.ReqTemplateActive).HasColumnName("REQ_TEMPLATE_ACTIVE");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");


            // Relationships
            this.HasRequired(t => t.RelReqTemplateType)
                .WithMany(t => t.ReqTemplateType)
                .HasForeignKey(d => d.ReqTemplateType);

            this.HasRequired(t => t.RelReqTemplateEmployeeType)
                .WithMany(t => t.ReqTemplateEmployeeType)
                .HasForeignKey(d => d.ReqTemplateEmployeeType);
        }
    }
}
