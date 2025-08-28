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

namespace SisConAxs_DM.Repository
{
    public class MetadataRepository : AxsBaseRepository
    {
        public MetadataRepository() //SessionData sessionData)
        {
            //this.sessionData = sessionData;
            dbSet = db.Metadata;
        }

        public MetadataDTO GetMetadataByID(int id)
        {
            return GetQuery(m => m.MetadataID == id).FirstOrDefault();
        }

        public IQueryable<MetadataDTO> GetMetadataByParentID(int id)
        {
            return GetQuery(m => m.MetadataParentID == id);
        }

        public IQueryable<MetadataDTO> GetMetadata()
        {
            return GetQuery(null);
        }

        public IQueryable<MetadataDTO> GetQuery(Expression<Func<MetadataDTO, bool>> whereExpr)
        {
            var query = from m in db.Metadata
                        orderby
                            m.MetadataParentID,
                            m.MetadataDisplay
                        select new MetadataDTO()
                        {
                            MetadataID = m.MetadataID,
                            MetadataParentID = m.MetadataParentID,
                            MetadataDisplay = m.MetadataDisplay,
                            MetadataDescription = m.MetadataDescription,
                            MetadataInt1 = m.MetadataInt1,
                            MetadataInt2 = m.MetadataInt2,
                            MetadataInt3 = m.MetadataInt3,
                            MetadataInt4 = m.MetadataInt4,
                            MetadataInt5 = m.MetadataInt5,
                            MetadataStr1 = m.MetadataStr1,
                            MetadataStr2 = m.MetadataStr2,
                            MetadataStr3 = m.MetadataStr3,
                            MetadataStr4 = m.MetadataStr4,
                            MetadataStr5 = m.MetadataStr5,
                            MetadataDatetime1 = m.MetadataDatetime1,
                            MetadataDatetime2 = m.MetadataDatetime2,
                            MetadataDatetime3 = m.MetadataDatetime3,
                            MetadataActive = m.MetadataActive
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public MetadataDTO SaveMetadata(MetadataDTO dto, string userID)
        {
            try
            {
                Metadata model = null;
                if (dto.MetadataID == 0)
                    model = db.Metadata.Create();  // create new from context
                else
                    model = db.Metadata.FirstOrDefault(a => a.MetadataID == dto.MetadataID);  // get from context

                Mapper.Map<MetadataDTO, Metadata>(dto, model);
                model.EditUser = userID;

                //if (Validate(model, dto))
                //{
                if (SaveEntity(dto.MetadataID == 0, model))
                {
                    dto = Mapper.Map<MetadataDTO>(model);
                    return dto;
                }
                //}
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message, ex);
            }
            return null;
        }

        public MetadataDTO SaveMetadata(SisConAxsContext db, MetadataDTO dto, string userID)
        {
            try
            {
                Metadata model = null;
                if (dto.MetadataID == 0)
                    model = db.Metadata.Create();  // create new from context
                else
                    model = db.Metadata.FirstOrDefault(a => a.MetadataID == dto.MetadataID);  // get from context

                Mapper.Map<MetadataDTO, Metadata>(dto, model);
                model.EditUser = userID;

                if (dto.MetadataID == 0)
                {
                    db.Metadata.Add(model);
                }
                else
                {
                    db.Metadata.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                }

                if (db.SaveChanges() > 0)
                {
                    dto = Mapper.Map<MetadataDTO>(model);
                    return dto;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return null;
        }
    }
}
