using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class ResourcePeopleFilter
    {
        public int onlyActive { get; set; }
    }

    public class ResourcePeopleRepository : AxsBaseRepository
    {
        public ResourcePeopleRepository()
        {
            dbSet = db.ResourcePeople;
        }

        public IQueryable<ResourcePeopleDTO> GetByPeopleID(int id, bool onlyActive = true)
        {
            return from rp in db.ResourcePeople
                   join res in db.AccessResources on rp.ResourceID equals res.ResourceID
                   join cat in db.ResourceCategories on res.ResourceCategory equals cat.CategoryID
                   join c in db.Companies on rp.PresCompany equals c.CompanyID
                   join addedRequest in db.AccessRequests on rp.AddedRequestID equals addedRequest.RequestID into AddedRequest
                   from addedRequest in AddedRequest.DefaultIfEmpty()
                   join removedRequest in db.AccessRequests on rp.RemovedRequestID equals removedRequest.RequestID into RemovedRequest
                   from removedRequest in RemovedRequest.DefaultIfEmpty()
                   where
                        (!onlyActive || (onlyActive && rp.PresActive > 0))
                        && rp.PeopleID == id
                   orderby
                        c.CompanyName,
                        cat.CategoryName,
                        res.ResourceFullName
                   select new ResourcePeopleDTO()
                   {
                       PeopleID = rp.PeopleID,
                       ResourceID = rp.ResourceID,
                       AddedRequestID = rp.AddedRequestID,
                       AddedRequestNum = addedRequest.RequestNumber,
                       AddedRequestDetID = rp.AddedRequestDetID,
                       RemovedRequestID = rp.RemovedRequestID,
                       RemovedRequestNum = removedRequest.RequestNumber,
                       RemovedRequestDetID = rp.RemovedRequestDetID,
                       //PresIntValue = rp.PresIntValue,
                       //PresStrValue = rp.PresStrValue,
                       //PresDateValue = rp.PresDateValue,
                       PresDateStart = rp.PresDateStart,
                       PresDateEnd = rp.PresDateEnd,
                       PresActive = rp.PresActive,
                       PresTemporal = rp.PresTemporal,
                       PresValidityFrom = rp.PresValidityFrom,
                       PresValidityUntil = rp.PresValidityUntil,
                       PeopleDepartment = rp.PeopleDepartment,

                       PresAdditional = rp.PresAdditional,
                       PresAdditionalStrValue = rp.PresAdditionalStrValue,
                       PresAdditionalIntValue = rp.PresAdditionalIntValue,

                       CategoryName = cat.CategoryName,
                       ResourceFullName = res.ResourceFullName,
                       PresDisplayValue = rp.PresDisplayValue,

                       PresCompany = rp.PresCompany,
                       PresCompanyName = c.CompanyName,

                       ResourcePeopleLog = (from log in db.ResourcePeopleLog
                                            join action in db.CommonValues on log.Action equals action.CommonValueID
                                            where
                                                log.ResourcePeopleID == rp.PresID
                                            select new ResourcePeopleLogDTO()
                                            {
                                                Action = log.Action,
                                                ActionDisplay = action.CommonValueDisplay,
                                                Source = log.Source,
                                                Description = log.Description,
                                                CreateUser = log.CreateUser,
                                                CreateDate = log.CreateDate
                                            }).ToList()
                   };
        }

        // Guarda los permisos del usuario
        public void SaveResourcePeople(SisConAxsContext db, AccessRequestDetails requestDetail, string user = "icarus.taskmanager")
        {
            if (requestDetail.RequestDetStatus == AccessRequestDTO.STATUS_APPROVE)
            {
                AccessRequests request = requestDetail.AccessRequest;
                PeopleDTO people = new PeopleRepository(null).GetPeopleById(request.RequestTo).FirstOrDefault();
                ResourcePeople model = db.ResourcePeople.FirstOrDefault(t => t.PeopleID == request.RequestTo
                                                                             && t.ResourceID == requestDetail.ResourceID
                                                                             && t.PresActive == 1);

                switch (requestDetail.RequestDetType)
                {
                    case AccessRequestDTO.TYPE_ALTA:
                        if (model != null)
                        {
                            throw new ModelException($"La persona '{people.PeopleFullname}' ya tiene un registro para el ALTA del recurso '{requestDetail.ResourceFullName}'");
                        }
                        SaveSubscribe(db, model, requestDetail, user);
                        //db.SaveChanges();
                        break;
                    case AccessRequestDTO.TYPE_MODIFICACION:
                        if (model == null)
                        {
                            throw new ModelException($"La persona '{people.PeopleFullname}' NO tiene un registro para dar de BAJA al recurso '{requestDetail.ResourceFullName}'");
                        }
                        SaveUnSubscribe(db, model, requestDetail, user);
                        SaveSubscribe(db, model, requestDetail, user);
                        //db.SaveChanges();
                        break;
                    case AccessRequestDTO.TYPE_BAJA:
                        if (model == null)
                        {
                            throw new ModelException($"La persona '{people.PeopleFullname}' NO tiene un registro para dar de BAJA al recurso '{requestDetail.ResourceFullName}'");
                        }
                        SaveUnSubscribe(db, model, requestDetail, user);
                        //db.SaveChanges();
                        break;
                    default:
                        throw new Exception($"Error al grabar los permisos para persona '{people.PeopleFullname}' en la solicitud # [{request.RequestNumber}] - Recurso: '{requestDetail.ResourceFullName}'");
                }
            }
        }

        private void SaveSubscribe(SisConAxsContext db, ResourcePeople model, AccessRequestDetails requestDetail,  string user)
        {
            AccessRequests request = requestDetail.AccessRequest;
            model = db.ResourcePeople.Create();
            model.PeopleID = request.RequestTo;                                           // <= Persona
            model.PeopleDepartment = request.RequestDepartment;
            model.ResourceID = requestDetail.ResourceID;                                  // <= Recurso
            model.ResourceFullName = requestDetail.ResourceFullName;
            model.AddedRequestID = requestDetail.RequestID;
            model.AddedRequestDetID = requestDetail.RequestDetID;
            model.PresIntValue = requestDetail.RequestDetIntValue;
            model.PresStrValue = requestDetail.RequestDetStrValue;
            model.PresDateValue = null; // no usado por ahora
            model.PresDateStart = DateTime.Now;
            model.PresDateEnd = null;
            model.PresActive = 1;                                                         // <= Accion: ALTA
            model.PresTemporal = requestDetail.ReqDetTemporal;
            model.PresValidityFrom = requestDetail.ReqDetValidityFrom;
            model.PresValidityUntil = requestDetail.ReqDetValidityUntil;
            model.PresAdditional = requestDetail.RequestDetAdditional;
            model.PresAdditionalIntValue = requestDetail.RequestDetAdditionalIntValue;
            model.PresAdditionalStrValue = requestDetail.RequestDetAdditionalStrValue;
            model.PresDisplayValue = requestDetail.RequestDetDisplayValue;
            model.PresCompany = request.RequestCompany;                                   // <= Empresa
            model.EditUser = user;

            db.ResourcePeople.Add(model);
        }

        private void SaveUnSubscribe(SisConAxsContext db, ResourcePeople model, AccessRequestDetails requestDetail, string user)
        {
            model.PresActive = 0;                                                         // <= Accion: BAJA
            model.RemovedRequestID = requestDetail.RequestID;                             // Lo demas funciona igual
            model.RemovedRequestDetID = requestDetail.RequestDetID;
            model.PresDateEnd = DateTime.Now;
            model.EditUser = user;

            db.ResourcePeople.Attach(model);
            db.Entry(model).State = EntityState.Modified;
        }

        public void SaveResourcePeople(ResourcePeople resourcePeopleInput, int type = AccessRequestDTO.TYPE_ALTA)
        {
            var db = new SisConAxsContext();
            this.SaveResourcePeople(db, resourcePeopleInput, type);
            db.SaveChanges();
        }

        public void SaveResourcePeople(SisConAxsContext db, ResourcePeople resourcePeopleInput, int type = AccessRequestDTO.TYPE_ALTA)
        {
            var rp = new ResourcePeople();
            AutoMapper.Mapper.Map<ResourcePeople, ResourcePeople>(resourcePeopleInput, rp);  // se copia el objeto para evitar bloqueos al usar el mismo contexto

            PeopleDTO people = new PeopleRepository(null).GetPeopleById(resourcePeopleInput.PeopleID).FirstOrDefault();
            ResourcePeople model = db.ResourcePeople.FirstOrDefault(t => t.PeopleID == people.PeopleID
                                                                         && t.ResourceID == rp.ResourceID
                                                                         && t.PresActive == 1);

            switch (type)
            {
                case AccessRequestDTO.TYPE_ALTA:
                    if (model != null)
                    {
                        throw new ModelException($"La persona '{people.PeopleFullname}' ya tiene un registro para el ALTA del recurso '{rp.ResourceFullName}'");
                    }

                    model = db.ResourcePeople.Create();
                    model.PeopleID = people.PeopleID;                                       // <= Persona
                    model.PeopleDepartment = people.PeopleDepartment;
                    model.ResourceID = rp.ResourceID;                                       // <= Recurso
                    model.ResourceFullName = rp.ResourceFullName;
                    //model.AddedRequestID = resource.RequestID;
                    //model.AddedRequestDetID = resource.RequestDetID;
                    //model.PresIntValue = resource.RequestDetIntValue;
                    //model.PresStrValue = resource.RequestDetStrValue;
                    //model.PresDateValue = null; // no usado por ahora
                    model.PresDateStart = DateTime.Now;
                    model.PresDateEnd = null;
                    model.PresActive = 1;                                                   // <= Accion: ALTA
                    model.PresTemporal = rp.PresTemporal;
                    model.PresValidityFrom = rp.PresValidityFrom;
                    model.PresValidityUntil = rp.PresValidityUntil;
                    model.PresAdditional = rp.PresAdditional;
                    model.PresAdditionalIntValue = rp.PresAdditionalIntValue;
                    model.PresAdditionalStrValue = rp.PresAdditionalStrValue;
                    model.PresDisplayValue = rp.PresDisplayValue;
                    model.PresCompany = rp.PresCompany;                                     // <= Empresa
                    model.EditUser = rp.EditUser;

                    foreach (var log in rp.ResourcePeopleLog)
                    {
                        if (log.ResourcePeopleLogID == 0) // solo agrega los nuevos
                        {
                            log.Action = AccessRequestDTO.TYPE_ALTA;
                            log.CreateDate = DateTime.Now;
                            log.EditUser = rp.EditUser;
                            model.ResourcePeopleLog.Add(log);
                        }
                    }

                    db.ResourcePeople.Add(model);
                    //db.SaveChanges();
                    break;
                case AccessRequestDTO.TYPE_BAJA:
                    if (model == null)
                    {
                        throw new ModelException($"La persona '{people.PeopleFullname}' NO tiene un registro para dar de BAJA al recurso '{rp.ResourceFullName}'");
                    }

                    model.PresActive = 0;                                                   // <= Accion: BAJA
                    //model.RemovedRequestID = resource.RequestID;                          // Lo demas funciona igual
                    //model.RemovedRequestDetID = resource.RequestDetID;
                    model.PresDateEnd = DateTime.Now;
                    model.EditUser = rp.EditUser;

                    foreach (var log in rp.ResourcePeopleLog)
                    {
                        if (log.ResourcePeopleLogID == 0) // solo agrega los nuevos
                        {
                            log.Action = AccessRequestDTO.TYPE_BAJA;
                            log.CreateDate = DateTime.Now;
                            log.EditUser = rp.EditUser;
                            model.ResourcePeopleLog.Add(log);
                        }
                    }

                    db.ResourcePeople.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    //db.SaveChanges();
                    break;
                default:
                    throw new ModelException($"Error al grabar los permisos para persona '{people.PeopleFullname}' en la sincronización - Recurso: '{rp.ResourceFullName}'");
            }
        }
    }
}
