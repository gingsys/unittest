using AutoMapper;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class CalendarFilter
    {
        public int country { get; set; }
        public int year { get; set; }
        public int status { get; set; }
    }
    public class CalendarRepository : AxsBaseRepository
    {
        public CalendarRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.Calendar;
        }

        public IQueryable<CalendarDTO> GetCalendar(CalendarFilter filter = null)
        {
            var query = db.Database.SqlQuery<CalendarDTO>("SITCORE.SP_GET_CALENDAR_BY_REQUEST {0}, {1}, {2}", filter.country, filter.year, filter.status);
            return query.AsQueryable<CalendarDTO>();
        }

        private bool Validate(Calendar model, CalendarDTO dto)
        {
            // Fecha
            if (model.CalDate == null)
            {
                throw new ModelException("No ha asignado una fecha.");
            }

            // Descripción
            if (String.IsNullOrEmpty(model.CalDescription))
            {
                throw new ModelException("No ha asignado una descripción.");
            }

            // Valida que no exista otra fecha ya registrada
            if (db.Calendar.Count(t => t.CalDate.Day == model.CalDate.Day && t.CalDate.Month == model.CalDate.Month && t.CalAnual == true && t.CalIdCountry == model.CalIdCountry) > 0)
            {
                throw new ModelException("Ya existe una fecha anual registrada.");
            }

            if (db.Calendar.Count(t => t.CalDate == model.CalDate && t.CalIdCountry == model.CalID) > 0)
            {
                throw new ModelException("Ya existe una fecha registrada.");
            }

            return true;
        }

        public CalendarDTO SaveCalendar(CalendarDTO calendar, AccessUsers sessionUser)
        {
            try
            {
                Calendar model = null;
                if (calendar.CalID == 0)
                    model = db.Calendar.Create();  // create new from context
                else
                    model = db.Calendar.FirstOrDefault(a => a.CalID == calendar.CalID);  // get from context

                Mapper.Map<CalendarDTO, Calendar>(calendar, model);
                model.EditUser = sessionUser.UserInternalID;

                if (Validate(model, calendar))//&& ValidateAD(sessionUser, model))
                {
                    if (SaveEntity(model.CalID == 0, model))
                    {
                        calendar = AutoMapper.Mapper.Map<CalendarDTO>(model);
                        return calendar;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }

        public bool DeleteCalendar(int id)
        {
            try
            {
                Calendar model = db.Calendar.FirstOrDefault(a => a.CalID == id);
                //model.CalActive = 0;  // desactiva a la persona
                //SaveEntity(model.CalID == 0, model);
                DeleteEntity(model);
                return true;
            }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                return false;
            }
            
            //int count = 0;
            //bool canDelete = true;

            // Valida que no tenga Recursos Asignados
            //count = db.ResourcePeople.Count(x => x.PeopleID == id);
            //canDelete = canDelete && count == 0;

            //if (canDelete)
            ////{
            //    DeleteEntity(model);
            //    return null;
            //}
        }
    }
}
