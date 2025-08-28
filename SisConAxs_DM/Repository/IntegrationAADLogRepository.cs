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
    public class IntegrationAADLogRepository : AxsBaseRepository
    {
        public IntegrationAADLogRepository()
        {
            dbSet = db.IntegrationAADLog;
        }

        public IQueryable<IntegrationAADLogDTO> GetLogByResult(string result) //, bool? lastChange)
        {
            return GetQuery(x => x.LogResult == result); // && x.LogLastChange == (lastChange ?? x.LogLastChange));
        }

        private IQueryable<IntegrationAADLogDTO> GetQuery(Expression<Func<IntegrationAADLogDTO, bool>> whereExpr)
        {
            var query = from l in db.IntegrationAADLog
                        orderby
                            l.CreateDate descending
                        select new IntegrationAADLogDTO()
                        {
                            LogID = l.LogID,
                            LogResult = l.LogResult,

                            LogDocNumber = l.LogDocNumber,
                            LogUserName = l.LogUserName,
                            LogNames = l.LogNames,
                            LogLastnames = l.LogLastnames,
                            LogCompanyName = l.LogCompanyName,
                            LogEmail = l.LogEmail,
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

        public IntegrationAADLogDTO SaveAADLog(IntegrationAADLogDTO dto, string userID)
        {
            try
            {
                IntegrationAADLog model = null;
                if (dto.LogID == 0)
                {
                    model = db.IntegrationAADLog.Create();  // create new from context
                    model.CreateDate = DateTime.Now;
                }
                else
                    model = db.IntegrationAADLog.FirstOrDefault(a => a.LogID == dto.LogID);  // get from context

                Mapper.Map<IntegrationAADLogDTO, IntegrationAADLog>(dto, model);
                if (dto.LogException != null)
                {
                    model.LogMessage += ": " + dto.LogException.Message;
                    if (dto.LogException.InnerException != null)
                    {
                        model.LogMessage += "\n\n[InnerException>\n" + dto.LogException.InnerException;
                    }
                    model.LogMessage += "\n\n[StackTrace>\n" + dto.LogException.StackTrace;
                }
                model.CreateUser = userID;
                if (model.LogNames == null)
                {
                    model.LogNames = "";
                }
                if (model.LogLastnames == null)
                {
                    model.LogLastnames = "";
                }
                if (model.LogDocNumber == null)
                {
                    model.LogDocNumber = "";
                }
                if (model.LogCompanyName == null)
                {
                    model.LogCompanyName = "";
                }

                if (SaveEntity(dto.LogID == 0, model))
                {
                    dto = Mapper.Map<IntegrationAADLogDTO>(model);
                    return dto;
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }        
    }
}
