using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Data.SqlClient;
using System.Data.Entity;

namespace SisConAxs_DM.Repository
{
    public class SystemConfigRepository : AxsBaseRepository
    {
        public SystemConfigRepository()
        {
            dbSet = db.SystemConfig;
        }

        public NotifConfigDTO GetADfilterConfig()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "AD_FILTER");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public NotifConfigDTO GetADdomainConfig()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "AD_DOMAIN");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public NotifConfigDTO GetWebHostConfig()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "WEB_HOST_CONFIG");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public NotifConfigDTO GetNotifConfig()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "NOTIFICATION_CONFIG");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public NotifConfigDTO GetInitLoadFlag()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "INITIAL_LOAD_FLAG");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public NotifConfigDTO GetExcelFilter()
        {
            var query = GetSystemConfig().Where(sc => sc.SysConfName == "ALLOWED_CATEGORY_EXCEL");
            return Mapper.Map<SystemConfig, NotifConfigDTO>(query.FirstOrDefault());
        }

        public SystemConfigDTO GetFromName(string name)
        {
            var query = from s in db.SystemConfig
                        where
                            s.SysConfName == name
                        select s;
            return Mapper.Map<SystemConfigDTO>(query.FirstOrDefault());
        }
        public NotifConfigDTO SaveNotifConfig(NotifConfigDTO notifConfig, string userId)
        {
            try
            {
                SystemConfig model = null;
                if (notifConfig.NotifConfID == 0)
                    model = db.SystemConfig.Create();  // create new from context
                else
                {
                    if (notifConfig.NotifConfName == "NOTIFICATION_CONFIG")
                        notifConfig.NotifConfDesc = "Configuración de Correo para envío de Notificaciones.";

                    model = db.SystemConfig.FirstOrDefault(a => a.SysConfID == notifConfig.NotifConfID);  // get from context
                }

                Mapper.Map<NotifConfigDTO, SystemConfig>(notifConfig, model);
                model.EditUser = userId;

                if (Validate(model))
                {
                    if (SaveEntity(model.SysConfID == 0, model))
                    {
                        notifConfig = AutoMapper.Mapper.Map<NotifConfigDTO>(model);
                        return notifConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }

        public IQueryable<SystemConfig> GetSystemConfig()
        {
            var query = from at in db.SystemConfig
                        select at;
            return query;
        }


        private bool Validate(SystemConfig model)
        {

            return true;
        }

        public bool ValidateHoliday(DateTime date, int countryID)
        {
            var query = db.Database.SqlQuery<bool>("SELECT SITCORE.SP_CHECK_HOLIDAY({0}, {1})", date, countryID);
            return query.FirstOrDefault();
        }
    }



}
