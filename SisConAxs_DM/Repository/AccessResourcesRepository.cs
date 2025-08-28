using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Linq.Expressions;

namespace SisConAxs_DM.Repository
{
    // Filters
    public class AccessResourceFilter
    {
        public string ResourceCategoryName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceFullName { get; set; }
        public string ResourceAccessTypeName { get; set; }
        public string ResourceWorkflowName { get; set; }
        public string ResourceDepartmentName { get; set; }
        public int ResourceActive { get; set; } = -1;
        public int excludeWithChilds { get; set; }
        public int showResourceWithoutWorkflow { get; set; } = 0;
    }

    public class AccessResourcesSaveMultiple
    {
        public int[] Resources { get; set; }
        //public Nullable<int> ResourceCategory { get; set; }
        public Nullable<int> ResourceAccessType { get; set; }
        public Nullable<int> ResourceParent { get; set; }
        public Nullable<int> ResourceWorkflow { get; set; }
        //public Nullable<int> ResourceDepartment { get; set; }
        public Nullable<int> ResourceActive { get; set; }

        public bool HaveValues()
        {
            return this.ResourceAccessType.HasValue || this.ResourceParent.HasValue || this.ResourceWorkflow.HasValue || this.ResourceActive.HasValue;
        }
    }

    public class AccessResourcesRepository : AxsBaseRepository
    {
        public AccessResourcesRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.AccessResources;
        }


        /// <summary>Llamada para obtener la lista de AccessResource.</summary>
        /// <para>GET api/AccessResources </para>
        /// <returns>Lista de AccessResource</returns>
        public IQueryable<AccessResourcesDTO> GetAccessResources(AccessResourceFilter filter = null)
        {
            var query = db.Database.SqlQuery<AccessResourcesDTO>("AXSCONTROL.SP_GET_RESOURCES {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                filter.ResourceCategoryName,
                filter.ResourceName,
                filter.ResourceFullName,
                filter.ResourceAccessTypeName,
                filter.ResourceWorkflowName,
                filter.ResourceDepartmentName,
                filter.ResourceActive,
                filter.excludeWithChilds,
                filter.showResourceWithoutWorkflow,
                this.sessionData.CompanyID
            );
            return query.AsQueryable<AccessResourcesDTO>();
        }

        public IQueryable<AccessResourcesDTO> GetAccessResourceQuery(Expression<Func<AccessResourcesDTO, bool>> whereExpr = null)
        {
            var query = from ar in db.AccessResources
                        join rc in db.ResourceCategories on ar.ResourceCategory equals rc.CategoryID
                        join at in db.AccessTypes on ar.ResourceAccessType equals at.AccessTypeID
                        join wf in db.Workflow on ar.ResourceWorkflow equals wf.WfID into workflow
                        from wf in workflow.DefaultIfEmpty()
                        join arp in db.AccessResources on ar.ResourceParent equals arp.ResourceID into parent
                        from arp in parent.DefaultIfEmpty()
                        join dep in db.CommonValues on ar.ResourceDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join arrq in db.AccessResources on ar.ResourceRequired equals arrq.ResourceID into res_required
                        from arrq in parent.DefaultIfEmpty()
                        select new AccessResourcesDTO()
                        {
                            ResourceID = ar.ResourceID,
                            ResourceName = ar.ResourceName,
                            ResourceFullName = ar.ResourceFullName,
                            ResourceDescription = ar.ResourceDescription,
                            ResourceCategory = ar.ResourceCategory,
                            ResourceCategoryName = rc.CategoryName,
                            ResourceAccessType = ar.ResourceAccessType,
                            ResourceAccessTypeName = at.AccessTypeName,
                            ResourceParent = (int)ar.ResourceParent,
                            ResourceParentName = arp.ResourceName,
                            ResourceDepartment = ar.ResourceDepartment,
                            ResourceDepartmentName = dep.CommonValueName,
                            ResourceTemporal = ar.ResourceTemporal,
                            ResourceSendAtEnd = ar.ResourceSendAtEnd,
                            ResourceWorkflow = ar.ResourceWorkflow,
                            ResourceWorkflowName = wf.WfName,
                            ResourceActive = ar.ResourceActive,
                            ResourceOnlyAssignable = ar.ResourceOnlyAssignable,
                            ResourceRequired = (int)ar.ResourceRequired,
                            ResourceRequiredName = arrq.ResourceName,
                            ResourceCompany = ar.ResourceCompany
                            //,ResourceParameters = (from parameter in db.AccessResourceParameters
                            //                      join metadataData in db.Metadata on parameter.ResourceParameterMetadataID equals metadataData.MetadataID into Metadata
                            //                      from metadata in Metadata.DefaultIfEmpty()
                            //                      where
                            //                         parameter.ResourceID == ar.ResourceID
                            //                      select new AccessResourceParameterDTO()
                            //                      {
                            //                          ResourceID = parameter.ResourceID,
                            //                          ResourceParameterID = parameter.ResourceParameterID,
                            //                          ResourceParameterDisplay = parameter.ParameterName.CommonValueDisplay,
                            //                          ResourceParameterMetadataID = parameter.ResourceParameterMetadataID,
                            //                          ValueDisplay = metadata.MetadataDisplay,
                            //                          Value = parameter.Value
                            //                          //ValueInt = parameter.ValueInt,
                            //                          //ValueDate = parameter.ValueDate
                            //                      }).ToList()
                        }; ;
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public IQueryable<AccessResourcesDTO> GetAccessResourceByCategoryId(int id)
        {
            return GetAccessResourceQuery(r => r.ResourceCategory == id);
        }

        public IQueryable<AccessResourcesDTO> GetAccessResourceById(int id)
        {
            var query = from ar in db.AccessResources
                        join rc in db.ResourceCategories on ar.ResourceCategory equals rc.CategoryID
                        join at in db.AccessTypes on ar.ResourceAccessType equals at.AccessTypeID
                        join wf in db.Workflow on ar.ResourceWorkflow equals wf.WfID into workflow
                        from wf in workflow.DefaultIfEmpty()
                        join arp in db.AccessResources on ar.ResourceParent equals arp.ResourceID into parent
                        from arp in parent.DefaultIfEmpty()
                        join dep in db.CommonValues on ar.ResourceDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join arrq in db.AccessResources on ar.ResourceRequired equals arrq.ResourceID into res_required
                        from arrq in parent.DefaultIfEmpty()
                        where
                            ar.ResourceID == id
                        select new AccessResourcesDTO()
                        {
                            ResourceID = ar.ResourceID,
                            ResourceName = ar.ResourceName,
                            ResourceDescription = ar.ResourceDescription,
                            ResourceCategory = ar.ResourceCategory,
                            ResourceCategoryName = rc.CategoryName,
                            ResourceAccessType = ar.ResourceAccessType,
                            ResourceAccessTypeName = at.AccessTypeName,
                            ResourceParent = (int)ar.ResourceParent,
                            ResourceParentName = arp.ResourceName,
                            ResourceDepartment = ar.ResourceDepartment,
                            ResourceDepartmentName = dep.CommonValueName,
                            ResourceTemporal = ar.ResourceTemporal,
                            ResourceSendAtEnd = ar.ResourceSendAtEnd,
                            ResourceWorkflow = ar.ResourceWorkflow,
                            ResourceWorkflowName = wf.WfName,
                            ResourceActive = ar.ResourceActive,
                            ResourceOnlyAssignable = ar.ResourceOnlyAssignable,
                            ResourceRequired = (int)ar.ResourceRequired,
                            ResourceRequiredName = arrq.ResourceName,
                            ResourceCompany = ar.ResourceCompany,
                            ResourceParameters = (from parameter in db.AccessResourceParameters
                                                  join metadataData in db.Metadata on parameter.ResourceParameterMetadataID equals metadataData.MetadataID into Metadata
                                                  from metadata in Metadata.DefaultIfEmpty()
                                                  where
                                                     parameter.ResourceID == ar.ResourceID
                                                  select new AccessResourceParameterDTO()
                                                  {
                                                      ResourceID = parameter.ResourceID,
                                                      ResourceParameterID = parameter.ResourceParameterID,
                                                      ResourceParameterDisplay = parameter.ParameterName.CommonValueDisplay,
                                                      ResourceParameterMetadataID = parameter.ResourceParameterMetadataID,
                                                      ValueDisplay = metadata.MetadataDisplay,
                                                      Value = parameter.Value
                                                      //ValueInt = parameter.ValueInt,
                                                      //ValueDate = parameter.ValueDate
                                                  }).ToList()
                        };
            return query;
        }

        public IQueryable<AccessResourcesDTO> GetAccessResourceWithParametersByCategoryId(int id)
        {
            var query = from ar in db.AccessResources
                        join rc in db.ResourceCategories on ar.ResourceCategory equals rc.CategoryID
                        join at in db.AccessTypes on ar.ResourceAccessType equals at.AccessTypeID
                        join wf in db.Workflow on ar.ResourceWorkflow equals wf.WfID into workflow
                        from wf in workflow.DefaultIfEmpty()
                        join arp in db.AccessResources on ar.ResourceParent equals arp.ResourceID into parent
                        from arp in parent.DefaultIfEmpty()
                        join dep in db.CommonValues on ar.ResourceDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join arrq in db.AccessResources on ar.ResourceRequired equals arrq.ResourceID into res_required
                        from arrq in parent.DefaultIfEmpty()
                        where
                            ar.ResourceCategory == id
                        select new AccessResourcesDTO()
                        {
                            ResourceID = ar.ResourceID,
                            ResourceName = ar.ResourceName,
                            ResourceFullName = ar.ResourceFullName,
                            ResourceDescription = ar.ResourceDescription,
                            ResourceCategory = ar.ResourceCategory,
                            ResourceCategoryName = rc.CategoryName,
                            ResourceAccessType = ar.ResourceAccessType,
                            ResourceAccessTypeName = at.AccessTypeName,
                            ResourceParent = (int)ar.ResourceParent,
                            ResourceParentName = arp.ResourceName,
                            ResourceDepartment = ar.ResourceDepartment,
                            ResourceDepartmentName = dep.CommonValueName,
                            ResourceTemporal = ar.ResourceTemporal,
                            ResourceSendAtEnd = ar.ResourceSendAtEnd,
                            ResourceWorkflow = ar.ResourceWorkflow,
                            ResourceWorkflowName = wf.WfName,
                            ResourceActive = ar.ResourceActive,
                            ResourceOnlyAssignable = ar.ResourceOnlyAssignable,
                            ResourceRequired = (int)ar.ResourceRequired,
                            ResourceRequiredName = arrq.ResourceName,
                            ResourceCompany = ar.ResourceCompany,
                            ResourceParameters = (from parameter in db.AccessResourceParameters
                                                  join metadataData in db.Metadata on parameter.ResourceParameterMetadataID equals metadataData.MetadataID into Metadata
                                                  from metadata in Metadata.DefaultIfEmpty()
                                                  where
                                                     parameter.ResourceID == ar.ResourceID
                                                  select new AccessResourceParameterDTO()
                                                  {
                                                      ResourceID = parameter.ResourceID,
                                                      ResourceParameterID = parameter.ResourceParameterID,
                                                      ResourceParameterDisplay = parameter.ParameterName.CommonValueDisplay,
                                                      ResourceParameterMetadataID = parameter.ResourceParameterMetadataID,
                                                      ValueDisplay = metadata.MetadataDisplay,
                                                      Value = parameter.Value
                                                  }).ToList()
                        };
            return query;
        }

        public AccessResourcesDTO SaveAccessResource(AccessResourcesDTO dto, string userId)
        {
            try
            {
                AccessResources model = null;
                if (dto.ResourceID == 0)
                    model = db.AccessResources.Create();  // create new from context
                else
                    model = db.AccessResources.FirstOrDefault(a => a.ResourceID == dto.ResourceID);  // get from context

                // set data
                Mapper.Map<AccessResourcesDTO, AccessResources>(dto, model);
                model.EditUser = userId;
                if (this.sessionData != null)
                    model.ResourceCompany = this.sessionData.CompanyID;

                foreach (var param in model.ResourceParameters)
                {
                    param.ResourceID = model.ResourceID;
                    param.EditUser = model.EditUser;
                }

                if (Validate(model))
                {
                    if (SaveEntity(dto.ResourceID == 0, model))
                    {
                        dto = AutoMapper.Mapper.Map<AccessResourcesDTO>(model);
                        return dto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }

        public void SaveMultiple(AccessResourcesSaveMultiple dto, string userID)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                AccessResources model = null;
                try
                {
                    if (dto.Resources.Length == 0)
                    {
                        throw new Exception("No hay recursos por actualizar.");
                    }
                    if(!dto.HaveValues())
                    {
                        throw new Exception("No hay valores a actualizar.");
                    }

                    foreach(var ResourceID in dto.Resources)
                    {
                        model = db.AccessResources.FirstOrDefault(r => r.ResourceID == ResourceID);
                        if(model == null)
                        {
                            throw new Exception($"Recurso no econtrado '{ResourceID}'.");
                        }
                        if (dto.ResourceParent.HasValue)
                        {
                            if(dto.ResourceParent.Value == model.ResourceID)
                            {
                                throw new Exception($"No puede seleccionar como recurso padre a sí mismo '{model.ResourceFullName}'.");
                            }
                            model.ResourceParent = dto.ResourceParent.Value;
                        }   
                        if (dto.ResourceAccessType.HasValue)
                            model.ResourceAccessType = dto.ResourceAccessType.Value;
                        if (dto.ResourceWorkflow.HasValue)
                            model.ResourceWorkflow = dto.ResourceWorkflow.Value;
                        if (dto.ResourceActive.HasValue)
                            model.ResourceActive = dto.ResourceActive.Value;
                        model.EditUser = userID;

                        if(Validate(model))
                        {
                            SaveEntity(false, model);
                        }
                    }

                    transaction.Commit();
                }
                catch (ModelException ex)
                {
                    transaction.Rollback();
                    throw new ModelException($"{ex.Message}\n Recurso: '{model.ResourceFullName}'", ex);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ModelException($"Error al grabar los recursos", ex);
                }
            }
        }

        public bool DeleteAccessResource(int id)
        {
            AccessResources model = db.AccessResources.FirstOrDefault(
                a => a.ResourceID == id
                     && a.ResourceCompany == this.sessionData.CompanyID);
            return DeleteEntity(model);
        }


        private bool Validate(AccessResources model)
        {
            // validate name
            if (String.IsNullOrEmpty(model.ResourceName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // validate category
            if (model.ResourceCategory <= 0)
            {
                throw new ModelException("No ha asignado una categoría.");
            }

            // validate if exist resource people for this resource
            //var result = db.ResourcePeople.Where(x => x.ResourceID == model.ResourceID).ToList();
            //if (result.Count > 0)
            //{
            //    throw new ModelException("No se puede inactivar un recurso con " + result.Count + " registros asociados en recurso por persona.");
            //}

            // validate access type
            if (model.ResourceAccessType <= 0)
            {
                throw new ModelException("No ha asignado un tipo de acceso.");
            }

            // validate workflow
            if (model.ResourceActive > 0 && (!model.ResourceWorkflow.HasValue || model.ResourceWorkflow <= 0))
            {
                throw new ModelException("No ha asignado un workflow.");
            }

            if (model.ResourceID > 0 && model.ResourceWorkflow.HasValue && model.ResourceWorkflow > 0)
            {
                var workflow = db.Workflow.FirstOrDefault(w => w.WfID == model.ResourceWorkflow);
                if ((workflow != null) && (workflow.WorkflowItems != null) && (workflow.WorkflowItems.Count == 0))
                {
                    throw new ModelException("El workflow seleccionado no tiene acciones.");
                }
            }

            // validate repeat name
            int count = db.AccessResources.Count(
                    t => t.ResourceID != model.ResourceID
                         && t.ResourceParent == model.ResourceParent
                         && t.ResourceName.Trim().ToUpper() == model.ResourceName.Trim().ToUpper()
                         && t.ResourceCompany == model.ResourceCompany
                );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", model.ResourceName));
            }

            // validate params
            foreach (var param in model.ResourceParameters)
            {                
                var finded = db.AccessResourceParameters.FirstOrDefault(x => x.ResourceParameterID == param.ResourceParameterID
                                                                         && x.Value.Trim() == param.Value.Trim()
                                                                         && !(x.ResourceID == param.ResourceID && x.ResourceParameterID == param.ResourceParameterID)
                                                                    );
                if (finded != null)
                {
                    var parameterInfo = db.CommonValues.FirstOrDefault(x => x.CommonValueID == finded.ResourceParameterID);
                    var resource = db.AccessResources.FirstOrDefault(x => x.ResourceID == finded.ResourceID);
                    var company = db.Companies.FirstOrDefault(x => x.CompanyID == resource.ResourceCompany);
                    throw new ModelException($"El parámetro '{parameterInfo.CommonValueDisplay}' ya esta siendo usado<br/> con el mismo valor '{param.Value}'<br/> en '{company.CompanyDisplay}' - '{resource.ResourceFullName}'.");
                }                
            }
            return true;
        }
    }
}
