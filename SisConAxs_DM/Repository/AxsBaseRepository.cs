using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Entity.Validation;

namespace SisConAxs_DM.Repository
{
    using SisConAxs_DM.Models;
    using SisConAxs_DM.DTO;

    public class AxsBaseRepository
    {
        protected SisConAxsContext db = new SisConAxsContext();
        protected DbSet dbSet;
        protected SessionData sessionData;

        public AxsBaseRepository()
        {
        }

        static public string GetCurrentDateStr()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public bool SaveEntity(bool IsNew, object model)
        {
            try
            {
                if (!IsNew)
                {
                    dbSet.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    
                }
                else
                {
                    dbSet.Add(model);
                }
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }

        }

        public bool DeleteEntity(object model)
        {
            return AxsBaseRepository.DeleteEntity(model, db, dbSet);
        }

        static public bool DeleteEntity(object model, SisConAxsContext db, DbSet dbSet)
        {
            try
            {
                if (model != null)
                {
                    dbSet.Remove(model);
                    return db.SaveChanges() > 0;
                }
                throw new ModelException("Este registro no existe");
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        /*private void PrepareDetails<DTO, MODEL>(ref DTO dto, ref MODEL model)
        {
            CommonValues modelItem;
            CommonValuesDTO dtoItem;
            List<CommonValues> listToRemove = new List<CommonValues>();

            // update and find items to remove
            foreach (CommonValues item in model.CommonValues)
            {
                dtoItem = dto.CommonValues.FirstOrDefault(cv => cv.CommonValueID == item.CommonValueID);
                if (dtoItem != null)
                {
                    Mapper.Map<CommonValuesDTO, CommonValues>(dtoItem, item);
                    item.EditUser = model.EditUser;
                }
                else
                {
                    listToRemove.Add(item);
                }
            }
            // remove items
            foreach (CommonValues item in listToRemove)
            {
                db.CommonValues.Remove(item);
                model.CommonValues.Remove(item);
            }
            // add new items
            foreach (CommonValuesDTO item in dto.CommonValues.Where(at => at.CommonValueID == 0))
            {
                modelItem = AutoMapper.Mapper.Map<CommonValues>(item);
                modelItem.EditUser = model.EditUser;
                model.CommonValues.Add(modelItem);
            }
        }*/



        static protected void HandleException(Exception ex)
        {
            Exception exBase = ex.GetBaseException();
            if (exBase is SqlException)
            {
                HandleSQLException(exBase as SqlException);
            }
            if (exBase is DbEntityValidationException)
            {
                HandleDbEntityValidationException(exBase as DbEntityValidationException);
            }
            Console.Write(ex.Message);
            throw new ModelException("Error al ejecutar la operación.", ex);
        }

        static protected void HandleSQLException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 547: throw new ModelException("Este registro tiene dependencias.", ex); // FK exception
                case 2627:
                case 2601:
                    throw new ModelException("Ya existe un registro con este código.", ex); // primary key exception
                default: throw new ModelException("Error al ejecutar la operación.", ex); // others
            }
        }

        static private void HandleDbEntityValidationException(DbEntityValidationException ex)
        {
            string message = "DbEntityValidationException\n\n";
            foreach (DbEntityValidationResult entityError in ex.EntityValidationErrors)
            {
                message += "> " + entityError.Entry.Entity.ToString() + "\n\n";
                foreach (DbValidationError dbError in entityError.ValidationErrors)
                {
                    message += dbError.PropertyName + " : " + dbError.ErrorMessage + "\n";
                }
                message += "\n-------------------------------------------------------------\n\n";
            }

            // aqui iria el log -----------------------------------------------
            Console.Write(message);

            throw new ModelException("Error al ejecutar la operación.", ex);
        }

        private string PrepareParams(SqlParameter[] parameters = null)
        {
            if (parameters != null)
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    parameters[i].Value = parameters[i].Value ?? DBNull.Value;
                }
                return String.Join(", ", parameters.Select((p, index) => $"{p.ParameterName} = {p.ParameterName}"));
            }
            return "";
        }

        public int ExecuteSP(string name, SqlParameter[] parameters = null, SisConAxsContext dbContext = null)
        {
            parameters = parameters ?? new SqlParameter[] { };
            return (dbContext ?? this.db).Database.ExecuteSqlCommand($"{name} {PrepareParams(parameters)}", parameters);
        }
        public T SelectOne<T>(string name, SqlParameter[] parameters = null, SisConAxsContext dbContext = null)
        {
            parameters = parameters ?? new SqlParameter[] { };
            return (dbContext ?? this.db).Database.SqlQuery<T>($"{name} {PrepareParams(parameters)}", parameters).FirstOrDefault();
        }
        public List<T> Select<T>(string name, SqlParameter[] parameters = null, SisConAxsContext dbContext = null)
        {
            parameters = parameters ?? new SqlParameter[] { };
            return (dbContext ?? this.db).Database.SqlQuery<T>($"{name} {PrepareParams(parameters)}", parameters).ToList();
        }
    }
}
