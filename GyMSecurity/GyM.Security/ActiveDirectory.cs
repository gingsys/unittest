using System;
using System.Data;
using System.DirectoryServices;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;

namespace GyM.Security
{
    public class ActiveDirectory
    {
        public enum Filter
        {
            Nombre,
            Alias,
            Company,
            Department
        }

        private string[] Filters = new string[]
		{
			"name",
			"samAccountName",
            "company",
            "department"
		};

        private DataTable GetUserTable()
        {
            return new DataTable
            {
                Columns = 
				{
                    "givenName",
                    "sn",
					"displayname",
					"title",
					"telephoneNumber",
					"mail",
					"company",
					"samAccountName",
					"department",
                    "postalCode",
                    "mobile",
                    "userAccountControl",
                    "userAccountControl$AccountDisabled",
                    "userAccountControl$Lockout",
                    "userAccountControl$PasswordExpired",

                    "objectCategory",
                    "objectClass",

                    "userPrincipalName"
                }
            };
        }

        private void AddUser(DirectoryEntry de, ref DataTable dtUsuarios)
        {
            // validate if row have properties
            //bool have = false;
            //try { have = de.Properties.Count > 0; }
            //catch (Exception ex) { }
            //if (!have) return;

            string givenName = "";
            string sn = "";
            string displayname = "";
            string title = "";
            string telephoneNumber = "";
            string mail = "";
            string company = "";
            string sAMAccountName = "";
            string department = "";
            string postalCode = "";
            string mobile = "";
            int userAccountControl = 0;
            string userPrincipalName = "";

            string objectCategory = "";
            string objectClass = "";

            if (de.Properties.Contains("givenName"))
                givenName = de.Properties["givenName"].Value.ToString().Trim();
            if (de.Properties.Contains("sn"))
                sn = de.Properties["sn"].Value.ToString().Trim();
            if (de.Properties.Contains("displayname"))
                displayname = de.Properties["displayname"].Value.ToString().Trim();
            if (de.Properties.Contains("title"))
                title = de.Properties["title"].Value.ToString().Trim();
            if (de.Properties.Contains("telephoneNumber"))
                telephoneNumber = de.Properties["telephoneNumber"].Value.ToString().Trim();
            if (de.Properties.Contains("mail"))
                mail = de.Properties["mail"].Value.ToString().Trim();
            if (de.Properties.Contains("company"))
                company = de.Properties["company"].Value.ToString().Trim();
            if (de.Properties.Contains("sAMAccountName"))
                sAMAccountName = de.Properties["sAMAccountName"].Value.ToString().Trim();
            if (de.Properties.Contains("department"))
                department = de.Properties["department"].Value.ToString().Trim();
            if (de.Properties.Contains("postalCode"))
                postalCode = de.Properties["postalCode"].Value.ToString().Trim();
            if (de.Properties.Contains("mobile"))
                mobile = de.Properties["mobile"].Value.ToString().Trim();
            if (de.Properties.Contains("userAccountControl"))
                userAccountControl = (int)de.Properties["userAccountControl"].Value;

            if (de.Properties.Contains("objectCategory"))
                objectCategory = de.Properties["objectCategory"].Value.ToString().Trim();
            if (de.Properties.Contains("objectClass"))
                objectClass = de.Properties["objectClass"].Value.ToString().Trim();

            if (de.Properties.Contains("userPrincipalName"))
                userPrincipalName = de.Properties["userPrincipalName"].Value.ToString();

            if (!String.IsNullOrEmpty(sAMAccountName) && !String.IsNullOrEmpty(displayname))
            {
                DataRow dataRow = dtUsuarios.NewRow();
                dataRow["givenName"] = givenName;
                dataRow["sn"] = sn;
                dataRow["displayname"] = displayname;
                dataRow["title"] = title;
                dataRow["telephoneNumber"] = telephoneNumber;
                dataRow["mail"] = mail;
                dataRow["company"] = company;
                dataRow["samAccountName"] = sAMAccountName;
                dataRow["department"] = department;
                dataRow["postalCode"] = postalCode;
                dataRow["mobile"] = mobile;
                dataRow["userAccountControl"] = userAccountControl;
                dataRow["userAccountControl$AccountDisabled"] = (userAccountControl & 2) == 2;
                dataRow["userAccountControl$Lockout"] = (userAccountControl & (1 << 4)) == (1 << 4);
                dataRow["userAccountControl$PasswordExpired"] = (userAccountControl & (1 << 23)) == (1 << 23);

                dataRow["objectCategory"] = objectCategory;
                dataRow["objectClass"] = objectClass;

                dataRow["userPrincipalName"] = userPrincipalName;

                dtUsuarios.Rows.Add(dataRow);
            }
        }

        private string GetFilter(string objectCategory, string filter, ActiveDirectory.Filter filterUser, string AdditionalFilters = "")
        {
            return String.Format("(&(objectCategory={0})({1}={2}){3})",
                                 objectCategory,
                                 this.Filters[Convert.ToInt32(filterUser)],
                                 filter,
                                 AdditionalFilters
                   );
        }

        private string GetFilterString(string user, ActiveDirectory.Filter filterUser, string AdditionalFilters = "")
        {
            string text = this.GetFilter("user", user, filterUser, AdditionalFilters);
            return "(|" + text + ")";
        }

        public DataTable GetUsers(string Domain, ActiveDirectory.Filter filterUser, string filterUserCond, string user = null, string password = null, string AdditionalFilters = "")
        {
            string path = Utils.GetPath(Domain);

            var currentForest = Forest.GetCurrentForest();
            var gc = currentForest.FindGlobalCatalog();

            var userSearcher = gc.GetDirectorySearcher();
            userSearcher.Filter = this.GetFilterString(filterUserCond, filterUser, AdditionalFilters); //AdditionalFilters == null ? "" : AdditionalFilters.Trim(); //
            userSearcher.PropertyNamesOnly = true;
            userSearcher.PropertiesToLoad.Add("name");
            userSearcher.PageSize = 1001;  // *fix SizeLimit 1000

            SearchResultCollection searchResultCollection = userSearcher.FindAll();

            DataTable userTable = this.GetUserTable();
            foreach (SearchResult searchResult in searchResultCollection)
            {
                this.AddUser(searchResult.GetDirectoryEntry(), ref userTable);
            }
            return userTable;
        }
    }
}
