using System;
using System.DirectoryServices;

namespace GyM.Security
{
    public class Authentication
    {
        private string _filterAttribute;
        private string _Path = String.Empty;
        private string _Domain = "gym.com.pe";
        private string _UserName;
        private string _Password;
        public string Domain
        {
            get { return this._Domain; }
            set { this._Domain = value; }
        }
        public string UserName
        {
            get { return this._UserName; }
            set { this._UserName = value; }
        }
        public string Password
        {
            get { return this._Password; }
            set { this._Password = value; }
        }


        public Authentication()
        {
        }

        public Authentication(string Domain, string UserName, string Password)
        {
            this._Domain = Domain;
            this._UserName = UserName;
            this._Password = Password;
        }

        public bool IsAuthenticated(string Domain, string UserName, string Password)
        {
            this._Path = Utils.GetPath(Domain);
            string username = String.Format(@"{0}\{1}", Domain, UserName);
            DirectoryEntry directoryEntry = new DirectoryEntry(this._Path, username, Password);
            try
            {
                object arg_2E_0 = directoryEntry.NativeObject;
                SearchResult searchResult = new DirectorySearcher(directoryEntry)
                {
                    Filter = String.Format("(SAMAccountName={0})", UserName),
                    PropertiesToLoad = { "cn" }
                }.FindOne();
                if (searchResult == null)
                {
                    return false;
                }
                this._Path = searchResult.Path;
                this._filterAttribute = searchResult.Properties["cn"][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error de inicio de sesión: nombre de usuario desconocido o contraseña incorrecta.", ex);
            }
            return true;
        }

        public bool IsAuthenticated()
        {
            return this.IsAuthenticated(this._Domain, this._UserName, this._Password);
        }

        public bool IsAuthenticatedNetwork(string UserName, string Domain)
        {
            string[] array = UserName.Split(new char[] { '\\' });
            return array[0].ToUpper().Equals(Domain.ToUpper().Split(new char[] { '.' })[0]);
        }
    }
}