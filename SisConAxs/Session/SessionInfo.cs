using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SisConAxs.Session
{
    public class SessionInfo
    {
        public string SessionToken;
        public DateTime SessionStart;
        public AccessUsers SessionUser;
        public int SessionCompany;

        public SessionInfo(AccessUsers user, int companyID = -1)
        {
            SessionStart = DateTime.Now;
            SessionUser = user;
            SessionCompany = companyID;
        }

        public void GenerateToken()
        {
            SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
            Random rnd = new Random();
            string tokenBase = SessionUser.UserInternalID + SessionStart.ToString("yyyyMMddhhmmssfffff") + String.Format("{0:00000}", rnd.Next(10000, 99999));

            byte[] textBytes = Encoding.UTF8.GetBytes(tokenBase);
            byte[] hashBytes = sha.ComputeHash(textBytes);

            SessionToken = Convert.ToBase64String(hashBytes);
        }

        public SessionData getSessionData()
        {
            SessionData sessionData = new SessionData();
            sessionData.sessionToken = SessionToken;
            sessionData.sessionUser = SessionUser.UserInternalID;
            sessionData.sessionUserFullName = SessionUser.UserFirstName + " " + SessionUser.UserLastName;
            sessionData.UserStatus = SessionUser.UserStatus;

            sessionData.User = new AccessUsersRepository().GetAccessUserById(this.SessionUser.UserID);
            sessionData.UserRoleSysAdmin = sessionData.User.UserRoleSysAdmin;

            AccessUserCompanyDTO userCompany = sessionData.User.UserCompanies.FirstOrDefault(x => x.CompanyID == SessionCompany);
            if (userCompany == null && sessionData.UserRoleSysAdmin == 0)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Forbidden);   //Unauthorized
                message.Content = new StringContent("El usuario no tiene permiso para acceder a esta empresa.");
                throw new HttpResponseException(message);
            }
            if (userCompany != null)
            {
                sessionData.CompanyID = userCompany.CompanyID;
                sessionData.CompanyDisplay = userCompany.CompanyDisplay;
                sessionData.CompanyName = userCompany.CompanyName;
                sessionData.UserRole1 = userCompany.UserRole1;
                sessionData.UserRole2 = userCompany.UserRole2;
                sessionData.UserRole3 = userCompany.UserRole3;
                sessionData.UserRole4 = userCompany.UserRole4;
                sessionData.UserRole5 = userCompany.UserRole5;
                sessionData.UserRole6 = userCompany.UserRole6;
                sessionData.UserRole7 = userCompany.UserRole7;
            }
            return sessionData;
        }

        public void SetCompany(int companyID = -1)
        {
            var user = new AccessUsersRepository().GetAccessUserById(this.SessionUser.UserID);
            AccessUserCompanyDTO company = null;
            if (companyID < 1)
            {
                company = user.UserCompanies.FirstOrDefault();
            }
            else
            {
                company = user.UserCompanies.FirstOrDefault(x => x.CompanyID == companyID);
            }
            if (company == null)
            {
                if (user.UserRoleSysAdmin == 0)
                {
                    HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.Forbidden);   //Unauthorized
                    message.Content = new StringContent("El usuario no tiene permiso para acceder a esta empresa.");
                    throw new HttpResponseException(message);
                }
                else
                {
                    SessionCompany = -1;
                }
            }
            else
            {
                SessionCompany = company.CompanyID;
            }
        }
    }
}