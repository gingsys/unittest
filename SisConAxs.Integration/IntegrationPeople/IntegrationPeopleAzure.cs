using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using SisConAxs.Integration.Helpers.AzureAuthentication;
using SisConAxs.Integration.IntegrationUser;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SisConAxs.Integration
{
    public class IntegrationPeopleAzure
    {
        private CompanyRepository CompanyRepository;
        private PeopleRepository PeopleRepository;
        private IntegrationAADLogRepository IntegrationAADLogRepository;
        private const string USER_ID = "integration.azure";
        private SessionData USER_SESSIONDATA = new SessionData()
        {
            User = new AccessUserDTO() { UserFirstName = "Intergración Azure", UserLastName = "Intergración Azure" },
            sessionUser = USER_ID,
            UserRole3 = 1,        // ADMIN
            UserRoleSysAdmin = 1  // SYSADMIN
        };

        public IntegrationPeopleAzure()
        {
            CompanyRepository = new CompanyRepository();
            PeopleRepository = new PeopleRepository(USER_SESSIONDATA);
            IntegrationAADLogRepository = new IntegrationAADLogRepository();
        }

        public void SyncData()
        {
            try
            {
                var ADUsers = IntegrationUserAzure.GetOrganizationUsers();
                var companies = CompanyRepository.GetCompanies().Where(c => c.CompanyID > 0);
                foreach (var company in companies)
                {
                    // get list companies AD
                    var companiesAD = CompanyRepository.GetCompaniesAD(company.CompanyID);
                    foreach (var companyADName in companiesAD)
                    {
                        // update people
                        var users = ADUsers.FindAll(u => u.CompanyName == companyADName);
                        foreach (var user in users)
                        {
                            string messagePeople = "Destinatario (Persona): No ejecutado.\n\n";

                            IntegrationAADLogDTO log = user.ToIntegrationAADLogDTO();
                            PeopleDTO people = null;
                            try
                            {
                                people = PeopleRepository.GetPeopleByDocumentNumber(user.DocNumber);                                
                                if(people != null)
                                {
                                    if (people.UserID?.ToLower().Trim() != user.Username.ToLower() || people.PeopleEmail?.ToLower().Trim() != user.Email.ToLower())
                                    {
                                        if (people.UserID?.ToLower().Trim() != user.Username.ToLower() && people.PeopleEmail?.ToLower().Trim() != user.Email.ToLower())
                                        {
                                            messagePeople = "Destinatario (Persona): Actualizado.";
                                            log.LogResult = IntegrationAADLogDTO.RESULT_SUCCESS;
                                        }
                                        if (people.UserID?.ToLower().Trim() != user.Username.ToLower() && people.PeopleEmail?.ToLower().Trim() == user.Email.ToLower())
                                        {
                                            messagePeople = "Destinatario (Persona): Alias actualizado.";
                                            log.LogResult = IntegrationAADLogDTO.RESULT_PARCIAL;
                                        }
                                        if (people.UserID?.ToLower().Trim() == user.Username.ToLower() && people.PeopleEmail?.ToLower().Trim() != user.Email.ToLower())
                                        {
                                            messagePeople = "Destinatario (Persona): Correo actualizado.";
                                            log.LogResult = IntegrationAADLogDTO.RESULT_PARCIAL;
                                        }

                                        people.UserID = user.Username;
                                        people.PeopleEmail = user.Email;
                                        PeopleRepository.SavePeople(people, people.PeopleCompany);                                        
                                        log.LogMessage = messagePeople;
                                        IntegrationAADLogRepository.SaveAADLog(log, USER_ID);
                                    }
                                    else
                                    {
                                        messagePeople = "Destinatario (Persona): Sin cambios.";
                                        log.LogMessage = messagePeople;
                                        IntegrationAADLogRepository.SaveAADLog(log, USER_ID);
                                    }
                                }
                                else
                                {
                                    log.LogMessage = $"Destinatario (persona) no encontrada, no cuenta con un registro en ICARUS.";
                                    IntegrationAADLogRepository.SaveAADLog(log, USER_ID);
                                }                                
                            }
                            catch (Exception ex)
                            {
                                log.LogException = ex;
                                log.LogResult = IntegrationAADLogDTO.RESULT_ERROR;
                                log.LogMessage = $"Error al actualizar al destinatario (persona).\n\n";
                                IntegrationAADLogRepository.SaveAADLog(log, USER_ID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"> Integration.Azure [SyncUpdateFromServer] >> Error al actualizar los datos desde Azure. {ex.Message}", ex);
            }
        }
    }
}
