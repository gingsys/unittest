using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using AutoMapper;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Configuration;
using SisConAxs_DM.Repository.Util;
using SisConAxs_DM.Utils;
using System.Data.Entity;
using SisConAxs_DM.DTO.Filters;
using SisConAxs_DM.DTO.Response;

namespace SisConAxs_DM.Repository
{
    public class PeopleRepository : AxsBaseRepository
    {
        public PeopleRepository(SessionData sessionData = null)
        {
            this.sessionData = sessionData;
            dbSet = db.People;
        }

        public IQueryable<PeopleDTO> GetPeople(PeopleFilter filter = null)
        {
            if (filter != null)
            {
                return GetPeopleQuery(p =>
                        filter.allCompanies ? true : p.PeopleCompany == this.sessionData.CompanyID
                        && filter.status == -1 ? true : p.PeopleStatus == filter.status
                );
            }
            return GetPeopleQuery(null);
        }

        public IQueryable<PeopleDTO> GetPeoplewithActives(PeopleFilter filter)
        {
            var query1 = (from pp in db.People //personas con activos asignados
                          join rp in db.ResourcePeople on pp.PeopleID equals rp.PeopleID
                          where
                            rp.PresCompany == this.sessionData.CompanyID
                            && rp.PresActive == 1
                          select pp.PeopleID).ToList();

            var query2 = (from pp in db.People //personas con solicitudes pendientes
                          join req in db.AccessRequests on pp.PeopleID equals req.RequestTo
                          join reqd in db.AccessRequestDetails on req.RequestID equals reqd.RequestID
                          //join res in db.AccessResources on reqd.ResourceID equals res.ResourceID
                          join wfe in db.WorkflowExecution on new { resID = reqd.RequestDetID, objName = "ACCESS_REQUEST" } equals new { resID = wfe.WfExecObjectID, objName = wfe.WfExecObjectName }
                          where
                            wfe.WfExecCompany == this.sessionData.CompanyID
                            && wfe.WfExecStatus < 10
                          select pp.PeopleID).ToList();

            query1.AddRange(query2);
            query1.Distinct();

            var query = from p in db.People
                        join dt in db.CommonValues on p.PeopleDocType equals dt.CommonValueID into doctype
                        from dt in doctype.DefaultIfEmpty()
                        join dep in db.CommonValues on p.PeopleDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join pos in db.CommonValues on p.PeoplePosition equals pos.CommonValueID into position
                        from pos in position.DefaultIfEmpty()

                        join tc in db.CommonValues on p.PeopleTypeClasificacion equals tc.CommonValueID into TypeClasificacion
                        from tc in TypeClasificacion.DefaultIfEmpty()
                        join et in db.CommonValues on p.PeopleEmployeeType equals et.CommonValueID into EmployeeType
                        from et in EmployeeType.DefaultIfEmpty()
                        where
                            p.PeopleCompany == this.sessionData.CompanyID &&
                            //new
                            (filter.PeopleInternalID == null || (p.PeopleInternalID!=null && p.PeopleInternalID.ToLower().Contains(filter.PeopleInternalID.ToLower().Trim()))) &&
                            (filter.PeopleFullLastName == null || (p.PeopleLastName.ToLower() ?? "" + " " + p.PeopleLastName2.ToLower() ?? "").Trim().Contains(filter.PeopleFullLastName.ToLower().Trim())) &&
                            (filter.PeopleFullFirstName == null || (p.PeopleFirstName.ToLower() ?? "" + " " + p.PeopleFirstName2.ToLower() ?? "").Trim().Contains(filter.PeopleFullFirstName.ToLower().Trim())) &&
                            (filter.PeopleDocTypeName == null || (dt.CommonValueDisplay!=null && dt.CommonValueDisplay.ToLower().Contains(filter.PeopleDocTypeName.ToLower().Trim()) )) &&
                            (filter.PeopleDocNum == null || (p.PeopleDocNum!=null && p.PeopleDocNum.ToLower().Contains(filter.PeopleDocNum.ToLower().Trim())) ) &&
                            (filter.PeopleEmail == null || (p.PeopleEmail!=null && p.PeopleEmail.ToLower().Contains(filter.PeopleEmail.ToLower().Trim())) ) &&
                            (filter.PeopleDepartmentName == null || (dep.CommonValueDisplay!=null && dep.CommonValueDisplay.ToLower().Contains(filter.PeopleDepartmentName.ToLower().Trim())) ) &&
                            (filter.PeoplePositionName == null || (pos.CommonValueDisplay!=null && pos.CommonValueDisplay.ToLower().Contains(filter.PeoplePositionName.ToLower().Trim())) ) &&
                            //new
                            query1.Contains(p.PeopleID)
                        orderby
                            p.PeopleLastName,
                            p.PeopleLastName2,
                            p.PeopleFirstName,
                            p.PeopleFirstName2
                        select new PeopleDTO()
                        {
                            PeopleID = p.PeopleID,
                            PeopleInternalID = p.PeopleInternalID,
                            PeopleLastName = p.PeopleLastName,
                            PeopleLastName2 = p.PeopleLastName2,
                            PeopleFirstName = p.PeopleFirstName,
                            PeopleFirstName2 = p.PeopleFirstName2,
                            PeopleDocType = p.PeopleDocType,
                            PeopleDocTypeName = dt.CommonValueDisplay,

                            PeopleTypeClasificacion = p.PeopleTypeClasificacion,
                            PeopleTypeClasificacionName = tc.CommonValueDisplay,
                            PeopleEmployeeType = p.PeopleEmployeeType,
                            PeopleEmployeeTypeName = et.CommonValueDisplay,

                            PeopleDocNum = p.PeopleDocNum,
                            PeopleAddress1 = p.PeopleAddress1,
                            PeopleAddress2 = p.PeopleAddress2,
                            PeoplePhone1 = p.PeoplePhone1,
                            PeoplePhone2 = p.PeoplePhone2,
                            PeopleEmail = p.PeopleEmail,
                            PeopleDepartment = p.PeopleDepartment,
                            PeopleDepartmentName = dep.CommonValueDisplay,
                            PeoplePosition = p.PeoplePosition,
                            PeoplePositionName = pos.CommonValueDisplay,
                            UserID = p.UserID,
                            PeopleIsSourceSAP = p.PeopleIsSourceSAP,
                            AssignedItems = (from accres in db.AccessResources
                                             join rp in db.ResourcePeople on accres.ResourceID equals rp.ResourceID
                                             where
                                                rp.PresCompany == this.sessionData.CompanyID
                                                && rp.PeopleID == p.PeopleID
                                                && rp.PresActive == 1
                                             select accres.ResourceFullName
                            ).ToList(),
                            PendingApproveItems = (from pp in db.People
                                                   join req in db.AccessRequests on pp.PeopleID equals req.RequestTo
                                                   join reqd in db.AccessRequestDetails on req.RequestID equals reqd.RequestID
                                                   join accres in db.AccessResources on reqd.ResourceID equals accres.ResourceID
                                                   join wfe in db.WorkflowExecution on new { resID = reqd.RequestDetID, objName = "ACCESS_REQUEST" } equals new { resID = wfe.WfExecObjectID, objName = wfe.WfExecObjectName }
                                                   where
                                                        req.RequestCompany == this.sessionData.CompanyID
                                                        && wfe.WfExecStatus < 10
                                                        && pp.PeopleID == p.PeopleID
                                                   select accres.ResourceFullName
                            ).ToList()
                        };
            return query;
        }

        public IQueryable<PeopleDTO> GetPeopleFromExcel()
        {
            return getPeopleListFromExcel(
                AppDomain.CurrentDomain.BaseDirectory +
                ConfigurationManager.AppSettings["FilePath"],
                ConfigurationManager.AppSettings["FileName"],
                "Cesados$").AsQueryable<PeopleDTO>();
        }

        public IQueryable<PeopleDTO> GetPeopleById(int id)
        {
            return GetPeopleQuery(p => p.PeopleID == id);
        }

        public PeopleDTO GetPeopleByUserId(string UserId)
        {
            return GetPeopleQuery(p => p.UserID == UserId).FirstOrDefault();
        }
        public PeopleDTO GetPeopleByInternalID(string id)
        {
            return GetPeopleQuery(p => p.PeopleInternalID == id).FirstOrDefault();
        }
        public PeopleDTO GetPeopleByDocumentNumber(string PeopleDocNum)
        {
            return GetPeopleQuery(p => p.PeopleDocNum == PeopleDocNum).FirstOrDefault();
        }

        public IQueryable<object> GetPeopleApproversRequestPendings()
        {
            var query = from peop in db.People
                        join dep in db.CommonValues on peop.PeopleDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join pos in db.CommonValues on peop.PeoplePosition equals pos.CommonValueID into position
                        from pos in position.DefaultIfEmpty()
                        join executionParams in db.WFExecutionParameters on peop.PeopleID equals executionParams.WfExecParamIntValue
                        join execution in db.WorkflowExecution on executionParams.WfExecID equals execution.WfExecID
                        join request in db.AccessRequests on execution.WfExecParentObjectID equals request.RequestID
                        join requestCompany in db.Companies on request.RequestCompany equals requestCompany.CompanyID
                        where
                            execution.WfExecStatus == 2
                            && executionParams.WfExecParamName == "approver"
                            && request.RequestCompany == this.sessionData.CompanyID
                        orderby
                            peop.PeopleLastName,
                            peop.PeopleLastName2,
                            peop.PeopleFirstName,
                            peop.PeopleFirstName2,
                            requestCompany.CompanyName,
                            request.RequestDate
                        group new
                        {
                            RequestID = request.RequestID,
                            RequestNumber = request.RequestNumber,
                            RequestDate = request.RequestDate,
                            RequestToName = request.PeopleRequestTo.PeopleFirstName + " " + request.PeopleRequestTo.PeopleFirstName2 + ", " + request.PeopleRequestTo.PeopleLastName + " " + request.PeopleRequestTo.PeopleLastName2,
                            RequestCompanyName = requestCompany.CompanyName
                        }
                        by new
                        {
                            peop.PeopleID,
                            peop.PeopleInternalID,
                            PeopleFullname = peop.PeopleFirstName + " " + peop.PeopleFirstName2 + ", " + peop.PeopleLastName + " " + peop.PeopleLastName2,
                            //peop.PeopleLastName,
                            //peop.PeopleLastName2,
                            //peop.PeopleFirstName,
                            //peop.PeopleFirstName2,
                            peop.PeopleDocType,
                            //peop.PeopleDocTypeName,
                            peop.PeopleDocNum,
                            peop.PeopleAddress1,
                            peop.PeopleAddress2,
                            peop.PeoplePhone1,
                            peop.PeoplePhone2,
                            peop.PeopleEmail,
                            peop.PeopleDepartment,
                            PeopleDepartmentName = dep.CommonValueDisplay,
                            //peop.PeopleDepartmentName,
                            peop.PeoplePosition,
                            PeoplePositionName = pos.CommonValueDisplay,
                            //peop.PeoplePositionName,
                            peop.PeopleAttribute2,
                            peop.PeopleProject,
                            peop.PeopleAttribute3,
                            peop.UserID,
                            peop.PeopleStatus
                        }
                        into g
                        select new
                        {
                            g.Key.PeopleID,
                            g.Key.PeopleInternalID,
                            g.Key.PeopleFullname,
                            //g.Key.PeopleLastName,
                            //g.Key.PeopleLastName2,
                            //g.Key.PeopleFirstName,
                            //g.Key.PeopleFirstName2,
                            g.Key.PeopleDocType,
                            //peop.PeopleDocTypeName,
                            g.Key.PeopleDocNum,
                            g.Key.PeopleAddress1,
                            g.Key.PeopleAddress2,
                            g.Key.PeoplePhone1,
                            g.Key.PeoplePhone2,
                            g.Key.PeopleEmail,
                            g.Key.PeopleDepartment,
                            g.Key.PeopleDepartmentName,
                            g.Key.PeoplePosition,
                            g.Key.PeoplePositionName,
                            g.Key.PeopleAttribute2,
                            g.Key.PeopleProject,
                            g.Key.PeopleAttribute3,
                            g.Key.UserID,
                            g.Key.PeopleStatus,
                            ForApprove = g.Count(),
                            Requests = g.ToList()
                        };
            return query;
        }

        public IQueryable<PeopleDTO> GetPeopleQuery(Expression<Func<PeopleDTO, bool>> whereExpr)
        {
            var query = from p in db.People
                        join company in db.Companies on p.PeopleCompany equals company.CompanyID
                        join dt in db.CommonValues on p.PeopleDocType equals dt.CommonValueID into doctype
                        from dt in doctype.DefaultIfEmpty()

                        join au in db.AccessUsers on p.UserID.ToLower() equals au.UserInternalID.ToLower() into alias
                        from au in alias.DefaultIfEmpty()

                        join tc in db.CommonValues on p.PeopleTypeClasificacion equals tc.CommonValueID into TypeClasificacion
                        from tc in TypeClasificacion.DefaultIfEmpty()
                        join et in db.CommonValues on p.PeopleEmployeeType equals et.CommonValueID into EmployeeType
                        from et in EmployeeType.DefaultIfEmpty()

                        join dep in db.CommonValues on p.PeopleDepartment equals dep.CommonValueID into department
                        from dep in department.DefaultIfEmpty()
                        join pos in db.CommonValues on p.PeoplePosition equals pos.CommonValueID into position
                        from pos in position.DefaultIfEmpty()
                        orderby
                            p.PeopleLastName,
                            p.PeopleLastName2,
                            p.PeopleFirstName,
                            p.PeopleFirstName2
                        select new PeopleDTO()
                        {
                            PeopleID = p.PeopleID,
                            PeopleInternalID = p.PeopleInternalID,
                            PeopleLastName = p.PeopleLastName,
                            PeopleLastName2 = p.PeopleLastName2,
                            PeopleFirstName = p.PeopleFirstName,
                            PeopleFirstName2 = p.PeopleFirstName2,
                            PeopleDocType = p.PeopleDocType,
                            PeopleDocTypeName = dt.CommonValueDisplay,

                            PeopleTypeClasificacion = p.PeopleTypeClasificacion,
                            PeopleTypeClasificacionName = tc.CommonValueDisplay,
                            PeopleEmployeeType = p.PeopleEmployeeType,
                            PeopleEmployeeTypeName = et.CommonValueDisplay,

                            PeopleDocNum = p.PeopleDocNum,
                            PeopleAddress1 = p.PeopleAddress1,
                            PeopleAddress2 = p.PeopleAddress2,
                            PeoplePhone1 = p.PeoplePhone1,
                            PeoplePhone2 = p.PeoplePhone2,
                            PeopleEmail = p.PeopleEmail,
                            PeopleBirthday = p.PeopleBirthday,
                            PeopleGender = p.PeopleGender,
                            PeopleCompany = p.PeopleCompany,
                            PeopleCompanyName = company.CompanyName,
                            PeopleDepartment = p.PeopleDepartment,
                            PeopleDepartmentName = dep.CommonValueDisplay,
                            PeoplePosition = p.PeoplePosition,
                            PeoplePositionName = pos.CommonValueDisplay,
                            PeopleAttribute2 = p.PeopleAttribute2,
                            PeopleProject = p.PeopleProject,
                            PeopleAttribute3 = p.PeopleAttribute3,
                            PeopleIsSourceSAP = p.PeopleIsSourceSAP,
                            UserID = p.UserID,
                            UserInternalID=au.UserInternalID,
                            PeopleStatus = p.PeopleStatus
                        };
            if (whereExpr != null)
            {
                query = query.Where(whereExpr);
            }
            return query;
        }

        public PeopleDTO SavePeople(PeopleDTO people)
        {
            return SavePeople(people, this.sessionData.sessionUser, null);
        }
        public PeopleDTO SavePeople(PeopleDTO people, int companyID)
        {
            return SavePeople(people, this.sessionData.sessionUser, companyID);
        }
        public PeopleDTO SavePeople(PeopleDTO people, string EditUser, int? companyID = null)
        {
            try
            {
                People model = null;

                model = db.People.FirstOrDefault(a => a.PeopleInternalID == people.PeopleInternalID); // get from context
                if (model == null)
                    model = db.People.Create();  // create new from context 
                else
                    people.PeopleID = model.PeopleID;

                if (Validate(model, people, EditUser)) //&& ValidateAD(sessionUser, model))
                {
                    Mapper.Map<PeopleDTO, People>(people, model);
                    model.PeopleCompany = companyID ?? this.sessionData.CompanyID;
                    model.EditUser = EditUser;
                    if (SaveEntity(model.PeopleID == 0, model))
                    {
                        people = AutoMapper.Mapper.Map<PeopleDTO>(model);
                        return people;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
            return null;
        }

        public PeopleDTO SavePeopleFromAuthenticationData(AuthenticationDataDTO authData)
        {
            IQueryable<People> query = null;
            PeopleDTO people = null;
            string documentNumber = authData.numeroDocumento.Trim();
            int queryCount = 0;

            // Search -----------------------------------------------------------------------------------------
            query = (
                from p in db.People
                where
                    p.UserID == authData.userAlias

                    || p.PeopleDocNum == documentNumber
                    || p.PeopleDocNum.Substring(p.PeopleDocNum.Length - documentNumber.Length) == documentNumber
                    || documentNumber.Substring(documentNumber.Length - p.PeopleDocNum.Length) == p.PeopleDocNum

                    || p.PeopleInternalID == documentNumber
                    || p.PeopleInternalID.Substring(p.PeopleInternalID.Length - documentNumber.Length) == documentNumber
                    || documentNumber.Substring(documentNumber.Length - p.PeopleInternalID.Length) == p.PeopleInternalID
                orderby
                    p.CreateDate descending
                select p
            );
            queryCount = query.Count();

            if (queryCount > 1)  // Si hay mas de una coincidencia se va buscando dentro de los resultados
            {
                return null; // no se hace cambios a registros duplicados (porque genera errores), se refactorizará al corregir los registros de personas
                //IQueryable<People> subQuery;

                //subQuery = query.Where(p => p.UserID == authData.userAlias);     // Search USER_ID
                //if (subQuery.Count() == 1)
                //{
                //    people = Mapper.Map<People, PeopleDTO>(subQuery.FirstOrDefault());
                //}

                //subQuery = query.Where(p => p.PeopleDocNum == documentNumber);   // Search NUM_DOC
                //if (people == null && subQuery.Count() == 1)
                //{
                //    people = Mapper.Map<People, PeopleDTO>(subQuery.FirstOrDefault());
                //}
            }
            else if (queryCount == 1)  // Si sólo hay una coincidencia se carga en el objeto PeopleDTO
            {
                people = Mapper.Map<People, PeopleDTO>(query.FirstOrDefault());
                // Por ahora estará aqui hasta q se corrijan los registros duplicados v
                people.PeopleInternalID = authData.numeroDocumento.Trim();
                people.PeopleDocNum = authData.numeroDocumento.Trim();
                // Por ahora estará aqui hasta q se corrijan los registros duplicados ^
            }
            else if (queryCount == 0) // Si no hay coincidencias se crea un PeopleDTO nuevo
            {
                people = new PeopleDTO();
                // Por ahora estará aqui hasta q se corrijan los registros duplicados v
                people.PeopleInternalID = authData.numeroDocumento.Trim();
                people.PeopleDocNum = authData.numeroDocumento.Trim();
                // Por ahora estará aqui hasta q se corrijan los registros duplicados ^

                //people.PeopleCompany = 4;   // Por defecto: Aenza
                var company = new CompanyRepository().GetCompanyByAD(authData.empresa.Trim()).FirstOrDefault();
                if (company != null)
                {
                    people.PeopleCompany = company.CompanyID;
                }
                else
                {
                    throw new ModelException("El nombre de la empresa asociada a su usuario no se encuentra registrada en ICARUS, comuníquese con Mesa de ayuda");
                }
            }

            if (people == null)
            {
                throw new ModelException("Error al guardar el registro de empleado.");
            }

            // Se ingresan los demás atributos ----------------------------------------------------------
            //people.PeopleInternalID = authData.numeroDocumento.Trim();            // Comentado hasta q se corrijan los registros duplicados
            people.PeopleDocType = people.PeopleDocType ?? PeopleDTO.DOC_TYPE_DNI;
            //people.PeopleDocNum = authData.numeroDocumento.Trim();                // Comentado hasta q se corrijan los registros duplicados
            people.PeopleEmail = authData.username.Trim();                          // Correo completo
            people.PeopleLastName = authData.surname.Trim();
            people.PeopleLastName2 = "";
            people.PeopleFirstName = authData.givenName.Trim();
            people.PeopleFirstName2 = "";
            people.PeopleAttribute3 = authData.jobTitle?.Trim();
            people.PeopleTypeClasificacion = PeopleDTO.CLAS_TYPE_COLABORADOR;
            people.UserID = authData.userAlias;
            people.PeopleStatus = 1;
            return SavePeople(people,people.PeopleCompany);
        }

        public void UpdateActive(int peopleID, bool active, string userID)
        {
            try
            {
                var model = db.People.FirstOrDefault(a => a.PeopleID == peopleID);
                if (model == null)
                    throw new ModelException("Persona no encontrada para el cambio de estado");
                model.PeopleStatus = active ? 1 : 0;
                model.EditUser = userID;
                SaveEntity(false, model);
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
        }

        public String[] SavePeopleFromExcel()
        {
            // Validation
            string[] filtro = null;
            if (!(this.sessionData.UserRole3 > 0 || this.sessionData.UserRole4 > 0))  // rol "admnistrador" o "crear personas"
            {
                throw new ModelException("No tiene permiso para realizar la carga de personas.");
            }
            String[] result = new String[] { "", "", "", "" };
            int newPeopleCount = 0, updatedPeopleCount = 0, errorCount = 0;
            DataTable peopleDT = new ExcelReader().fillDtFromExcel(
                AppDomain.CurrentDomain.BaseDirectory +
                ConfigurationManager.AppSettings["FilePath"],
                ConfigurationManager.AppSettings["FileName"],
                "Activos$");
            if (peopleDT == null)
                throw new ModelException("El formato del archivo no es válido");
            // Validaciones ---------------------------------------------------------------------------------------------- //
            bool isValid = true;
            string[] columNames = { "Nombre lugar trabajo", "ID CC", "Unidad org#", "ID RH", "Nombre", "Fecha Ingreso", "Nombre de Puesto", "Nombre categoría puesto" };
            for (int idx = 0; idx < columNames.Length; idx++)
            {
                isValid = isValid && peopleDT.Columns.IndexOf(columNames[idx]) == idx;
            }
            if (!isValid)
                throw new ModelException("El formato del archivo no es válido");
            // Validaciones ---------------------------------------------------------------------------------------------- //

            SisConAxsContext db = new SisConAxsContext();
            IQueryable<AccessResources> flaggedResourcesList = db.AccessResources.Where(p => p.ResourceFlag == 1);
            NotifConfigDTO config = new SystemConfigRepository().GetInitLoadFlag();
            NotifConfigDTO configExcel = new SystemConfigRepository().GetExcelFilter();
            try
            {
                filtro = configExcel.NotifConfHost.Split(',');
            }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                throw new ModelException("Error al crear los filtros a partir de los datos configurados en la BD");
            }

            //AccessUsers user = new AccessUsers() { UserInternalID = "SYSTEM" };
            string peopleInternalID = "";
            bool initLoad = config.NotifConfHost == "1";
            PeopleDTO people = null;
            for (int i = 0; i < peopleDT.Rows.Count; i++)
            {
                if (
                    !String.IsNullOrWhiteSpace(peopleDT.Rows[i].ItemArray[3].ToString())  // DNI
                    && !String.IsNullOrWhiteSpace(peopleDT.Rows[i].ItemArray[4].ToString())  // Nombre completo
                    && comesFromAcceptableCategory(filtro, peopleDT.Rows[i].ItemArray[7].ToString().ToUpper())  //Solo aceptamos gente cuya categoria este incluida en el filtro
                  )
                {
                    string[] fullName = getName(peopleDT.Rows[i].ItemArray[4].ToString());
                    string[] doc = getDoc(peopleDT.Rows[i].ItemArray[3].ToString());

                    if (doc != null && fullName != null)
                    {
                        peopleInternalID = peopleDT.Rows[i].ItemArray[3].ToString();
                        people = new PeopleRepository(this.sessionData).GetPeopleQuery(p => p.PeopleInternalID == peopleInternalID).FirstOrDefault(); // busca si ya existe en la BD
                        if (people == null)
                        {
                            people = new PeopleDTO();
                            ++newPeopleCount;
                        }
                        else
                            ++updatedPeopleCount;

                        people.PeopleFirstName = fullName[0];
                        people.PeopleFirstName2 = fullName[1];
                        people.PeopleLastName = fullName[2];
                        people.PeopleLastName2 = fullName[3];
                        people.PeopleInternalID = peopleDT.Rows[i].ItemArray[3].ToString();
                        people.PeopleDocType = Convert.ToInt32(doc[0]);
                        people.PeopleDocNum = doc[1];
                        people.PeopleStatus = 1;

                        (new PeopleRepository(this.sessionData)).SavePeople(people); //, this.sessionData.sessionUser);
                        if (initLoad)
                        {
                            AddResourcePeople(flaggedResourcesList, people.PeopleID);
                        }
                    }
                    else
                    {
                        errorCount++;

                        result[3] += peopleDT.Rows[i].ItemArray[3].ToString() + " - " +
                            peopleDT.Rows[i].ItemArray[4].ToString() + "<br />";
                    }
                }
            }
            if (initLoad)
            {
                config.NotifConfHost = "0";
                new SystemConfigRepository().SaveNotifConfig(config, this.sessionData.sessionUser);
            }

            result[0] = newPeopleCount.ToString();
            result[1] = updatedPeopleCount.ToString();
            result[2] = errorCount.ToString();
            return result;
        }

        public List<PeopleDTO> getPeopleListFromExcel(string filePath, string fileName, string sheetName, Func<DataTable, bool> validation = null)
        {
            DataTable dt = new ExcelReader().fillDtFromExcel(filePath, fileName, sheetName);
            if (dt == null)
            {
                throw new ModelException("La hoja no ha sido encontrada en el archivo Excel.");
            }

            if (validation != null)
            {
                validation(dt);
            }

            List<PeopleDTO> peopleList = new List<PeopleDTO>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].ItemArray[0].ToString().Equals(""))
                    break;

                string peopleInternalID = dt.Rows[i].ItemArray[0].ToString();

                if (String.IsNullOrWhiteSpace(peopleInternalID))
                    throw new Exception(String.Format("[{0}] No tiene un Código Interno válido", i)); //tmp="aqui";


                int pplId = (from pp in db.People where pp.PeopleDocNum == peopleInternalID && pp.PeopleCompany == this.sessionData.CompanyID select pp.PeopleID).FirstOrDefault();
                if (pplId > 0)
                {
                    if (peopleList.Count(x => x.PeopleInternalID == peopleInternalID) == 0)
                    {
                        PeopleDTO people = (from p in db.People
                                            where p.PeopleID == pplId
                                            select new PeopleDTO()
                                            {
                                                PeopleInternalID = p.PeopleInternalID,
                                                PeopleID = p.PeopleID,
                                                PeopleFirstName = p.PeopleFirstName,
                                                PeopleFirstName2 = p.PeopleFirstName2,
                                                PeopleLastName = p.PeopleLastName,
                                                PeopleLastName2 = p.PeopleLastName2,
                                                PeopleDocType = p.PeopleDocType,
                                                PeopleDocNum = p.PeopleDocNum,
                                                PeopleStatus = p.PeopleStatus,
                                                AssignedItems = (from accres in db.AccessResources
                                                                 join rp in db.ResourcePeople on accres.ResourceID equals rp.ResourceID
                                                                 where rp.PresActive == 1 && rp.PeopleID == pplId
                                                                    && rp.PresCompany == this.sessionData.CompanyID
                                                                 select accres.ResourceFullName).ToList(),
                                                PendingApproveItems = (from pp in db.People
                                                                       join req in db.AccessRequests on pp.PeopleID equals req.RequestTo
                                                                       join reqd in db.AccessRequestDetails on req.RequestID equals reqd.RequestID
                                                                       join accres in db.AccessResources on reqd.ResourceID equals accres.ResourceID
                                                                       join wfe in db.WorkflowExecution on new { resID = reqd.RequestDetID, objName = "ACCESS_REQUEST" } equals new { resID = wfe.WfExecObjectID, objName = wfe.WfExecObjectName }
                                                                       where wfe.WfExecStatus < 10 && pp.PeopleID == pplId
                                                                            && req.RequestCompany == this.sessionData.CompanyID
                                                                       select accres.ResourceFullName).ToList()
                                            }).FirstOrDefault();

                        if (people.AssignedItems.Count() == 0 && people.PendingApproveItems.Count() == 0)
                        {
                            people.Failed = true;
                            people.FailedDescription = "No se encontraron recursos asignados";
                        }
                        peopleList.Add(people);
                    }
                }
                else
                {
                    peopleList.Add(new PeopleDTO()
                    {
                        PeopleInternalID = peopleInternalID,
                        Failed = true,
                        FailedDescription = "Número ID no encontrado"
                    });
                }
            }

            return peopleList;
        }

        protected void AddResourcePeople(IQueryable<AccessResources> flaggedResourcesList, int peopleID)
        {
            SisConAxsContext db = new SisConAxsContext();
            foreach (AccessResources resource in flaggedResourcesList)
            {
                ResourcePeople model = db.ResourcePeople.Create();
                model.PeopleID = peopleID;
                model.ResourceID = resource.ResourceID;
                model.AddedRequestID = null;
                model.AddedRequestDetID = null;
                db.ResourcePeople.Add(model);
                db.SaveChanges();
            }
        }

        protected bool comesFromAcceptableCategory(string[] arrayCategory, string category)
        {
            if (arrayCategory == null || arrayCategory.Length == 0)
                return true;

            foreach (string cate in arrayCategory)
            {
                if (cate.Equals(category))
                    return true;
            }
            return false;
        }

        protected string[] getName(string fullName)
        {//fullName="Anthony Juan Alberto, Romero Perez"
            string[] result = new string[] { "", "", "", "" };
            string[] names = fullName.Split(',');
            if (names.Length <= 1)
            {
                //result[0] = fullName;
                return null;
            }

            string[] lastName = names[0].Split(' ');//["Romero", "Perez" ]
            string[] name = names[1].Split(' '); //["Anthony", "Juan", "Alberto"]
            if (lastName.Length == 2)
            {
                result[2] = lastName[0];
                result[3] = lastName[1];
            }
            else
            {
                for (int i = 0; i < lastName.Length; i++)
                {
                    result[2] += lastName[i];
                    if (i < lastName.Length - 1) result[2] += " ";
                }
                result[3] = "";
            }

            if (name.Length == 3)
            {
                result[0] = name[1];
                result[1] = name[2];
            }
            else
            {
                for (int i = 0; i < name.Length; i++)
                {
                    result[0] += name[i];
                    if (i < name.Length - 1) result[0] += " ";
                }
                result[1] = "";
            }

            return result;
        }

        protected string[] getDoc(string internalID)
        {//0: primeraLetra, 1: numero, 2: commonValue
            string[] doc = new string[3];
            doc[0] = internalID.Substring(0, 1);
            switch (doc[0])
            {
                case "D":
                    doc[0] = "1"; doc[2] = "DNI";
                    break;
                case "P":
                    doc[0] = "2"; doc[2] = "Pasaporte";
                    break;
                case "E":
                    doc[0] = "3"; doc[2] = "C.E";
                    break;
                default:
                    return null;
            }
            doc[1] = internalID.Substring(1);
            //Primero trate de leer el nombre del documento desde la BD pero si la persona no existe entonces no mostrara el tipo de doc
            //doc[2] = (from p in db.People
            //                join d in db.CommonValues on p.PeopleDocType equals d.CommonValueID
            //                where p.PeopleInternalID == internalID
            //                select d.CommonValueDisplay).FirstOrDefault();

            return doc;
        }


        private bool ValidatePersona(People model, PeopleDTO dto)
        {
            // Valida que no se pueda editar si la empresa no es igual a la de la sesion activa
            //if ((model.PeopleCompany > 0 && model.PeopleCompany != this.sessionData.CompanyID) && model.PeopleStatus > 0)
            //{
            //    throw new ModelException("No puede editar a una persona de otra empresa, primero debe estar inactiva.");
            //}

            // ID interno
            if (String.IsNullOrEmpty(dto.PeopleInternalID))
            {
                throw new ModelException("No ha asignado un ID interno.");
            }

            // Nombre
            if (String.IsNullOrEmpty(dto.PeopleFirstName) || String.IsNullOrEmpty(dto.PeopleLastName))
            {
                throw new ModelException("No ha asignado un nombre y/o un apellido a la persona.");
            }

            // Num doc
            if (String.IsNullOrEmpty(dto.PeopleDocNum))
            {
                throw new ModelException("No ha asignado un número de documento de identidad.");
            }

            // Valida que no exista otra persona con el mismo Tipo de Documento y mismo Número de Documento
            if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleDocType == dto.PeopleDocType && t.PeopleDocNum.Trim() == dto.PeopleDocNum.Trim()))
            {
                throw new ModelException("Ya existe una persona registrada con el mismo número de Documento de Identidad.");
            }

            // Valida que no exista otra persona con el mismo Número de ID Interno
            if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleInternalID.ToUpper().Trim() == dto.PeopleInternalID.ToUpper().Trim()))
            {
                throw new ModelException("Ya existe una persona registrada con el mismo número de ID Interno.");
            }

            // Valida que la dirección de correo no se repita
            //if (!String.IsNullOrEmpty(dto.PeopleEmail))
            //{
            //    if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleEmail.ToLower().Trim() == dto.PeopleEmail.ToLower().Trim()))
            //    {
            //        throw new ModelException("La dirección de correo ya esta siendo utilizada.");
            //    }
            //}

            // Valida que un usuario este asignado a mas de una persona
            if (!String.IsNullOrEmpty(dto.UserID))
            {
                dto.UserID = dto.UserID.Trim().ToUpper();
                if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.UserID.Trim().ToUpper() == dto.UserID.Trim().ToUpper()))
                {
                    throw new ModelException("Ya existe una persona registrada con el mismo usuario.");
                }
            }

            // Valida que un usuario no pueda tener el mismo cargo y área workflow
            var peoplePosition = db.People.Where(x => x.PeopleID != dto.PeopleID && dto.PeopleDepartment != null && dto.PeoplePosition != null && x.PeopleDepartment == dto.PeopleDepartment && x.PeoplePosition == dto.PeoplePosition && x.PeopleCompany == dto.PeopleCompany && x.PeopleStatus == 1).FirstOrDefault();
            if (peoplePosition != null)
            {
                throw new ModelException("Ya existe una persona con el mismo cargo y área (workflow): " + peoplePosition.GetFullName());
            }

            return true;
        }
        private bool Validate(People model, PeopleDTO dto, string EditUser = null)
        {
            if (!(EditUser == ConstantsDM.USER_SYSTEM_ICARUS || this.sessionData.UserRoleSysAdmin > 0))  // Si es el usuario del sistema o un sysadmin no se valida
            {
                // Valida que no se pueda editar si la empresa no es igual a la de la sesion activa
                if ((model.PeopleCompany > 0 && model.PeopleCompany != this.sessionData.CompanyID) && model.PeopleStatus > 0)
                {
                    throw new ModelException("No puede editar a una persona de otra empresa, primero debe estar inactiva.");
                }
            }


            // ID interno
            if (String.IsNullOrEmpty(dto.PeopleInternalID))
            {
                throw new ModelException("No ha asignado un ID interno.");
            }

            // Nombres, Apellidos
            if (String.IsNullOrEmpty(dto.PeopleFirstName))
            {
                throw new ModelException("No ha asignado un nombre.");
            }
            if (dto.PeopleTypeClasificacion != PeopleDTO.CLAS_TYPE_CLIENTE && dto.PeopleTypeClasificacion != PeopleDTO.CLAS_TYPE_PROVEEDOR)
            {
                if (dto.PeopleDocType != PeopleDTO.DOC_TYPE_RUC && String.IsNullOrEmpty(dto.PeopleLastName))
                {
                    throw new ModelException("No ha asignado un apellido.");
                }
            }


            // Num doc
            if (String.IsNullOrEmpty(dto.PeopleDocNum))
            {
                throw new ModelException("No ha asignado un número de documento de identidad.");
            }

            // Valida que no exista otra persona con el mismo Tipo de Documento y mismo Número de Documento
            if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleDocType == dto.PeopleDocType && t.PeopleDocNum.Trim() == dto.PeopleDocNum.Trim()))
            {
                throw new ModelException("Ya existe una persona registrada con el mismo número de Documento de Identidad.");
            }

            // Valida que no exista otra persona con el mismo Número de ID Interno
            if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleInternalID.ToUpper().Trim() == dto.PeopleInternalID.ToUpper().Trim()))
            {
                throw new ModelException("Ya existe una persona registrada con el mismo número de ID Interno.");
            }

            // Valida que la dirección de correo no se repita
            //if (!String.IsNullOrEmpty(dto.PeopleEmail))
            //{
            //    if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.PeopleEmail.ToLower().Trim() == dto.PeopleEmail.ToLower().Trim()))
            //    {
            //        throw new ModelException("La dirección de correo ya esta siendo utilizada.");
            //    }
            //}

            // Valida que un usuario este asignado a mas de una persona
            if (!String.IsNullOrEmpty(dto.UserID))
            {
                dto.UserID = dto.UserID.Trim().ToUpper();
                if (db.People.Any(t => t.PeopleID != dto.PeopleID && t.UserID.Trim().ToUpper() == dto.UserID.Trim().ToUpper()))
                {
                    throw new ModelException("Ya existe una persona registrada con el mismo usuario.");
                }
            }

            // Valida que un usuario no pueda tener el mismo cargo y área workflow
            var peoplePosition = db.People.Where(x => x.PeopleID != dto.PeopleID && dto.PeopleDepartment != null && dto.PeoplePosition != null && x.PeopleDepartment == dto.PeopleDepartment && x.PeoplePosition == dto.PeoplePosition && x.PeopleCompany == dto.PeopleCompany && x.PeopleStatus == 1).FirstOrDefault();
            if (peoplePosition != null)
            {
                throw new ModelException("Ya existe una persona con el mismo cargo y área (workflow): " + peoplePosition.GetFullName());
            }

            return true;
        }

        public bool ValidateAD(AccessUsers sessionUser, People model)
        {
            int count = 0;

            string domain = new SystemConfigRepository().GetADdomainConfig().NotifConfHost;
            GyM.Security.ActiveDirectory ad = new GyM.Security.ActiveDirectory();
            System.Data.DataTable dt = ad.GetUsers(domain, GyM.Security.ActiveDirectory.Filter.Nombre, "*", sessionUser.UserInternalID, sessionUser.UserPassword);
            var query = from usersAD in dt.Select()
                        select new
                        {
                            DocType = !String.IsNullOrWhiteSpace(usersAD["postalCode"].ToString()) ? usersAD["postalCode"].ToString().Substring(0, 1) : "",
                            DocNumber = !String.IsNullOrWhiteSpace(usersAD["postalCode"].ToString()) ? usersAD["postalCode"].ToString().Substring(1, usersAD["postalCode"].ToString().Length - 1) : ""
                        };


            string docTypeCode = "";
            CommonValues docType = db.CommonValues.FirstOrDefault(t => t.CommonValueID == model.PeopleDocType);
            if (docType != null)
            {
                switch (docType.CommonValueName)
                {
                    case "DNI":
                        docTypeCode = "D";
                        break;
                    case "PASAPORTE":
                        docTypeCode = "";
                        break;
                    case "C_E":
                        docTypeCode = "E";
                        break;
                }
            }

            // Valida si ya existe un usuario con ese numero de Documento de documento de identidad
            count = query.Count(t => t.DocType == docTypeCode && t.DocNumber == model.PeopleDocNum);
            if (String.IsNullOrWhiteSpace(model.UserID) && count > 0)
            {
                throw new ModelException("Ya existe un usuario en AD con este número de Documento de Identidad.");
            }

            return true;
        }

        public PeopleDTO DeletePeople(int id)
        {
            People model = db.People.FirstOrDefault(a => a.PeopleID == id);
            int count = 0;
            bool canDelete = true;

            // Valida si la persona no ha sido Aprobador
            count = db.WFExecutionParameters.Count(x => x.WfExecParamName == "approver" && x.WfExecParamIntValue == id);
            canDelete = canDelete && count == 0;

            // Valida que no este en las Solicitudes
            count = db.AccessRequests.Count(x => x.RequestTo == id);
            canDelete = canDelete && count == 0;

            // Valida que no tenga Recursos Asignados
            count = db.ResourcePeople.Count(x => x.PeopleID == id);
            canDelete = canDelete && count == 0;

            //También se inactiva el usuario
            AccessUsers modelUser = db.AccessUsers.FirstOrDefault(a => a.UserInternalID == model.UserID);
            if (modelUser != null)
            {
                modelUser.UserStatus = 0;  // desactiva al usuario
                SaveEntity(modelUser.UserID == 0, model);
                AccessUserDTO dtoUser = new AccessUserDTO();
                Mapper.Map<AccessUsers, AccessUserDTO>(modelUser, dtoUser);
            }

            if (canDelete)
            {
                DeleteEntity(model);
                return null;
            }

            model.PeopleStatus = 0;  // desactiva a la persona
            SaveEntity(model.PeopleID == 0, model);
            PeopleDTO dto = new PeopleDTO();
            Mapper.Map<People, PeopleDTO>(model, dto);
            return dto;
        }

        /// <summary>Llamada para obtener la lista de Personas paginadas.</summary>
        /// <para>GET api/People </para>
        /// <returns>Lista de People</returns>
        public PaginationResponse<PeopleDTO> GetPeoplePaginate(PeopleFilter filter)
        {
            IQueryable<PeopleDTO> query =
                    from p in db.People
                    join company in db.Companies on p.PeopleCompany equals company.CompanyID
                    join dt in db.CommonValues on p.PeopleDocType equals dt.CommonValueID into doctype
                    from dt in doctype.DefaultIfEmpty()

                    join au in db.AccessUsers on p.UserID.ToLower() equals au.UserInternalID.ToLower() into alias
                    from au in alias.DefaultIfEmpty()

                    join tc in db.CommonValues on p.PeopleTypeClasificacion equals tc.CommonValueID into TypeClasificacion
                    from tc in TypeClasificacion.DefaultIfEmpty()
                    join et in db.CommonValues on p.PeopleEmployeeType equals et.CommonValueID into EmployeeType
                    from et in EmployeeType.DefaultIfEmpty()

                    join dep in db.CommonValues on p.PeopleDepartment equals dep.CommonValueID into department
                    from dep in department.DefaultIfEmpty()
                    join pos in db.CommonValues on p.PeoplePosition equals pos.CommonValueID into position
                    from pos in position.DefaultIfEmpty()
                    orderby
                        p.PeopleLastName,
                        p.PeopleLastName2,
                        p.PeopleFirstName,
                        p.PeopleFirstName2

                    select new PeopleDTO()
                    {
                        PeopleID = p.PeopleID,
                        PeopleInternalID = p.PeopleInternalID,
                        PeopleLastName = p.PeopleLastName,
                        PeopleLastName2 = p.PeopleLastName2,
                        PeopleFirstName = p.PeopleFirstName,
                        PeopleFirstName2 = p.PeopleFirstName2,
                        PeopleDocType = p.PeopleDocType,
                        PeopleDocTypeName = dt.CommonValueDisplay,

                        PeopleTypeClasificacion = p.PeopleTypeClasificacion,
                        PeopleTypeClasificacionName = tc.CommonValueDisplay,
                        PeopleEmployeeType = p.PeopleEmployeeType,
                        PeopleEmployeeTypeName = et.CommonValueDisplay,

                        PeopleDocNum = p.PeopleDocNum,
                        PeopleAddress1 = p.PeopleAddress1,
                        PeopleAddress2 = p.PeopleAddress2,
                        PeoplePhone1 = p.PeoplePhone1,
                        PeoplePhone2 = p.PeoplePhone2,
                        PeopleEmail = p.PeopleEmail,
                        PeopleCompany = p.PeopleCompany,
                        PeopleCompanyName = company.CompanyName,
                        PeopleDepartment = p.PeopleDepartment,
                        PeopleDepartmentName = dep.CommonValueDisplay,
                        PeoplePosition = p.PeoplePosition,
                        PeoplePositionName = pos.CommonValueDisplay,
                        PeopleAttribute2 = p.PeopleAttribute2,
                        PeopleProject = p.PeopleProject,
                        PeopleAttribute3 = p.PeopleAttribute3,
                        UserID = p.UserID,
                        UserInternalID = au.UserInternalID,
                        PeopleStatus = p.PeopleStatus,
                        PeopleFullFirstName = p.PeopleFirstName + " " + p.PeopleFirstName2,
                        PeopleFullLastName = p.PeopleLastName + " " + p.PeopleLastName2,
                        PeopleStatusDesc = p.PeopleStatus == 0 ? "Inactivo" : "Activo"
                    };

            // filters
            query = query.Where(p => filter.allCompanies ? true : p.PeopleCompany == this.sessionData.CompanyID);
            query = query.Where(p => filter.status == -1 ? true : p.PeopleStatus == filter.status);

            // table filters
            if (!String.IsNullOrWhiteSpace(filter.PeopleInternalID))
            {
                query = query.Where(x => x.PeopleInternalID.ToString().Contains(filter.PeopleInternalID));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleFullFirstName))
            {
                query = query.Where(x => x.PeopleFullFirstName.ToString().Contains(filter.PeopleFullFirstName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleFullLastName))
            {
                query = query.Where(x => x.PeopleFullLastName.ToString().Contains(filter.PeopleFullLastName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleDocTypeName))
            {
                query = query.Where(x => x.PeopleDocTypeName.ToString().Contains(filter.PeopleDocTypeName));
            }


            if (filter.PeopleTypeClasificacion != null)
            {
                query = query.Where(x => x.PeopleTypeClasificacion == filter.PeopleTypeClasificacion);
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleTypeClasificacionName))
            {
                query = query.Where(x => x.PeopleTypeClasificacionName.ToString().Contains(filter.PeopleTypeClasificacionName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleEmployeeTypeName))
            {
                query = query.Where(x => x.PeopleEmployeeTypeName.ToString().Contains(filter.PeopleEmployeeTypeName));
            }


            if (!String.IsNullOrWhiteSpace(filter.PeopleDocNum))
            {
                query = query.Where(x => x.PeopleDocNum.ToString().Contains(filter.PeopleDocNum));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleEmail))
            {
                query = query.Where(x => x.PeopleEmail.ToString().Contains(filter.PeopleEmail));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleDepartmentName))
            {
                query = query.Where(x => x.PeopleDepartmentName.Contains(filter.PeopleDepartmentName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeoplePositionName))
            {
                query = query.Where(x => x.PeoplePositionName.Contains(filter.PeoplePositionName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleCompanyName))
            {
                query = query.Where(x => x.PeopleCompanyName.Contains(filter.PeopleCompanyName));
            }
            if (!String.IsNullOrWhiteSpace(filter.PeopleStatusDesc))
            {
                query = query.Where(x => x.PeopleStatusDesc.Equals(filter.PeopleStatusDesc));
            }

            // response
            var response = new PaginationResponse<PeopleDTO>();
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
    }

}
