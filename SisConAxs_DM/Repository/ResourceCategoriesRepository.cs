using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using SisConAxs_DM.DTO.Filters;
using SisConAxs_DM.DTO.Response;

namespace SisConAxs_DM.Repository
{

    public class ResourceCategoriesRepository : AxsBaseRepository
    {
        public ResourceCategoriesRepository(SessionData sessionData)
        {
            this.sessionData = sessionData;
            dbSet = db.ResourceCategories;
        }

        /// <summary>Llamada para obtener la lista de ResourceCategory.</summary>
        /// <para>GET api/ResourceCategories </para>
        /// <returns>Lista de ResourceCategory</returns>
        public IQueryable<ResourceCategoriesDTO> GetResourceCategories()
        {
            return GetResourceCategoriesQuery(null);
        }

        public IQueryable<ResourceCategoriesDTO> GetResourceCategoryById(int id)
        {
            return GetResourceCategoriesQuery(c => c.CategoryID == id);
        }

        private IQueryable<ResourceCategoriesDTO> GetResourceCategoriesQuery(Expression<Func<ResourceCategoriesDTO, bool>> whereExpr = null)
        {
            var query = from at in db.ResourceCategories
                        orderby
                            at.CategoryName
                        select new ResourceCategoriesDTO()
                        {
                            CategoryID = at.CategoryID,
                            CategoryName = at.CategoryName,
                            CategoryDescription = at.CategoryDescription
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public PaginationResponse<ResourceCategoriesDTO> GetResourceCategoriesPaginate(ResourceCategoryFilter filter)
        {
            var query = GetResourceCategoriesQuery();

            if (filter.CategoryID != null)
            {
                query = query.Where(q => q.CategoryID == filter.CategoryID);
            }
            if (!String.IsNullOrWhiteSpace(filter.CategoryName))
            {
                query = query.Where(q => q.CategoryName.Contains(filter.CategoryName));
            }
            if (!String.IsNullOrWhiteSpace(filter.CategoryDescription))
            {
                query = query.Where(q => q.CategoryDescription.Contains(filter.CategoryDescription));
            }

            // response
            var response = new PaginationResponse<ResourceCategoriesDTO>();
            int count = query.Count();
            if (count > 0)
            {
                response.TotalRows = count;
                if (filter.CurrentPage > 0 && filter.PageSize > 0)
                {
                    query = query.Skip((filter.CurrentPage - 1) * filter.PageSize).Take(filter.PageSize);
                }
                response.Rows = query.ToList();
            }
            return response;
        }


        public ResourceCategoriesDTO SaveCategory(ResourceCategoriesDTO dto, string userId)
        {
            try
            {
                ResourceCategories model = null;
                if (dto.CategoryID == 0)
                    model = db.ResourceCategories.Create();  // create new from context
                else
                    model = db.ResourceCategories.FirstOrDefault(a => a.CategoryID == dto.CategoryID);  // get from context

                Mapper.Map<ResourceCategoriesDTO, ResourceCategories>(dto, model);
                model.EditUser = userId;

                if (Validate(model, dto))
                {
                    if (SaveEntity(dto.CategoryID == 0, model))
                    {
                        dto = AutoMapper.Mapper.Map<ResourceCategoriesDTO>(model);
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


        public bool DeleteResourceCategory(int id)
        {
            ResourceCategories model = db.ResourceCategories.FirstOrDefault(a => a.CategoryID == id);
            return DeleteEntity(model);
        }


        private bool Validate(ResourceCategories model, ResourceCategoriesDTO dto)
        {
            // validate name
            if (String.IsNullOrEmpty(model.CategoryName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }

            // Validate repeat name
            int count = db.ResourceCategories.Count(
                    t => t.CategoryID != model.CategoryID 
                         && t.CategoryName.Trim().ToUpper() == model.CategoryName.Trim().ToUpper()
                );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", dto.CategoryName));
            }
            return true;
        }
    }
}
