using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Models.Mapping
{
    public class REQUEST_TEMPLATE_DETAILSMap : EntityTypeConfiguration<RequestTemplateDetail>
    {
        /*
         [REQ_TEMPLATE_ID]                       INT            NOT NULL,
        [REQ_TEMPLATE_DET_ID]                   INT            IDENTITY (1, 1) NOT NULL,
        [REQ_TEMPLATE_DET_RESOURCE_ID]          INT            NOT NULL,
        [REQ_TEMPLATE_DET_STR_VALUE]            VARCHAR (200)  NULL,
        [REQ_TEMPLATE_DET_INT_VALUE]            INT            NULL,
        [REQ_TEMPLATE_DET_TEMPORAL]             INT            DEFAULT ((0)) NOT NULL,
        [REQ_TEMPLATE_DET_VALIDITY_FROM]        DATETIME       NULL,
        [REQ_TEMPLATE_DET_VALIDITY_UNTIL]       DATETIME       NULL,
	    [REQ_TEMPLATE_DET_ADDITIONAL]           INT            DEFAULT ((0)) NOT NULL,
        [REQ_TEMPLATE_DET_ADDITIONAL_STR_VALUE] VARCHAR (200)  NULL,
        [REQ_TEMPLATE_DET_ADDITIONAL_INT_VALUE] INT            DEFAULT ((0)) NOT NULL,
         */

        public REQUEST_TEMPLATE_DETAILSMap()
        {
            // Primary Key
            this.HasKey(t => t.ReqTemplateDetID);

            // Properties
            this.Property(t => t.ReqTemplateDetStrValue)
                .HasMaxLength(200);
            this.Property(t => t.ReqTemplateDetAdditionalStrValue)
                .HasMaxLength(200);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("REQUEST_TEMPLATE_DETAILS", "AXSCONTROL");
            this.Property(t => t.ReqTemplateID).HasColumnName("REQ_TEMPLATE_ID");
            this.Property(t => t.ReqTemplateDetID).HasColumnName("REQ_TEMPLATE_DET_ID");
            this.Property(t => t.ReqTemplateDetResourceID).HasColumnName("REQ_TEMPLATE_DET_RESOURCE_ID");
            this.Property(t => t.ReqTemplateDetStrValue).HasColumnName("REQ_TEMPLATE_DET_STR_VALUE");
            this.Property(t => t.ReqTemplateDetIntValue).HasColumnName("REQ_TEMPLATE_DET_INT_VALUE");
            this.Property(t => t.ReqTemplateDetTemporal).HasColumnName("REQ_TEMPLATE_DET_TEMPORAL");
            this.Property(t => t.ReqTemplateDetValidityFrom).HasColumnName("REQ_TEMPLATE_DET_VALIDITY_FROM");
            this.Property(t => t.ReqTemplateDetValidityUntil).HasColumnName("REQ_TEMPLATE_DET_VALIDITY_UNTIL");
            //this.Property(t => t.ReqDetSendAtEnd).HasColumnName("REQ_TEMPLATE_DET_SEND_AT_END");
            this.Property(t => t.ReqTemplateDetAdditional).HasColumnName("REQ_TEMPLATE_DET_ADDITIONAL");
            this.Property(t => t.ReqTemplateDetAdditionalStrValue).HasColumnName("REQ_TEMPLATE_DET_ADDITIONAL_STR_VALUE");
            this.Property(t => t.ReqTemplateDetAdditionalIntValue).HasColumnName("REQ_TEMPLATE_DET_ADDITIONAL_INT_VALUE");

            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");


            // Relationships
            this.HasRequired(t => t.RelRequestTemplate)
                .WithMany(t => t.ReqTemplateDetails)
                .HasForeignKey(d => d.ReqTemplateID);

            this.HasRequired(t => t.RelResource)
                .WithMany(t => t.RequestTemplateDetail)
                .HasForeignKey(d => d.ReqTemplateDetResourceID);
        }
    }
}
