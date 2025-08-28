using AutoMapper;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO.Response;
using SisConAxs_DM.DTO.Filters;

namespace SisConAxs_DM.Repository
{
    public class CompanyRepository : AxsBaseRepository
    {
        public CompanyRepository()
        {
            dbSet = db.Companies;
        }

        /// <summary>Llamada para obtener la lista de ResourceCategory.</summary>
        /// <para>GET api/ResourceCategories </para>
        /// <returns>Lista de ResourceCategory</returns>
        public IQueryable<CompanyDTO> GetCompanies()
        {
            return GetCompanyQuery(null);
        }


        public CompanyDTO GetCompanyById(int id)
        {
            return GetCompanyQuery(c => c.CompanyID == id).FirstOrDefault();
        }

        private IQueryable<CompanyDTO> GetCompanyQuery(Expression<Func<CompanyDTO, bool>> whereExpr)
        {
            var query = from c in db.Companies
                        join country in db.CommonValues on c.CompanyCountry equals country.CommonValueID
                        orderby
                            c.CompanyName
                        select new CompanyDTO()
                        {
                            CompanyID = c.CompanyID,
                            CompanyTaxpayerID = c.CompanyTaxpayerID,
                            CompanyName = c.CompanyName,
                            CompanyDisplay = c.CompanyDisplay,
                            CompanyCountry = c.CompanyCountry,
                            CompanyCountryName = country.CommonValueDisplay,
                            //CompanyAD = c.CompanyAD,
                            CompanyAddress = c.CompanyAddress,
                            CompanyActive = c.CompanyActive,
                            //CompanyParameters = (from parameter in db.CompanyParameters
                            //                     where
                            //                        parameter.CompanyID == c.CompanyID
                            //                     select new CompanyParameterDTO()
                            //                     {
                            //                         CompanyID = parameter.CompanyID,
                            //                         CompanyParameterID = parameter.CompanyParameterID,
                            //                         CompanyParameterDisplay = parameter.ParameterName.CommonValueDisplay,
                            //                         Value = parameter.Value,
                            //                         //ValueInt = parameter.ValueInt,
                            //                         //ValueDate = parameter.ValueDate
                            //                     }).ToList()
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public PaginationResponse<CompanyDTO> GetCompaniesPaginate(CompanyFilter filter)
        {
            var query = GetCompanies();

            if(!String.IsNullOrWhiteSpace(filter.CompanyTaxpayerID))
            {
                query = query.Where(q => q.CompanyTaxpayerID.Contains(filter.CompanyTaxpayerID));
            }
            if (!String.IsNullOrWhiteSpace(filter.CompanyName))
            {
                query = query.Where(q => q.CompanyName.Contains(filter.CompanyName));
            }
            if (!String.IsNullOrWhiteSpace(filter.CompanyDisplay))
            {
                query = query.Where(q => q.CompanyDisplay.Contains(filter.CompanyDisplay));
            }                

            // response
            var response = new PaginationResponse<CompanyDTO>();
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


        public CompanyDTO SaveCompany(CompanyDTO dto, string userId)
        {
            try
            {
                Company model = null;
                if (dto.CompanyID == 0)
                    model = db.Companies.Create();  // create new from context
                else
                    model = db.Companies.FirstOrDefault(a => a.CompanyID == dto.CompanyID);  // get from context

                Mapper.Map<CompanyDTO, Company>(dto, model);
                model.EditUser = userId;
                //foreach(var param in model.CompanyParameters)
                //{
                //    param.CompanyID = model.CompanyID;
                //    param.EditUser = model.EditUser;
                //}

                if (Validate(model, dto))
                {
                    if (SaveEntity(dto.CompanyID == 0, model))
                    {
                        dto = AutoMapper.Mapper.Map<CompanyDTO>(model);
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


        public bool DeleteCompany(int id)
        {
            Company model = db.Companies.FirstOrDefault(a => a.CompanyID == id);
            return DeleteEntity(model);
        }


        private bool Validate(Company model, CompanyDTO dto)
        {
            if(model.CompanyID == -1)
            {
                throw new ModelException("No puede editar esta empresa.");
            }
            if (String.IsNullOrEmpty(model.CompanyTaxpayerID))
            {
                throw new ModelException("No ha asignado un número de contribuyente.");
            }
            if (String.IsNullOrEmpty(model.CompanyName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }
            if (String.IsNullOrEmpty(model.CompanyDisplay))
            {
                throw new ModelException("No ha asignado una etiqueta.");
            }

            int count = 0;
            // Validate repeat name
            count = db.Companies.Count(
                    t => t.CompanyID != model.CompanyID
                    && t.CompanyName.Trim().ToUpper() == model.CompanyName.Trim().ToUpper()
            );
            if (count > 0)
            {
                throw new ModelException(String.Format("El nombre '{0}' ya esta siendo usado.", dto.CompanyName));
            }
            // Validate repeat display
            count = db.Companies.Count(
                    t => t.CompanyID != model.CompanyID
                    && t.CompanyDisplay.Trim().ToUpper() == model.CompanyDisplay.Trim().ToUpper()
            );
            if (count > 0)
            {
                throw new ModelException(String.Format("La etiqueta '{0}' ya esta siendo usada.", dto.CompanyDisplay));
            }
            //Validate TaxpayerID
            count = db.Companies.Count(
                t => t.CompanyID != model.CompanyID
                    && t.CompanyTaxpayerID.Trim().ToUpper() == model.CompanyTaxpayerID.Trim().ToUpper()
                    && t.CompanyCountry == model.CompanyCountry
            );
            if (count > 0)
            {
                throw new ModelException(String.Format("El ID de contribuyente '{0}' ya esta siendo usado para este país.", dto.CompanyTaxpayerID));
            }

            // validate params
            //foreach (var param in model.CompanyParameters)
            //{
            //    var finded = db.CompanyParameters.FirstOrDefault(x => x.CompanyParameterID == param.CompanyParameterID
            //                                                            && x.Value.Trim() == param.Value.Trim()
            //                                                            && !(x.CompanyID == param.CompanyID && x.CompanyParameterID == param.CompanyParameterID)
            //                                                    );
            //    if (finded != null)
            //    {
            //        var parameterInfo = db.CommonValues.FirstOrDefault(x => x.CommonValueID == finded.CompanyParameterID);
            //        var company = db.Companies.FirstOrDefault(x => x.CompanyID == finded.CompanyID);
            //        throw new ModelException($"El parámetro '{parameterInfo.CommonValueDisplay}' ya esta siendo usado<br/> con el mismo valor '{param.Value}'<br/> en '{company.CompanyName}'.");
            //    }
            //}
            return true;
        }

        public List<string> GetCompaniesAD(int companyID)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@COMPANY_ID", SqlDbType.VarChar) { Value = companyID }
            };
            var result = this.Select<string>("AXSCONTROL.SP_GET_COMPANY_AD", parameters);
            return result;
        }
        public List<string> GetCompaniesSAP(int companyID)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@COMPANY_ID", SqlDbType.VarChar) { Value = companyID }
            };
            var result = this.Select<string>("AXSCONTROL.SP_GET_COMPANY_SAP", parameters);
            return result;
        }

        public List<CompanyDTO> GetCompanyByAD(string empresa)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@companyAD", SqlDbType.VarChar) { Value = empresa }
            };
            var company = this.Select<CompanyDTO>("AXSCONTROL.SP_GET_COMPANY_BY_COMPANY_AD", parameters);
            return company;
        }
        public List<CompanyDTO> GetCompanyBySAP(string empresa)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@companySAP", SqlDbType.VarChar) { Value = empresa }
            };
            var company = this.Select<CompanyDTO>("AXSCONTROL.SP_GET_COMPANY_BY_COMPANY_SAP", parameters);
            return company;
        }
    }
}
