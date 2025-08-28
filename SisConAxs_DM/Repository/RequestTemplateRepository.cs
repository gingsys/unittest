using AutoMapper;
using Microsoft.SharePoint.Client;
using SisConAxs_DM.DTO;
using SisConAxs_DM.DTO.Filters;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SisConAxs_DM.Repository
{
    public class RequestTemplateRepository : AxsBaseRepository
    {
        public RequestTemplateRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.RequestTemplate;
        }

        public IQueryable<RequestTemplateDTO> GetReqTemplates(RequestTemplateFilter filter = null)
        {
            var query = GetReqTemplatesQuery();
            if (filter != null)
            {
                if (filter.ReqTemplateCompany.HasValue)
                {
                    query = query.Where(rt => rt.ReqTemplateCompany == filter.ReqTemplateCompany.Value);
                }
            }
            return query;
        }

        public IQueryable<RequestTemplateDTO> GetReqTemplatesQuery(Expression<Func<RequestTemplateDTO, bool>> whereExpr = null)
        {
            var query = from template in db.RequestTemplate
                        join company in db.Companies on template.ReqTemplateCompany equals company.CompanyID
                        join reqTemplateType in db.CommonValues on template.ReqTemplateType equals reqTemplateType.CommonValueID
                        join reqTemplateEmployeeType in db.CommonValues on template.ReqTemplateEmployeeType equals reqTemplateEmployeeType.CommonValueID
                        //orderby
                        //    reqTemplateEmployeeType.CommonValueDesc
                        select new RequestTemplateDTO()
                        {
                            ReqTemplateID = template.ReqTemplateID,
                            ReqTemplateCompany = template.ReqTemplateCompany,
                            ReqTemplateCompanyName = company.CompanyDisplay,
                            ReqTemplateType = template.ReqTemplateType,
                            ReqTemplateTypeName = reqTemplateType.CommonValueDisplay,
                            ReqTemplateEmployeeType = template.ReqTemplateEmployeeType,
                            ReqTemplateEmployeeTypeName = reqTemplateEmployeeType.CommonValueDisplay,
                            ReqTemplateActive = template.ReqTemplateActive
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public RequestTemplateDTO GetReqTemplateByID(int id)
        {
            var dto = GetReqTemplatesQuery(t => t.ReqTemplateID == id).FirstOrDefault();
            dto.ReqTemplateDetails = (from details in db.RequestTemplateDetail
                                      join resource in db.AccessResources on details.ReqTemplateDetResourceID equals resource.ResourceID
                                      join category in db.ResourceCategories on resource.ResourceCategory equals category.CategoryID
                                      where
                                         details.ReqTemplateID == id
                                      orderby
                                         category.CategoryName,
                                         resource.ResourceFullName
                                      select new RequestTemplateDetailDTO()
                                      {
                                          ReqTemplateID = details.ReqTemplateID,
                                          ReqTemplateDetID = details.ReqTemplateDetID,

                                          ReqTemplateDetCategoryID = resource.ResourceCategory,
                                          ReqTemplateDetCategoryName = category.CategoryName,
                                          ReqTemplateDetResourceID = details.ReqTemplateDetResourceID,
                                          ReqTemplateDetResourceName = resource.ResourceName,
                                          ReqTemplateDetResourceFullName = resource.ResourceFullName,
                                          ReqTemplateDetAccessTypeID = resource.ResourceAccessType,

                                          ReqTemplateDetStrValue = details.ReqTemplateDetStrValue,
                                          ReqTemplateDetIntValue = details.ReqTemplateDetIntValue,
                                          ReqTemplateDetTemporal = details.ReqTemplateDetTemporal,
                                          ReqTemplateDetValidityFrom = details.ReqTemplateDetValidityFrom,
                                          ReqTemplateDetValidityUntil = details.ReqTemplateDetValidityUntil,
                                          ReqTemplateDetAdditional = details.ReqTemplateDetAdditional,
                                          ReqTemplateDetAdditionalStrValue = details.ReqTemplateDetAdditionalStrValue,
                                          ReqTemplateDetAdditionalIntValue = details.ReqTemplateDetAdditionalIntValue
                                      }
                                    ).ToList();
            return dto;
        }

        public RequestTemplateDTO InsertReqTemplate(RequestTemplateDTO dto, string editUser)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                var model = db.RequestTemplate.Create();
                Mapper.Map<RequestTemplateDTO, RequestTemplate>(dto, model);
                model.ReqTemplateCompany = this.sessionData.CompanyID;
                model.EditUser = editUser;
                PrepareDetail(dto, model, editUser);

                if (ValidateTemplate(model))
                {
                    if (SaveEntity(true, model))
                    {
                        transaction.Commit();
                        dto = AutoMapper.Mapper.Map<RequestTemplateDTO>(model);
                        return dto;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }

        public RequestTemplateDTO UpdateReqTemplate(RequestTemplateDTO dto, string editUser)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                var model = db.RequestTemplate.FirstOrDefault(a => a.ReqTemplateID == dto.ReqTemplateID);
                model.ReqTemplateEmployeeType = dto.ReqTemplateEmployeeType;
                model.ReqTemplateActive = dto.ReqTemplateActive;
                model.EditUser = editUser;
                PrepareDetail(dto, model, editUser);

                if (ValidateTemplate(model))
                {
                    if (SaveEntity(false, model))
                    {
                        transaction.Commit();
                        dto = AutoMapper.Mapper.Map<RequestTemplateDTO>(model);
                        return dto;
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }


        public void PrepareDetail(RequestTemplateDTO dto, RequestTemplate model, string editUser)
        {
            var removeDetails = model.ReqTemplateDetails.Where(md => !dto.ReqTemplateDetails.Any(dd => dd.ReqTemplateDetID == md.ReqTemplateDetID)).ToList();
            foreach (var detail in removeDetails)
            {
                db.RequestTemplateDetail.Remove(detail);
                model.ReqTemplateDetails.Remove(detail);
            }
            var addDetails = dto.ReqTemplateDetails.Where(dd => !model.ReqTemplateDetails.Any(md => md.ReqTemplateDetResourceID == dd.ReqTemplateDetResourceID)).ToList();
            foreach (var detail in addDetails)
            {
                model.ReqTemplateDetails.Add(AutoMapper.Mapper.Map<RequestTemplateDetail>(detail));
            }
            foreach (var detail in model.ReqTemplateDetails)
            {
                var dtoDetail = dto.ReqTemplateDetails.FirstOrDefault(d => d.ReqTemplateDetResourceID == detail.ReqTemplateDetResourceID);
                AutoMapper.Mapper.Map<RequestTemplateDetailDTO, RequestTemplateDetail>(dtoDetail, detail);
                detail.EditUser = editUser;
            }
        }

        public bool ValidateTemplate(RequestTemplate model)
        {
            var validEmployeeType = db.RequestTemplate.Any(rt => rt.ReqTemplateID != model.ReqTemplateID && rt.ReqTemplateCompany == model.ReqTemplateCompany && rt.ReqTemplateEmployeeType == model.ReqTemplateEmployeeType);
            if (validEmployeeType)
            {
                throw new ModelException("Existe un registro para este tipo de empleado.");
            }

            // details
            if (model.ReqTemplateDetails.Count == 0)
            {
                throw new ModelException("No tiene detalle.");
            }
            foreach (var item in model.ReqTemplateDetails)
            {
                var resource = db.AccessResources.FirstOrDefault(t => t.ResourceID == item.ReqTemplateDetResourceID);
                if (resource == null)
                {
                    throw new ModelException($"El recurso con ID '{item.ReqTemplateDetResourceID}' no existe.");
                }
                if (resource.Workflows == null)
                {
                    throw new ModelException($"El recurso '{resource.ResourceName}' no tiene workflow.");
                }
                var wfItemsCount = resource.Workflows.WorkflowItems.Count;
                if (wfItemsCount == 0)
                {
                    throw new ModelException($"El workflow '{resource.Workflows.WfName}' del recurso '{resource.ResourceName}' no tiene acciones.");
                }
            }
            var repeatResources = model.ReqTemplateDetails.GroupBy(d => d.ReqTemplateDetResourceID).Any(g => g.Count() > 1);
            if (repeatResources)
            {
                throw new ModelException($"El detalle tiene recursos repetidos.");
            }
            return true;
        }

        public bool DeleteReqTemplate(int id)
        {
            var model = db.RequestTemplate.FirstOrDefault(a => a.ReqTemplateID == id);
            return DeleteEntity(model);
        }


        public AccessRequestDTO BuildRequestDTOFromTemplate(int id)
        {
            RequestTemplateDTO template = GetReqTemplateByID(id);

            AccessRequestDTO dto = new AccessRequestDTO() {
                RequestType = template.ReqTemplateType,
                RequestPriority = AccessRequestDTO.PRIORITY_NORMAL
            };
            dto.AccessRequestDetails = template.ReqTemplateDetails.Select(
                d => new AccessRequestDetailsDTO()
                {
                    ResourceID = d.ReqTemplateDetResourceID,
                    RequestDetStrValue = d.ReqTemplateDetStrValue,
                    RequestDetIntValue = d.ReqTemplateDetIntValue,
                    ReqDetTemporal = d.ReqTemplateDetTemporal,
                    ReqDetValidityFrom= d.ReqTemplateDetValidityFrom,
                    ReqDetValidityUntil= d.ReqTemplateDetValidityUntil,
                    RequestDetAdditional = d.ReqTemplateDetAdditional,
                    RequestDetAdditionalStrValue = d.ReqTemplateDetAdditionalStrValue,
                    RequestDetAdditionalIntValue = d.ReqTemplateDetAdditionalIntValue
                }
            ).ToList();
            return dto;
        }
    }
}
