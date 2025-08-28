using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SisConAxs_DM.Models.Mapping
{
    public class SYSTEM_CONFIGMap : EntityTypeConfiguration<SystemConfig>
    {
        public SYSTEM_CONFIGMap()
        {
            // Primary Key
            this.HasKey(t => t.SysConfID);

            // Properties
            this.Property(t => t.SysConfName)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.SysConfDesc)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue1)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue2)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue3)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue4)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue5)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue6)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue7)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue8)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue9)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue10)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue11)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue12)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue13)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue14)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue15)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue16)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue17)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue18)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue19)
                .HasMaxLength(2000);

            this.Property(t => t.SysConfValue20)
                .HasMaxLength(2000);

            //this.Property(t => t.CreateUser)
            //    .HasMaxLength(50);

            this.Property(t => t.EditUser)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SYSTEM_CONFIG", "SITCORE");
            this.Property(t => t.SysConfID).HasColumnName("SYSCONF_ID");
            this.Property(t => t.SysConfName).HasColumnName("SYSCONF_NAME");
            this.Property(t => t.SysConfDesc).HasColumnName("SYSCONF_DESC");
            this.Property(t => t.SysConfValue1).HasColumnName("SYSCONF_VALUE1");
            this.Property(t => t.SysConfValue2).HasColumnName("SYSCONF_VALUE2");
            this.Property(t => t.SysConfValue3).HasColumnName("SYSCONF_VALUE3");
            this.Property(t => t.SysConfValue4).HasColumnName("SYSCONF_VALUE4");
            this.Property(t => t.SysConfValue5).HasColumnName("SYSCONF_VALUE5");
            this.Property(t => t.SysConfValue6).HasColumnName("SYSCONF_VALUE6");
            this.Property(t => t.SysConfValue7).HasColumnName("SYSCONF_VALUE7");
            this.Property(t => t.SysConfValue8).HasColumnName("SYSCONF_VALUE8");
            this.Property(t => t.SysConfValue9).HasColumnName("SYSCONF_VALUE9");
            this.Property(t => t.SysConfValue10).HasColumnName("SYSCONF_VALUE10");
            this.Property(t => t.SysConfValue11).HasColumnName("SYSCONF_VALUE11");
            this.Property(t => t.SysConfValue12).HasColumnName("SYSCONF_VALUE12");
            this.Property(t => t.SysConfValue13).HasColumnName("SYSCONF_VALUE13");
            this.Property(t => t.SysConfValue14).HasColumnName("SYSCONF_VALUE14");
            this.Property(t => t.SysConfValue15).HasColumnName("SYSCONF_VALUE15");
            this.Property(t => t.SysConfValue16).HasColumnName("SYSCONF_VALUE16");
            this.Property(t => t.SysConfValue17).HasColumnName("SYSCONF_VALUE17");
            this.Property(t => t.SysConfValue18).HasColumnName("SYSCONF_VALUE18");
            this.Property(t => t.SysConfValue19).HasColumnName("SYSCONF_VALUE19");
            this.Property(t => t.SysConfValue20).HasColumnName("SYSCONF_VALUE20");
            //this.Property(t => t.CreateUser).HasColumnName("CREATE_USER");
            //this.Property(t => t.CreateDate).HasColumnName("CREATE_DATE");
            this.Property(t => t.EditUser).HasColumnName("EDIT_USER");
            //this.Property(t => t.EditDate).HasColumnName("EDIT_DATE");
        }
    }
}
