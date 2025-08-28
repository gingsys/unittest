using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SisConAxs
{
    public partial class index : System.Web.UI.Page
    {
        public bool isRelease = false;
        protected void Page_Load(object sender, EventArgs e)
        {
        #if RELEASE
            isRelease = true;
        #endif
        }
    }
}