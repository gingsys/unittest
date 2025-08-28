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
    public partial class frm_rpt_integration_aad_log : System.Web.UI.Page
    {
        private SessionData sessionData = new SessionData();
        protected void Page_Load(object sender, EventArgs e)
        {
            var session = SessionManager.GetSessionByToken(Request.QueryString["token"]);
            sessionData = session.getSessionData();

            if (!Page.IsPostBack)
            {
                LimpiarControles();
                ConfigurarReporte();
                //fechaInicial();
                //cargarCombos();
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


        //private void cargarCombos()
        //{
        //    CompanyRepository repo = new CompanyRepository();
        //    List<CompanyDTO> data = repo.GetCompanies().ToList();
        //    data.Insert(0, new CompanyDTO() { CompanyID = 0, CompanyDisplay = "<TODOS>" });
        //    ddlEmpresas.DataSource = data;
        //    ddlEmpresas.DataValueField = "CompanyID";
        //    ddlEmpresas.DataTextField = "CompanyDisplay";
        //    ddlEmpresas.DataBind();

        //}

        private void ConfigurarReporte()
        {
            string nameReport = "rpt_integration_aad_log";

            this.ReportViewer01.ProcessingMode = ProcessingMode.Remote;
            this.ReportViewer01.ServerReport.ReportPath = @"/" + ConfigurationManager.AppSettings["ReportFolder"].ToString() + "/" + nameReport;
            this.ReportViewer01.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServer"]);
            this.ReportViewer01.ServerReport.ReportServerCredentials = (IReportServerCredentials)new CustomReportCredentials(
                                                                ConfigurationManager.AppSettings["ReportUser"],
                                                                ConfigurationManager.AppSettings["ReportPassword"],
                                                                ConfigurationManager.AppSettings["ReportDominio"]
                                                           );

            this.ReportViewer01.ShowParameterPrompts = false;
            this.ReportViewer01.ShowPrintButton = true;
            this.ReportViewer01.ShowRefreshButton = false;
        }

        private void CargarReporte()
        {
            if (ValidateDates(dpStart.Value, dpEnd.Value))
            {
                //string[] typeDate = new string[2];
                //typeDate = GetDates(dpTypeStart.Value, dpTypeEnd.Value);
                string[] date = new string[2];
                date = GetDates(dpStart.Value, dpEnd.Value);

                ReportParameter par;
                par = new ReportParameter("Result", ddlResult.SelectedValue == "" ? null : ddlResult.SelectedValue);
                this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });
                //par = new ReportParameter("Type", ddlType.SelectedValue == "" ? null : ddlType.SelectedValue);
                //this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });
                //par = new ReportParameter("TypeDateStart", typeDate[0]);
                //this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });
                //par = new ReportParameter("TypeDateEnd", typeDate[1]);
                //this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });
                par = new ReportParameter("DateStart", date[0]);
                this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });
                par = new ReportParameter("DateEnd", date[1]);
                this.ReportViewer01.ServerReport.SetParameters(new ReportParameter[] { par });

                ReportViewer01.ServerReport.Refresh();
                ReportViewer01.Visible = true;
            }
        }

        private void LimpiarControles()
        {

        }

        private string[] GetDates(string dateStart, string dateEnd)
        {
            string[] date = new string[2];
            DateTime dtStart, dtEnd;

            CultureInfo culture = new CultureInfo("es-PE");
            
            date[0] = null;
            if(!String.IsNullOrWhiteSpace(dateStart))
            {
                dtStart = DateTime.ParseExact(dateStart, "dd/MM/yyyy", culture);
                date[0] = dtStart.ToString("yyyy-MM-dd");
            }

            date[1] = null;
            if (!String.IsNullOrWhiteSpace(dateEnd))
            {
                dtEnd = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", culture);
                date[1] = dtEnd.ToString("yyyy-MM-dd");
            }
            return date;
        }

        private bool ValidateDates(string dateStart, string dateEnd)
        {
            bool success = false;
            DateTime dtm1, dtm2;

            if (dateStart == "" && dateEnd == "")
            {
                success = true;
            }   
            else if (dateStart == "" || dateEnd == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                       "alert", "alert('fechas ingresadas incorrectas, verifique llenar ambas fechas');", true);
            }   
            else
            {
                CultureInfo culture = new CultureInfo("es-PE");
                dtm1 = DateTime.ParseExact(dateStart, "dd/MM/yyyy", culture);
                dtm2 = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", culture);

                if (DateTime.Now.Date == dtm2.Date)
                    dtm2 = DateTime.Now;

                if (dtm1 <= dtm2)
                    success = true;

                if (success == false)
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                        "alert", "alert('fechas ingresadas incorrectas, la fecha inicial no puede ser mayor a la final');", true);
            }

            return success;
        }

        private void fechaInicial()
        {
            //string fi, ff;
            //string thisYear = System.DateTime.Now.Year.ToString();
            //string thisMont = System.DateTime.Now.Month.ToString();
            //string firstDay = "01";

            //if (thisMont.Length == 1)
            //    thisMont = "0" + thisMont;

            //fi = firstDay + "/" + thisMont + "/" + thisYear;
            //ff = DateTime.Now.ToString("dd/MM/yyyy");

            //dpi.Value = fi;
            //dpf.Value = ff;
        }


        protected void imaBtnConsultar_Click(object sender, ImageClickEventArgs e)
        {
            CargarReporte();
        }
    }



}