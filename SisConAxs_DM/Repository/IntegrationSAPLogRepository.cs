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
using static SisConAxs_DM.Utils.LogManager;

namespace SisConAxs_DM.Repository
{
    public class IntegrationSAPLogRepository : AxsBaseRepository
    {
        public IntegrationSAPLogRepository()
        {
            dbSet = db.IntegrationSAPLog;
        }

        public IQueryable<IntegrationSAPLogDTO> GetLogByType(int type) //, bool? lastChange)
        {
            return GetQuery(x => x.LogType == type); // && x.LogLastChange == (lastChange ?? x.LogLastChange));
        }

        private IQueryable<IntegrationSAPLogDTO> GetQuery(Expression<Func<IntegrationSAPLogDTO, bool>> whereExpr)
        {
            var query = from l in db.IntegrationSAPLog
                        orderby
                            l.CreateDate descending
                        select new IntegrationSAPLogDTO()
                        {
                            LogID = l.LogID,
                            LogType = l.LogType,
                            LogResult = l.LogResult,

                            LogTypeDate = l.LogTypeDate,
                            LogDocNumber = l.LogDocNumber,
                            LogNames = l.LogNames,
                            LogLastnames = l.LogLastnames,
                            LogCompanyName = l.LogCompanyName,
                            LogMessage = l.LogMessage,
                            //LogLastChange = l.LogLastChange,
                            CreateUser = l.CreateUser,
                            CreateDate = l.CreateDate
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public IntegrationSAPLogDTO SaveSAPLog(IntegrationSAPLogDTO dto, string userID)
        {
            try
            {
                IntegrationSAPLog model = null;
                if (dto.LogID == 0)
                {
                    model = db.IntegrationSAPLog.Create();  // create new from context
                    model.CreateDate = DateTime.Now;
                }
                else
                    model = db.IntegrationSAPLog.FirstOrDefault(a => a.LogID == dto.LogID);  // get from context

                Mapper.Map<IntegrationSAPLogDTO, IntegrationSAPLog>(dto, model);
                if (dto.LogException != null)
                {
                    //model.LogResult = IntegrationSAPLogDTO.RESULT_ERROR;
                    model.LogMessage += ": " + dto.LogException.Message;
                    if (dto.LogException.InnerException != null)
                    {
                        model.LogMessage += "\n\n[InnerException>\n" + dto.LogException.InnerException;
                    }
                    model.LogMessage += "\n\n[StackTrace>\n" + dto.LogException.StackTrace;
                }
                model.CreateUser = userID;

                if (SaveEntity(dto.LogID == 0, model))
                {
                    dto = Mapper.Map<IntegrationSAPLogDTO>(model);
                    return dto;
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }

        //public IntegrationSAPLogDTO SaveOracleLog(SisConAxsContext db, IntegrationSAPLogDTO dto, string userID)
        //{
        //    try
        //    {
        //        IntegrationSAPLog model = null;
        //        if (dto.LogID == 0)
        //        {
        //            model = db.IntegrationSAPLog.Create();  // create new from context
        //            model.CreateDate = DateTime.Now;
        //        }
        //        else
        //            model = db.IntegrationSAPLog.FirstOrDefault(a => a.LogID == dto.LogID);  // get from context

        //        Mapper.Map<IntegrationSAPLogDTO, IntegrationSAPLog>(dto, model);
        //        model.EditUser = userID;

        //        if (dto.LogID == 0)
        //        {
        //            db.IntegrationSAPLog.Add(model);
        //        }
        //        else
        //        {
        //            db.IntegrationSAPLog.Attach(model);
        //            db.Entry(model).State = EntityState.Modified;
        //        }

        //        if (db.SaveChanges() > 0)
        //        {
        //            dto = Mapper.Map<IntegrationSAPLogDTO>(model);
        //            return dto;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        HandleException(ex);
        //    }
        //    return null;
        //}
    }
}
