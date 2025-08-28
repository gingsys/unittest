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
using System.ComponentModel.Design;
using System.Collections;

namespace SisConAxs_DM.Repository
{
    public class FilterCommonValueSets
    {
        public bool OnlyRestrictedByCompany { get; set; }
        public bool? SystemCompanySets { get; set; }
    }

    public class CommonValueSetsRepository : AxsBaseRepository
    {
        public CommonValueSetsRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.CommonValueSets;
        }

        /// <summary>Llamada para obtener la lista de CommonValueSet.</summary>
        /// <para>GET api/CommonValueSets </para>
        /// <returns>Lista de CommonValueSet</returns>
        public IQueryable<CommonValueSetsDTO> GetCommonValueSets(FilterCommonValueSets filters = null)
        {
            var query = from at in db.CommonValueSets
                        select at;
            if (filters != null)
            {
                if (filters.OnlyRestrictedByCompany)
                {
                    query = query.Where(cs => cs.CommonValueSetRestrictedByCompany);
                }
                if (filters.SystemCompanySets != null)
                {
                    query = query.Where(cs => filters.SystemCompanySets.Value ? cs.CommonValueSetSystemValue > 0 : cs.CommonValueSetSystemValue == 0);
                }
            }

            return query
                .OrderBy(cs => cs.CommonValueSetName)
                .Select(at => new CommonValueSetsDTO()
                {
                    CommonValueSetID = at.CommonValueSetID,
                    CommonValueSetName = at.CommonValueSetName,
                    CommonValueSetDesc = at.CommonValueSetDesc
                });
        }


        public IQueryable<CommonValueSetsDTO> GetCommonValueSetById(string id)
        {
            int valueSetID = -1;
            int.TryParse(id, out valueSetID);

            var query = from cvs in db.CommonValueSets
                            //where cvs.CommonValueSetID == id
                        where cvs.CommonValueSetName == id ||
                              cvs.CommonValueSetID == valueSetID
                        select new CommonValueSetsDTO()
                        {
                            CommonValueSetID = cvs.CommonValueSetID,
                            CommonValueSetName = cvs.CommonValueSetName,
                            CommonValueSetDesc = cvs.CommonValueSetDesc,
                            CommonValues = (from cv in db.CommonValues
                                            where
                                                 cv.CommonValueSetID == cvs.CommonValueSetID
                                                 && (
                                                    (cvs.CommonValueSetSystemValue > 0) ||
                                                    (!cvs.CommonValueSetRestrictedByCompany) ||
                                                    (cvs.CommonValueSetRestrictedByCompany && cv.CommonValueCompany == this.sessionData.CompanyID)
                                                 )
                                            select new CommonValuesDTO()
                                            {
                                                CommonValueSetID = cv.CommonValueSetID,
                                                CommonValueID = cv.CommonValueID,
                                                CommonValueName = cv.CommonValueName,
                                                CommonValueDesc = cv.CommonValueDesc,
                                                CommonValueDisplay = cv.CommonValueDisplay,
                                                CommonValueCompany = cv.CommonValueCompany
                                            }).ToList()
                        };
            return query;
        }

        public IQueryable<CommonValuesDTO> GetCommonValuesBySet(string id, AccessUsers user = null, CommonValuesDTO filter = null)
        {
            int valueSetID = -1;
            int.TryParse(id, out valueSetID);

            var companyID = this.sessionData.CompanyID;
            if (filter != null)
            {
                if (filter.CommonValueCompany.HasValue && filter.CommonValueCompany > 0)
                {
                    companyID = (int)filter.CommonValueCompany;
                }
            }

            var query = from cv in db.CommonValues
                        join cvs in db.CommonValueSets on cv.CommonValueSetID equals cvs.CommonValueSetID
                        where (cvs.CommonValueSetName == id || cv.CommonValueSetID == valueSetID)
                               && (
                                    (cvs.CommonValueSetSystemValue > 0) ||
                                    (!cvs.CommonValueSetRestrictedByCompany) ||
                                    (cvs.CommonValueSetRestrictedByCompany && cv.CommonValueCompany == companyID)
                                )
                        select new CommonValuesDTO()
                        {
                            CommonValueSetID = cv.CommonValueSetID,
                            CommonValueID = cv.CommonValueID,
                            CommonValueName = cv.CommonValueName,
                            CommonValueDesc = cv.CommonValueDesc,
                            CommonValueDisplay = cv.CommonValueDisplay
                        };

            //if (user != null)
            //{
            //    if (id == "TIPO_SOLICITUD")
            //    {
            //        if (user.UserRole5 == 0)  // rol "dar de baja"
            //        {
            //            query = query.Where(x => x.CommonValueName != "ST_BAJA");
            //        }
            //    }
            //}
            return query;
        }

        public IQueryable<CommonValuesDTO> GetCommonValuesQuery(Expression<Func<CommonValuesDTO, bool>> whereExpr)
        {
            var query = from cv in db.CommonValues
                        join cvs in db.CommonValueSets on cv.CommonValueSetID equals cvs.CommonValueSetID
                        select new CommonValuesDTO()
                        {
                            CommonValueSetID = cv.CommonValueSetID,
                            CommonValueID = cv.CommonValueID,
                            CommonValueName = cv.CommonValueName,
                            CommonValueDesc = cv.CommonValueDesc,
                            CommonValueDisplay = cv.CommonValueDisplay
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }


        /// <summary>
        /// Devuelve un objeto CommonValueDto con el CommonValue por defecto para el CommonValueSet especificado 
        /// en el parametro. Si no existe un valor por defecto se devuelve null.
        /// </summary>
        /// <param name="valueset">Nombre del CommonValueSet para el que se busca el valor por defecto.</param>
        /// <returns>El CommonValue por defecto, o null si no se encuentra</returns>
        public CommonValuesDTO GetDefaultValue(string valueset)
        {
            CommonValuesDTO dto = null;
            CommonValueSets commonValueSet = db.CommonValueSets.FirstOrDefault(cvs => cvs.CommonValueSetName == valueset);
            if (commonValueSet != null)
            {
                CommonValues commonValue = commonValueSet.CommonValues.FirstOrDefault(cv => cv.CommonValueDefault == 1);
                if (commonValue != null)
                {
                    dto = new CommonValuesDTO();
                    Mapper.Map<CommonValues, CommonValuesDTO>(commonValue, dto);
                }
            }
            return dto;
        }


        public CommonValueSetsDTO SaveCommonValueSet(CommonValueSetsDTO dto, string userId)
        {
            try
            {
                CommonValueSets model = null;
                if (dto.CommonValueSetID == 0)
                    model = db.CommonValueSets.Create();  // create new from context
                else
                    model = db.CommonValueSets.FirstOrDefault(a => a.CommonValueSetID == dto.CommonValueSetID);  // get from context

                //Validando que no se modifiquen los datos del ValueSet(Solo se pueden modificar sus hijos)
                if (model.CommonValueSetDesc != dto.CommonValueSetDesc ||
                    model.CommonValueSetName != dto.CommonValueSetName)
                    throw new ModelException(String.Format("No se puede cambiar la descripcion de este juego de valores porque es un valor no editable.", model.CommonValueSetName));

                Mapper.Map<CommonValueSetsDTO, CommonValueSets>(dto, model);
                model.EditUser = userId;

                PrepareDetail(model, dto);
                if (Validate(model, dto))
                {
                    if (SaveEntity(model.CommonValueSetID == 0, model))
                    {
                        dto = Mapper.Map<CommonValueSetsDTO>(model);
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



        private void PrepareDetail(CommonValueSets model, CommonValueSetsDTO dto)
        {
            CommonValues modelItem;
            CommonValuesDTO dtoItem;
            List<CommonValues> listToRemove = new List<CommonValues>();

            // update and find items to remove
            IEnumerable<CommonValues> details;
            if (model.CommonValueSetRestrictedByCompany)
            {
                details = model.CommonValues.Where(cv => cv.CommonValueCompany == this.sessionData.CompanyID);
            }
            else
            {
                details = model.CommonValues;
            }
            foreach (CommonValues item in details)
            {
                dtoItem = dto.CommonValues.FirstOrDefault(cv => cv.CommonValueID == item.CommonValueID);
                if (dtoItem != null)
                {
                    Mapper.Map<CommonValuesDTO, CommonValues>(dtoItem, item);
                    if (model.CommonValueSetRestrictedByCompany)
                        item.CommonValueCompany = this.sessionData.CompanyID;
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
                if (model.CommonValueSetRestrictedByCompany)
                    modelItem.CommonValueCompany = this.sessionData.CompanyID;
                modelItem.EditUser = model.EditUser;
                model.CommonValues.Add(modelItem);
            }
        }

        private bool Validate(CommonValueSets model, CommonValueSetsDTO dto)
        {
            // validate system values
            if (model.CommonValueSetSystemValue == 1)
            {
                throw new ModelException(String.Format("No se puede modificar '{0}' porque es un valor del sistema.", model.CommonValueSetName));
            }

            else
            // validate name
            if (String.IsNullOrEmpty(model.CommonValueSetName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // validate description
            if (String.IsNullOrEmpty(model.CommonValueSetDesc))
            {
                throw new ModelException("No ha asignado una descripción.");
            }

            // validate repeat name
            int count = db.CommonValueSets.Count(
                t => t.CommonValueSetID != model.CommonValueSetID
                     && t.CommonValueSetName.Trim().ToUpper() == model.CommonValueSetName.Trim().ToUpper()
                );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", model.CommonValueSetName));
            }

            // validate detail
            //count = dto.CommonValues.Count;
            //if (count == 0)
            //{
            //    throw new ModelException("No se puede grabar sin detalle.");
            //}
            var detail = model.CommonValues.Where(cv => cv.CommonValueCompany == this.sessionData.CompanyID);
            foreach (CommonValues item in detail)
            {
                count = detail.Count(
                    cv => ((item.CommonValueID == 0 && cv != item) || cv.CommonValueID != item.CommonValueID)
                          && cv.CommonValueName.Trim().ToUpper() == item.CommonValueName.Trim().ToUpper()
                    );
                if (count > 0)
                {
                    throw new ModelException($"El nombre de detalle '{item.CommonValueName}' ya esta siendo usado.");
                }
            }

            return true;
        }


        public bool DeleteCommonValueSet(int id)
        {
            try
            {
                CommonValueSets model = db.CommonValueSets.FirstOrDefault(a => a.CommonValueSetID == id);
                if (ValidateDelete(model))
                {
                    return DeleteEntity(model);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
        }

        private bool ValidateDelete(CommonValueSets model)
        {
            if (model.CommonValueSetSystemValue > 1)
            {
                throw new ModelException(String.Format("No se puede eliminar '{0}' porque es un valor no editable.", model.CommonValueSetName));
            }
            else if (model.CommonValueSetSystemValue > 0)
            {
                throw new ModelException(String.Format("No se puede eliminar '{0}' porque es un valor del sistema.", model.CommonValueSetName));
            }
            return true;
        }
    }
}
