using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Linq.Expressions;
using SisConAxs_DM.DTO.Filters;

namespace SisConAxs_DM.Repository
{

    public class WorkflowApproveHierarchyRepository : AxsBaseRepository
    {

        public WorkflowApproveHierarchyRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.WorkflowApproveHierarchy;
        }

        public IQueryable<WorkflowApproveHierarchyDTO> GetWfApproveHierarchyQuery(Expression<Func<WorkflowApproveHierarchyDTO, bool>> whereExpr = null, bool withDetails = false)
        {
            var query = from ah in db.WorkflowApproveHierarchy
                        where
                            ah.WfApproveHierarchyCompany == this.sessionData.CompanyID
                        orderby
                            ah.WfApproveHierarchyName
                        select new WorkflowApproveHierarchyDTO()
                        {
                            WfApproveHierarchyID = ah.WfApproveHierarchyID,
                            WfApproveHierarchyName = ah.WfApproveHierarchyName,
                            WfApproveHierarchyDepartment = ah.WfApproveHierarchyDepartment,
                            WfApproveHierarchyDepartmentName = ah.Department.CommonValueDisplay,
                            WorkflowHierarchyMembers = (from hm in db.WorkflowHierarchyMembers
                                                        join company in db.Companies on hm.WfHierarchyMemberCompany equals company.CompanyID
                                                        join dep in db.CommonValues on hm.WfHierarchyMemberDepartment equals dep.CommonValueID
                                                        join pos in db.CommonValues on hm.WfHierarchyMemberPosition equals pos.CommonValueID
                                                        where
                                                             hm.WfApproveHierarchyID == (withDetails ? ah.WfApproveHierarchyID : 0)
                                                        orderby hm.WfHierarchyMemberOrder
                                                        select new WorkflowHierarchyMembersDTO()
                                                        {
                                                            WfApproveHierarchyID = hm.WfApproveHierarchyID,
                                                            WfHierarchyMemberID = hm.WfHierarchyMemberID,
                                                            WfHierarchyMemberCompany = company.CompanyID,
                                                            WfHierarchyMemberCompanyName = company.CompanyName,
                                                            WfHierarchyMemberCompanyDisplay = company.CompanyDisplay,
                                                            WfHierarchyMemberDepartment = hm.WfHierarchyMemberDepartment,
                                                            WfHierarchyMemberDepartmentName = dep.CommonValueDisplay,
                                                            WfHierarchyMemberPosition = hm.WfHierarchyMemberPosition,
                                                            WfHierarchyMemberPositionName = pos.CommonValueDisplay,
                                                            WfHierarchyMemberOrder = hm.WfHierarchyMemberOrder,
                                                            WfHierarchyMemberDescription = hm.WfHierarchyMemberDescription,
                                                            WfHierarchyMemberPerson = (from p in db.People
                                                                                       where p.PeopleDepartment == hm.WfHierarchyMemberDepartment
                                                                                       && p.PeoplePosition == hm.WfHierarchyMemberPosition
                                                                                       //&& p.PeopleCompany == this.sessionData.CompanyID
                                                                                       && p.PeopleStatus == 1
                                                                                       select (p.PeopleFirstName + " " + p.PeopleFirstName2 + " " + p.PeopleLastName + " " + p.PeopleLastName2)
                                                                                       ).FirstOrDefault()
                                                        }).ToList()
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public IQueryable<WorkflowApproveHierarchyDTO> GetWfApproveHierarchy(WfHierarchyFilter filter = null)
        {
            return GetWfApproveHierarchyQuery(withDetails: filter != null ? filter.WithDetails : false);
        }
        public IQueryable<WorkflowApproveHierarchyDTO> GetWfApproveHierarchyById(int id)
        {
            return GetWfApproveHierarchyQuery(whereExpr: ah => ah.WfApproveHierarchyID == id, withDetails: true);
        }


        public WorkflowApproveHierarchyDTO SaveWorkflowApproveHierarchy(WorkflowApproveHierarchyDTO dto, string userId)
        {
            try
            {
                WorkflowApproveHierarchy model = null;
                if (dto.WfApproveHierarchyID == 0)
                    model = db.WorkflowApproveHierarchy.Create();  // create new from context
                else
                    model = db.WorkflowApproveHierarchy.FirstOrDefault(a => a.WfApproveHierarchyID == dto.WfApproveHierarchyID);  // get from context

                Mapper.Map<WorkflowApproveHierarchyDTO, WorkflowApproveHierarchy>(dto, model);
                model.EditUser = userId;
                model.WfApproveHierarchyCompany = this.sessionData.CompanyID;

                PrepareDetail(model, dto);
                if (Validate(model, dto))
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (SaveEntity(model.WfApproveHierarchyID == 0, model))
                            {
                                int wfHierarchyIdHistory = SaveWFHierarchyHistory(model).FirstOrDefault();
                                foreach (var item in model.WorkflowHierarchyMembers)
                                {
                                    var result = SaveWFHierarchyHistoryMember(wfHierarchyIdHistory, model.WfApproveHierarchyCompany, item).FirstOrDefault();
                                }
                                dto = AutoMapper.Mapper.Map<WorkflowApproveHierarchyDTO>(model);
                            }
                            transaction.Commit();
                            return dto;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new ModelException(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }


        private void PrepareDetail(WorkflowApproveHierarchy model, WorkflowApproveHierarchyDTO dto)
        {
            WorkflowHierarchyMembers modelItem;
            WorkflowHierarchyMembersDTO dtoItem;
            List<WorkflowHierarchyMembers> listToRemove = new List<WorkflowHierarchyMembers>();

            // update and find items to remove
            foreach (WorkflowHierarchyMembers item in model.WorkflowHierarchyMembers)
            {
                dtoItem = dto.WorkflowHierarchyMembers.FirstOrDefault(atv => atv.WfHierarchyMemberID == item.WfHierarchyMemberID);
                if (dtoItem != null)
                {
                    Mapper.Map<WorkflowHierarchyMembersDTO, WorkflowHierarchyMembers>(dtoItem, item);
                    item.EditUser = model.EditUser;
                }
                else
                {
                    listToRemove.Add(item);
                }
            }
            // remove items
            foreach (WorkflowHierarchyMembers item in listToRemove)
            {
                db.WorkflowHierarchyMembers.Remove(item);
                model.WorkflowHierarchyMembers.Remove(item);
            }
            // add new items
            foreach (WorkflowHierarchyMembersDTO item in dto.WorkflowHierarchyMembers.Where(at => at.WfHierarchyMemberID == 0))
            {
                modelItem = AutoMapper.Mapper.Map<WorkflowHierarchyMembers>(item);
                modelItem.EditUser = model.EditUser;
                model.WorkflowHierarchyMembers.Add(modelItem);
            }
        }

        private bool Validate(WorkflowApproveHierarchy model, WorkflowApproveHierarchyDTO dto)
        {
            int count = 0;

            // validate name
            if (String.IsNullOrEmpty(model.WfApproveHierarchyName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // validate repeat name
            count = db.WorkflowApproveHierarchy.Count(
                        t => t.WfApproveHierarchyID != model.WfApproveHierarchyID
                             && t.WfApproveHierarchyName.Trim().ToUpper() == model.WfApproveHierarchyName.Trim().ToUpper()
                             && t.WfApproveHierarchyCompany == this.sessionData.CompanyID
                    );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", model.WfApproveHierarchyName));
            }
            // validate department
            if (model.WfApproveHierarchyDepartment > 0)
            {
                count = db.WorkflowApproveHierarchy.Count(
                            t => t.WfApproveHierarchyDepartment == model.WfApproveHierarchyDepartment
                                 && t.WfApproveHierarchyID != model.WfApproveHierarchyID
                                 && t.WfApproveHierarchyCompany == this.sessionData.CompanyID
                        );
                if (count > 0)
                {
                    throw new ModelException(String.Format("Ya existe una jerarquía por defecto para esta área.", model.WfApproveHierarchyName));
                }
            }

            // validate child
            foreach (WorkflowHierarchyMembers item in model.WorkflowHierarchyMembers)
            {
                count = model.WorkflowHierarchyMembers.Count(
                    wfm => wfm.WfHierarchyMemberDepartment == item.WfHierarchyMemberDepartment
                           && wfm.WfHierarchyMemberPosition == item.WfHierarchyMemberPosition
                );
                if (count > 1)
                {
                    var dep = db.CommonValues.FirstOrDefault(t => t.CommonValueID == item.WfHierarchyMemberDepartment);
                    var pos = db.CommonValues.FirstOrDefault(t => t.CommonValueID == item.WfHierarchyMemberPosition);
                    throw new ModelException(String.Format("El detalle de área '{0}' y cargo '{1}' ya esta siendo usado.", dep.CommonValueDisplay, pos.CommonValueDisplay));
                }
            }

            return true;
        }


        public bool DeleteWorkflowApproveHierarchy(int id)
        {
            if (db.Workflow.FirstOrDefault(x => x.WfApproveHierarchyID == id) == null)
            {
                List<WorkflowHierarchyMembers> listWorflowHierarchyMembers = (from hm in db.WorkflowHierarchyMembers
                                                                              where hm.WfApproveHierarchyID == id
                                                                              select hm).ToList();

                foreach (WorkflowHierarchyMembers item in listWorflowHierarchyMembers)
                {
                    db.WorkflowHierarchyMembers.Remove(item);
                }

                WorkflowApproveHierarchy model = db.WorkflowApproveHierarchy.FirstOrDefault(a => a.WfApproveHierarchyID == id);
                return DeleteEntity(model);
            }
            else
            {
                throw new ModelException("No se puede borrar, existe un workflow asociado a esta jerarquía.");
            }
        }

        public IQueryable<int> SaveWFHierarchyHistory(WorkflowApproveHierarchy dto)
        {
            var query = db.Database.SqlQuery<int>("SITWF.WORKFLOW_APPROVE_HIERARCHY_HISTORY_UPD {0}, {1}, {2}, {3}, {4}", dto.WfApproveHierarchyID, dto.WfApproveHierarchyName, dto.EditUser, dto.WfApproveHierarchyDepartment, dto.WfApproveHierarchyCompany);
            return query.AsQueryable<int>();
        }

        public IQueryable<int> SaveWFHierarchyHistoryMember(int wfHierarchyIdHistory, int idCompany, WorkflowHierarchyMembers dto)
        {
            var query = db.Database.SqlQuery<int>("SITWF.WORKFLOW_HIERARCHY_MEMBERS_HISTORY_UPD {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                wfHierarchyIdHistory, dto.WfHierarchyMemberID, dto.WfApproveHierarchyID, idCompany, dto.WfHierarchyMemberDepartment, dto.WfHierarchyMemberPosition, dto.WfHierarchyMemberOrder, dto.WfHierarchyMemberDescription, dto.EditUser);
            return query.AsQueryable<int>();
        }

        public string GetPeopleMemberWorkflowApproveHierarchy(int department, int position)
        {
            string peopleName = (from p in db.People
                                 where p.PeopleDepartment == department
                                 && p.PeoplePosition == position
                                 //&& p.PeopleCompany == this.sessionData.CompanyID
                                 && p.PeopleStatus == 1
                                 select (p.PeopleFirstName + " " + p.PeopleFirstName2 + " " + p.PeopleLastName + " " + p.PeopleLastName2)).FirstOrDefault();
            return peopleName;
        }
    }

}