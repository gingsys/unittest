using Microsoft.Reporting.WebForms;
using SisConAxs.Session;
using SisConAxs_DM.DTO;
using SisConAxs_DM.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SisConAxs.Reports
{
    public partial class frm_rpt_wfHiearchy : System.Web.UI.Page
    {
        private SessionData sessionData = new SessionData();

        protected void Page_Load(object sender, EventArgs e)
        {
            var session = SessionManager.GetSessionByToken(Request.QueryString["token"]);
            sessionData = session.getSessionData();
            //sessionData.CompanyID = Convert.ToInt32(Request.QueryString["company"]);
            if (!Page.IsPostBack)
            {
                LimpiarControles();
                ConfigurarReporte();
                CargarCombos();
                //fechaInicial();
            }
            else
            {
                var eventTarget = this.Request["__EVENTTARGET"] ?? "";
                var eventArgument = this.Request["__EVENTARGUMENT"] ?? "";
                if (eventTarget == "CallServerSideFunctionPostBack")
                {
                    if (eventArgument == "LoadReport")
                    {
                        CargarReporte();
                    }
                }
            }
        }

        private void ConfigurarReporte()
        {
            string nameReport = "rpt_wf_hierarchy_history";

            this.ReportViewerWf.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            this.ReportViewerWf.ServerReport.ReportPath = @"/" + ConfigurationManager.AppSettings["ReportFolder"].ToString() + "/" + nameReport;
            this.ReportViewerWf.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServer"]);
            this.ReportViewerWf.ServerReport.ReportServerCredentials = (IReportServerCredentials)new CustomReportCredentials(
                                                                ConfigurationManager.AppSettings["ReportUser"],
                                                                ConfigurationManager.AppSettings["ReportPassword"],
                                                                ConfigurationManager.AppSettings["ReportDominio"]
                                                           );

            this.ReportViewerWf.ShowParameterPrompts = false;
            this.ReportViewerWf.ShowPrintButton = true;
            this.ReportViewerWf.ShowRefreshButton = false;
        }

        private void CargarReporte()
        {
            //if (ValidarFechas(dpi.Value, dpf.Value))
            //{
                //string[] fecha = new string[2];
                //fecha = ConfigurarFechas(dpi.Value, dpf.Value);

                ReportParameter par;

                par = new ReportParameter("EDIT_USER", string.IsNullOrEmpty(txtEditUser.Value) ? null : txtEditUser.Value);
                this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("APPROVER_USER", string.IsNullOrEmpty(txtUserAprobador.Value) ? null : txtUserAprobador.Value);
                this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("HIERARCHY_NAME", string.IsNullOrEmpty(txtJerarquia.Value) ? null : txtJerarquia.Value);
                this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

            //par = new ReportParameter("REQUEST_DATE_INIT", fecha[0]);
            //this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

            //par = new ReportParameter("REQUEST_DATE_END", fecha[1]);
            //this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

            // Empresas --------------------------------------------------------------------- //
            var listEmpresas = new List<string>();
                listEmpresas.Add("0");
                for (int i = 0; i < chkEmpresas.Items.Count; i++)
                {
                    if (chkEmpresas.Items[i].Selected == true)
                    {
                        listEmpresas.Add(chkEmpresas.Items[i].Value);
                    }
                }
                string empresas = String.Join(",", listEmpresas);
                par = new ReportParameter("COMPANY_ID", empresas);
                this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("COMPANY_NAME", this.sessionData.CompanyName);
                this.ReportViewerWf.ServerReport.SetParameters(new ReportParameter[] { par });

                ReportViewerWf.ServerReport.Refresh();
                ReportViewerWf.Visible = true;

            //}
        }

        private void LimpiarControles()
        {
            txtEditUser.Value = "";
            txtUserAprobador.Value = "";
            txtJerarquia.Value = "";
        }

        //private string[] ConfigurarFechas(string fi, string ff)
        //{
        //    string[] date = new string[2];
        //    DateTime dtStart, dtEnd;

        //    CultureInfo culture = new CultureInfo("es-PE");

        //    date[0] = null;
        //    if (!String.IsNullOrWhiteSpace(fi))
        //    {
        //        dtStart = DateTime.ParseExact(fi, "dd/MM/yyyy", culture);
        //        date[0] = dtStart.ToString("yyyy-MM-dd");
        //    }

        //    date[1] = null;
        //    if (!String.IsNullOrWhiteSpace(ff))
        //    {
        //        dtEnd = DateTime.ParseExact(ff, "dd/MM/yyyy", culture);
        //        date[1] = dtEnd.ToString("yyyy-MM-dd");
        //    }
        //    return date;
        //}

        //private bool ValidarFechas(string fi, string ff)
        //{
        //    bool success = false;
        //    DateTime dtm1, dtm2;

        //    if (fi == "" && ff == "")
        //    {
        //        success = true;
        //    }
        //    else if (fi == "" || ff == "")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
        //               "alert", "alert('fechas ingresadas incorrectas, verifique llenar ambas fechas');", true);
        //    }
        //    else
        //    {

        //        //dtm1 = Convert.ToDateTime(fi);
        //        //dtm2 = Convert.ToDateTime(ff);
        //        CultureInfo culture = new CultureInfo("es-PE");
        //        //CultureInfo culture = new CultureInfo("en-US");

        //        dtm1 = DateTime.ParseExact(fi, "dd/MM/yyyy", culture);
        //        dtm2 = DateTime.ParseExact(ff, "dd/MM/yyyy", culture);

        //        if (System.DateTime.Now.Date == dtm2.Date)
        //        {
        //            dtm2 = System.DateTime.Now;
        //        }

        //        if (dtm1 <= dtm2)
        //        {
        //            success = true;
        //        }

        //        if (success == false)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
        //                "alert", "alert('fechas ingresadas incorrectas, la fecha inicial no puede ser mayor a la final');", true);
        //        }
        //    }

        //    return success;
        //}

        protected void imaBtnConsultar_Click(object sender, ImageClickEventArgs e)
        {
            CargarReporte();
        }

        private void CargarCombos()
        {
            // Companies
            CompanyRepository cRepository = new CompanyRepository();
            int roleSysAdmin = this.sessionData.UserRoleSysAdmin;

            List<CompanyDTO> cList = roleSysAdmin == 1 ? 
                cRepository.GetCompanies().Where(c => c.CompanyActive == 1).ToList() : 
                cRepository.GetCompanies().Where(c => c.CompanyID == this.sessionData.CompanyID && c.CompanyActive == 1).ToList();

            chkEmpresas.DataSource = cList;
            chkEmpresas.DataValueField = "CompanyID";
            chkEmpresas.DataTextField = "CompanyDisplay";
            chkEmpresas.DataBind();
            for (int i = 0; i < chkEmpresas.Items.Count; i++)
            {
                chkEmpresas.Items[i].Selected = true;
            }
            chkEmpresas.Enabled = false;
            chkTodasEmpresas.Checked = true;
        }

        protected void chkTodasEmpresas_CheckedChanged(object sender, EventArgs e)
        {
            this.chkEmpresas.Enabled = !this.chkTodasEmpresas.Checked;
            for (int i = 0; i < chkEmpresas.Items.Count; i++)
            {
                chkEmpresas.Items[i].Selected = this.chkTodasEmpresas.Checked;
            }
        }
    }
}