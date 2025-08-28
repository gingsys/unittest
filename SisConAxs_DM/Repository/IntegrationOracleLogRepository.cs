using AutoMapper;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class IntegrationOracleLogFilter
    {
        public string LogType { get; set; }
        public int LogActive { get; set; } = 1;
    }

    public class IntegrationOracleLogRepository : AxsBaseRepository
    {
        public IntegrationOracleLogRepository()
        {
            dbSet = db.IntegrationOracleLog;
        }

        public IQueryable<IntegrationOracleLogDTO> GetLogByType(string type, int active = 1)
        {
            return GetQuery(x => x.LogType == type && (active == -1 || x.LogActive == active));
        }

        private IQueryable<IntegrationOracleLogDTO> GetQuery(Expression<Func<IntegrationOracleLogDTO, bool>> whereExpr)
        {
            var query = from l in db.IntegrationOracleLog
                        orderby
                            l.LogData1,
                            l.CreateDate
                        select new IntegrationOracleLogDTO() { 
                            LogID = l.LogID,
                            LogType = l.LogType,
                            LogData1 = l.LogData1,
                            LogData2 = l.LogData2,
                            LogMessage = l.LogMessage,
                            LogActive = l.LogActive,
                            CreateDate = l.CreateDate
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public IntegrationOracleLogDTO SaveOracleLog(IntegrationOracleLogDTO dto, string userID)
        {
            try
            {
                IntegrationOracleLog model = null;
                if (dto.LogID == 0)
                    model = db.IntegrationOracleLog.Create();  // create new from context
                else
                    model = db.IntegrationOracleLog.FirstOrDefault(a => a.LogID == dto.LogID);  // get from context

                Mapper.Map<IntegrationOracleLogDTO, IntegrationOracleLog>(dto, model);
                model.EditUser = userID;

                //if (Validate(model, dto))
                //{
                if (SaveEntity(dto.LogID == 0, model))
                {
                    dto = Mapper.Map<IntegrationOracleLogDTO>(model);
                    return dto;
                }
                //}
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }

        public IntegrationOracleLogDTO SaveOracleLog(SisConAxsContext db, IntegrationOracleLogDTO dto, string userID)
        {
            try
            {
                IntegrationOracleLog model = null;
                if (dto.LogID == 0)
                {
                    model = db.IntegrationOracleLog.Create();  // create new from context
                    model.CreateDate = DateTime.Now;
                }                    
                else
                    model = db.IntegrationOracleLog.FirstOrDefault(a => a.LogID == dto.LogID);  // get from context

                Mapper.Map<IntegrationOracleLogDTO, IntegrationOracleLog>(dto, model);
                model.EditUser = userID;

                if (dto.LogID == 0)
                {
                    db.IntegrationOracleLog.Add(model);
                }
                else
                {
                    db.IntegrationOracleLog.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                }

                if (db.SaveChanges() > 0)
                {
                    dto = Mapper.Map<IntegrationOracleLogDTO>(model);
                    return dto;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return null;
        }
    }
}
