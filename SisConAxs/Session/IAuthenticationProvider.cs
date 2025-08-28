using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs.Session
{
    public interface IAuthenticationProvider
    {
        bool Validate(string username, string credentials);
        string GetLoginUsername(string username);
    }
}
