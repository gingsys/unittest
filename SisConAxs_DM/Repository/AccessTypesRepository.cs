using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using SisConAxs_DM.DTO.Filters;

namespace SisConAxs_DM.Repository
{

    public class AccessTypesRepository : AxsBaseRepository
    {

        public AccessTypesRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.AccessTypes;
        }

        /// <summary>Llamada para obtener la lista de AccesTypes.</summary>
        /// <para>GET api/AccessTypes </para>
        /// <returns>Lista de AccesType</returns>
        public IQueryable<AccessTypesDTO> GetAccessTypes(AccessTypeFilter filter)
        {
            bool withDetails = filter != null && filter.WithDetails;

            var query = from at in db.AccessTypes
                        where at.AccessTypeCompany == this.sessionData.CompanyID
                        orderby
                          at.AccessTypeName
                        select new AccessTypesDTO()
                        {
                            AccessTypeID = at.AccessTypeID,
                            AccessTypeName = at.AccessTypeName,
                            AccessTypeType = at.AccessTypeType,
                            AccessTypeValues = (from atv in db.AccessTypeValues
                                                where
                                                    atv.AccessTypeID == (withDetails ? at.AccessTypeID : 0)
                                                select new AccessTypeValuesDTO()
                                                {
                                                    AccessTypeID = atv.AccessTypeID,
                                                    TypeValueID = atv.TypeValueID,
                                                    TypeValueName = atv.TypeValueName,
                                                    TypeValueDisplay = atv.TypeValueDisplay,
                                                    TypeValueCharVal = atv.TypeValueCharVal,
                                                    TypeValueAdditional = atv.TypeValueAdditional
                                                }).ToList<AccessTypeValuesDTO>()
                        };
            return query;
        }


        public IQueryable<AccessTypesDTO> GetAccessTypeById(int id)
        {
            var query = from at in db.AccessTypes
                        where
                            at.AccessTypeID == id
                            && at.AccessTypeCompany == this.sessionData.CompanyID
                        select new AccessTypesDTO()
                        {
                            AccessTypeID = at.AccessTypeID,
                            AccessTypeName = at.AccessTypeName,
                            AccessTypeType = at.AccessTypeType,
                            AccessTypeValues = (from atv in db.AccessTypeValues
                                                where atv.AccessTypeID == at.AccessTypeID
                                                select new AccessTypeValuesDTO()
                                                {
                                                    AccessTypeID = atv.AccessTypeID,
                                                    TypeValueID = atv.TypeValueID,
                                                    TypeValueName = atv.TypeValueName,
                                                    TypeValueDisplay = atv.TypeValueDisplay,
                                                    TypeValueCharVal = atv.TypeValueCharVal,
                                                    TypeValueAdditional = atv.TypeValueAdditional
                                                }).ToList<AccessTypeValuesDTO>()
                        };
            return query;
        }


        public AccessTypesDTO SaveAccessType(AccessTypesDTO dto, string userId)
        {
            try
            {
                AccessTypes model = null;
                if (dto.AccessTypeID == 0)
                    model = db.AccessTypes.Create();  // create new from context
                else
                    model = db.AccessTypes.FirstOrDefault(a => a.AccessTypeID == dto.AccessTypeID);  // get from context

                Mapper.Map<AccessTypesDTO, AccessTypes>(dto, model);
                model.EditUser = userId;
                model.AccessTypeCompany = this.sessionData.CompanyID;

                PrepareDetail(model, dto);
                if (Validate(model, dto))
                {
                    if (SaveEntity(model.AccessTypeID == 0, model))
                    {
                        dto = AutoMapper.Mapper.Map<AccessTypesDTO>(model);
                        return dto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }



        private void PrepareDetail(AccessTypes model, AccessTypesDTO dto)
        {
            AccessTypeValues modelItem;
            AccessTypeValuesDTO dtoItem;
            List<AccessTypeValues> listToRemove = new List<AccessTypeValues>();

            // update and find items to remove
            foreach (AccessTypeValues item in model.AccessTypeValues)
            {
                dtoItem = dto.AccessTypeValues.FirstOrDefault(atv => atv.TypeValueID == item.TypeValueID);
                if (dtoItem != null)
                {
                    Mapper.Map<AccessTypeValuesDTO, AccessTypeValues>(dtoItem, item);
                    item.EditUser = model.EditUser;
                }
                else
                {
                    listToRemove.Add(item);
                }
            }
            // remove items
            foreach (AccessTypeValues item in listToRemove)
            {
                db.AccessTypeValues.Remove(item);
                model.AccessTypeValues.Remove(item);
            }
            // add new items
            foreach (AccessTypeValuesDTO item in dto.AccessTypeValues.Where(at => at.TypeValueID == 0))
            {
                modelItem = AutoMapper.Mapper.Map<AccessTypeValues>(item);
                modelItem.EditUser = model.EditUser;
                model.AccessTypeValues.Add(modelItem);
            }
        }

        private bool Validate(AccessTypes model, AccessTypesDTO dto)
        {
            // validate name
            if (String.IsNullOrEmpty(model.AccessTypeName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // validate repeat name
            int count = db.AccessTypes.Count(
                    t => t.AccessTypeID != model.AccessTypeID
                         && t.AccessTypeName.Trim().ToUpper() == model.AccessTypeName.Trim().ToUpper()
                         && t.AccessTypeCompany == this.sessionData.CompanyID

                );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", model.AccessTypeName));
            }

            // validate child
            //count = dto.AccessTypeValues.Count;
            //if (count == 0 && (model.AccessTypeType == 3 || model.AccessTypeType == 4))
            //{
            //    throw new ModelException("El tipo de acceso no se puede grabar sin detalle.");
            //}
            foreach (AccessTypeValues item in model.AccessTypeValues)
            {
                count = model.AccessTypeValues.Count(
                    atv => //atv != item &&
                           atv.TypeValueID != item.TypeValueID
                           && atv.TypeValueName.Trim().ToUpper() == item.TypeValueName.Trim().ToUpper()
                    );
                if (count > 0)
                {
                    throw new ModelException(String.Format("El nombre de detalle '{0}' ya esta siendo usado.", item.TypeValueName));
                }
            }

            return true;
        }


        public bool DeleteAccessType(int id)
        {
            AccessTypes model = db.AccessTypes.FirstOrDefault(
                a => a.AccessTypeID == id
                     && a.AccessTypeCompany == this.sessionData.CompanyID);
            return DeleteEntity(model);
        }
    }

}
