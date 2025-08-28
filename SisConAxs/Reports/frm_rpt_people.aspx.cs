using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using SisConAxs.Session;
using SisConAxs_DM.DTO.Filters;

namespace SisConAxs.Reports
{
    public partial class frm_rpt_people : System.Web.UI.Page
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
                fechaInicial();
                cargarCombos();
                
            }
            else
            {
                var eventTarget = this.Request["__EVENTTARGET"] ?? "";
                var eventArgument = this.Request["__EVENTARGUMENT"] ?? "";
                if (eventTarget == "CallServerSideFunctionPostBack")
                {
                    if (eventArgument == "LoadReport")
                    {
                        ConfigurarReporte();
                        CargarReporte();
                    }
                }
            }
        }

        private void ConfigurarReporte()
        {
            string nameReport = "";
            if (ddlTipoReporte.SelectedValue.Equals("0"))
                nameReport = "rpt_req_by_people";
            else
                nameReport = "rpt_req_by_number";

            this.ReportViewer1.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            this.ReportViewer1.ServerReport.ReportPath = @"/" + ConfigurationManager.AppSettings["ReportFolder"].ToString() + "/" + nameReport;
            this.ReportViewer1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServer"]);
            this.ReportViewer1.ServerReport.ReportServerCredentials = (IReportServerCredentials)new CustomReportCredentials(
                                                                ConfigurationManager.AppSettings["ReportUser"],
                                                                ConfigurationManager.AppSettings["ReportPassword"],
                                                                ConfigurationManager.AppSettings["ReportDominio"]
                                                           );

            this.ReportViewer1.ShowParameterPrompts = false;
            this.ReportViewer1.ShowPrintButton = true;
            this.ReportViewer1.ShowRefreshButton = false;
        }

        private void CargarReporte()
        {
            if (ValidarFechas(dpi.Value, dpf.Value))
            {
                string[] fecha = new string[2];
                fecha = ConfigurarFechas(dpi.Value, dpf.Value);

                ReportParameter par;

                par = new ReportParameter("REQUEST_NUMBER", txtNumReq.Value.Trim());
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_CATEGORY", ddlCategoria.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("RESOURCE_CATEGORY_STR_VALUE", ddlCategoria.SelectedItem.Text);
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_ID", ddlRecurso.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("RESOURCE_ID_STR_VALUE", ddlRecurso.SelectedItem.Text);
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("RESOURCE_ATRIBUTE", txtResAtrib.Value.Trim());
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_BY", ddlRequestBy.SelectedValue); //txtReqBy.Value.Trim());
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_TO", ddlRequestTo.SelectedValue); //txtReqTo.Value.Trim());
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_STATUS", ddlStatus.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("REQUEST_STATUS_STR_VALUE", ddlStatus.SelectedItem.Text);
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_DATE_INIT", fecha[0]);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_DATE_END", fecha[1]);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REPORT_TYPE", ddlTipoReporte.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("DATE_FILTER", ddlFiltroFecha.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("COMPANY_ID", this.sessionData.CompanyID.ToString());
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("COMPANY_NAME", this.sessionData.CompanyName);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                ReportViewer1.ServerReport.Refresh();

                ReportViewer1.Visible = true;
            }

        }

        private string[] ConfigurarFechas(string fi, string ff)
        {
            string[] fecha = new string[2];

            DateTime dti, dtf;

            dti = DateTime.ParseExact(fi, "dd/MM/yyyy", null); //Convert.ToDateTime(fi);
            dtf = DateTime.ParseExact(ff, "dd/MM/yyyy", null); //Convert.ToDateTime(ff);

            //fecha[0] = dti.ToString("MM-dd-yyyy HH:mm:ss");
            //fecha[1] = dtf.ToString("MM-dd-yyyy HH:mm:ss");
            fecha[0] = dti.ToString("yyyy-MM-dd");
            fecha[1] = dtf.ToString("yyyy-MM-dd");

            return fecha;
        }

        private bool ValidarFechas(string fi, string ff)
        {
            bool success = false;
            DateTime dtm1, dtm2;

            if (fi == "" && ff == "")
            {
                success = true;
            }
            else if (fi == "" || ff == "")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                       "alert", "alert('fechas ingresadas incorrectas, verifique llenar ambas fechas');", true);
            }
            else
            {

                CultureInfo culture = new CultureInfo("es-PE");
                //CultureInfo culture = new CultureInfo("en-US");

                dtm1 = DateTime.ParseExact(fi, "dd/MM/yyyy", culture);
                dtm2 = DateTime.ParseExact(ff, "dd/MM/yyyy", culture);

                if (System.DateTime.Now.Date == dtm2.Date)
                {
                    dtm2 = System.DateTime.Now;
                }

                if (dtm1 <= dtm2)
                {
                    success = true;
                }

                if (success == false)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                        "alert", "alert('fechas ingresadas incorrectas, la fecha inicial no puede ser mayor a la final');", true);
                }
            }

            return success;
        }

        private void fechaInicial()
        {
            string fi, ff;
            string thisYear = System.DateTime.Now.Year.ToString();
            string thisMont = System.DateTime.Now.Month.ToString();
            string firstDay = "01";

            if (thisMont.Length == 1)
            {
                thisMont = "0" + thisMont;
            }

            fi = firstDay + "/" + thisMont + "/" + thisYear;
            ff = System.DateTime.Now.ToString("dd/MM/yyyy");

            dpi.Value = fi;
            dpf.Value = ff;
        }

        private void LimpiarControles()
        {
            txtNumReq.Value = "";
            //txtReqBy.Value = "";
            //txtReqTo.Value = "";
            //txtResAtrib.Value = "";
            ddlRequestBy.SelectedIndex = 0;
            ddlRequestTo.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            ddlCategoria.SelectedIndex = 0;
        }

        protected void imaBtnConsultar_Click(object sender, ImageClickEventArgs e)
        {
            CargarReporte();
        }

        protected void imaBtnAtras_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ReportViewer1_ReportError(object sender, ReportErrorEventArgs e)
        {
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccessResourcesRepository repo = new AccessResourcesRepository(this.sessionData);
            IQueryable<AccessResourcesDTO> lista = repo.GetAccessResourceByCategoryId(Convert.ToInt32(ddlCategoria.SelectedValue));
            List<AccessResourcesDTO> data = new List<AccessResourcesDTO>();
            foreach (AccessResourcesDTO cv in lista)
            {
                data.Add(cv);
            }
            data.Insert(0, new AccessResourcesDTO() { ResourceID = 0, ResourceFullName = "<TODOS>" });
            ddlRecurso.DataSource = data;
            ddlRecurso.DataValueField = "ResourceID";
            ddlRecurso.DataTextField = "ResourceFullName";
            ddlRecurso.DataBind();

        }

        protected void ddlRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            //AccessTypesRepository repo = new AccessTypesRepository();
            //IQueryable<AccessTypesDTO> lista = repo.GetAccessTypeById(Convert.ToInt32(ddlRecurso.SelectedValue));
            //ICollection<AccessTypeValuesDTO> lista2 = lista.First().AccessTypeValues;

            //List<AccessTypeValuesDTO> data = new List<AccessTypeValuesDTO>();
            //foreach (AccessTypeValuesDTO cv in lista2)
            //{
            //    data.Add(cv);
            //}
            //data.Insert(0, new AccessTypeValuesDTO() { AccessTypeID = 0, TypeValueDisplay = "<TODOS>" });
            //ddlAtributo.DataSource = data;
            //ddlAtributo.DataValueField = "AccessTypeID";
            //ddlAtributo.DataTextField = "TypeValueDisplay";
            //ddlAtributo.DataBind();
        }

        private void cargarCombos()
        {
            // Request Status
            CommonValueSetsRepository cvRepository = new CommonValueSetsRepository(this.sessionData);
            List<CommonValuesDTO> cvList = cvRepository.GetCommonValuesBySet("ESTADOS_SOLICITUD")
                    .Where(x => x.CommonValueName == "S_APROBADO" || x.CommonValueName == "S_RECHAZADO" || x.CommonValueName == "S_PENDIENTE").ToList();
            cvList.Insert(0, new CommonValuesDTO() { CommonValueID = 0, CommonValueDisplay = "<TODOS>" });
            ddlStatus.DataSource = cvList;
            ddlStatus.DataValueField = "CommonValueID";
            ddlStatus.DataTextField = "CommonValueDisplay";
            ddlStatus.DataBind();

            // Resource Category
            ResourceCategoriesRepository repo = new ResourceCategoriesRepository(this.sessionData);
            List<ResourceCategoriesDTO> data = repo.GetResourceCategories().ToList();
            data.Insert(0, new ResourceCategoriesDTO() { CategoryID = 0, CategoryName = "<TODOS>" });
            ddlCategoria.DataSource = data;
            ddlCategoria.DataValueField = "CategoryID";
            ddlCategoria.DataTextField = "CategoryName";
            ddlCategoria.DataBind();

            // Request By
            AccessUsersRepository userRepository = new AccessUsersRepository();
            List<AccessUserDTO> userList = userRepository.GetAccessUsers().ToList();
            userList.Insert(0, new AccessUserDTO() { UserID = 0, UserInternalID = "<TODOS>" });
            ddlRequestBy.DataSource = userList;
            ddlRequestBy.DataValueField = "UserID";
            ddlRequestBy.DataTextField = "UserInternalID";
            ddlRequestBy.DataBind();

            // Request To
            PeopleRepository peopleRepository = new PeopleRepository(this.sessionData);
            List<PeopleDTO> peopleList = peopleRepository.GetPeople(new PeopleFilter() { status=-1 }).ToList();
            peopleList.Insert(0, new PeopleDTO() { PeopleID = 0, PeopleLastName = "<TODOS>" });
            ddlRequestTo.DataSource = peopleList;
            ddlRequestTo.DataValueField = "PeopleID";
            ddlRequestTo.DataTextField = "PeopleFullname";
            ddlRequestTo.DataBind();
        }
    }

}