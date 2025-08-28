using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GyM.Security
{
    class Utils
    {
        static public string GetPath(string domain)
        {
            if (!String.IsNullOrEmpty(domain))
            {
                domain = domain.Trim();
                Regex regExp = new Regex(@"([a-z0-9]+).*?(?:[\.a-z0-9]+)?\b", RegexOptions.IgnoreCase);
                Match match = regExp.Match(domain);
                if (match.Success && match.Groups.Count >= 2)
                {
                    string[] array = domain.Split('.');
                    string domainAD = array[0];
                    for (int i = 0; i < array.Length; i++)
                        array[i] = "DC=" + array[i];
                    return String.Format("LDAP://{0}/{1}", domainAD, String.Join(",", array));
                }
            }
            return null;
        }
    }
}