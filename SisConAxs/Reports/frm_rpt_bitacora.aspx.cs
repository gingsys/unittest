using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SisConAxs_DM.Repository;
using SisConAxs_DM.DTO;
using System.Globalization;
using SisConAxs.Session;

namespace SisConAxs.Reports
{
    public partial class frm_rpt_bitacora : System.Web.UI.Page
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
                        CargarReporte();
                    }
                }
            }
        }

        private void ConfigurarReporte()
        {
            string nameReport = "rpt_req_by_auditoria";

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

                //par = new ReportParameter("REQUEST_NUMBER", txtNumReq.Value.Trim());
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_CATEGORY", ddlCategoria.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_CATEGORY_STR_VALUE", ddlCategoria.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_ID", ddlRecurso.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_ID_STR_VALUE", ddlRecurso.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("RESOURCE_FULLNAME", ddlRecurso.SelectedValue);
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });              

                par = new ReportParameter("RESOURCE_ATRIBUTE", ddlAtributo.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("RESOURCE_ATRIBUTE_STR_VALUE", ddlAtributo.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                //par = new ReportParameter("RESOURCE_ID", ddlSistemaInf.SelectedValue);
                //this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUIRED_ACTION", ddlAccRequerida.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUIRED_ACTION_STR_VALUE", ddlAccRequerida.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("STATUS", ddlStatus.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("STATUS_STR_VALUE", ddlStatus.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("TEMPORAL", ddlCtaTemporal.SelectedValue);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("TEMPORAL_STR_VALUE", ddlCtaTemporal.SelectedItem.Text);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("DATE_FILTER", cbFechas.Value);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_DATE_INIT", fecha[0]);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                par = new ReportParameter("REQUEST_DATE_END", fecha[1]);
                this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                if (sessionData.UserRoleSysAdmin == 1)
                {
                    //Enviamos vacio para poder traer información de todas las compañias.
                    par = new ReportParameter("COMPANY_ID", "0");
                    this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                    par = new ReportParameter("COMPANY_NAME","TODOS");
                    this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });
                }
                else
                {
                    par = new ReportParameter("COMPANY_ID", this.sessionData.CompanyID.ToString());
                    this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });

                    par = new ReportParameter("COMPANY_NAME", this.sessionData.CompanyName);
                    this.ReportViewer1.ServerReport.SetParameters(new ReportParameter[] { par });
                }
               


                ReportViewer1.ServerReport.Refresh();

                ReportViewer1.Visible = true;

            }
        }

        private void LimpiarControles()
        {
            //txtNumReq.Value = "";
            ddlCtaTemporal.SelectedIndex = 0;
            ddlAccRequerida.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
        }

        private string[] ConfigurarFechas(string fi, string ff)
        {
            string[] fecha = new string[2];

            DateTime dti, dtf;

            CultureInfo culture = new CultureInfo("es-PE");
            //CultureInfo culture = new CultureInfo("en-US");

            dti = DateTime.ParseExact(fi, "dd/MM/yyyy", culture);
            dtf = DateTime.ParseExact(ff, "dd/MM/yyyy", culture);

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

                //dtm1 = Convert.ToDateTime(fi);
                //dtm2 = Convert.ToDateTime(ff);
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

        protected void imaBtnConsultar_Click(object sender, ImageClickEventArgs e)
        {
            CargarReporte();
        }

        private void cargarCombos()
        {
            CommonValueSetsRepository repo = new CommonValueSetsRepository(this.sessionData);
            //IQueryable<CommonValuesDTO> lista = repo.GetCommonValuesBySet("17");
            IQueryable<CommonValuesDTO> lista;
            List<CommonValuesDTO> data = new List<CommonValuesDTO>();

            //foreach (CommonValuesDTO cv in lista)
            //{
            //    data.Add(cv);
            //}
            data.Insert(0, new CommonValuesDTO() { CommonValueID = 0, CommonValueDisplay = "<TODOS>" });
            data.Insert(1, new CommonValuesDTO() { CommonValueID = 40, CommonValueDisplay = "Pendientes" });
            data.Insert(2, new CommonValuesDTO() { CommonValueID = 43, CommonValueDisplay = "Aprobados" });            
            data.Insert(3, new CommonValuesDTO() { CommonValueID = 44, CommonValueDisplay = "Rechazados" });
            ddlStatus.DataSource = data;
            ddlStatus.DataValueField = "CommonValueID";
            ddlStatus.DataTextField = "CommonValueDisplay";
            ddlStatus.DataBind();

            //
            lista = repo.GetCommonValuesBySet("18");
            data = new List<CommonValuesDTO>();

            foreach (CommonValuesDTO cv in lista)
            {
                data.Add(cv);
            }
            data.Insert(0, new CommonValuesDTO() { CommonValueID = 0, CommonValueDisplay = "<TODOS>" });
            ddlAccRequerida.DataSource = data;
            ddlAccRequerida.DataValueField = "CommonValueID";
            ddlAccRequerida.DataTextField = "CommonValueDisplay";
            ddlAccRequerida.DataBind();

            //
            //AccessResourcesRepository repo1 = new AccessResourcesRepository();
            //IQueryable<AccessResourcesDTO> lista1 = repo1.GetAccessResources(new AccessResourceFilter());
            //List<AccessResourcesDTO> data1 = new List<AccessResourcesDTO>();
            //foreach (AccessResourcesDTO cv in lista1)
            //{
            //    data1.Add(cv);
            //}
            //data1.Insert(0, new AccessResourcesDTO() { ResourceID = 0, ResourceFullName = "<TODOS>" });
            //ddlSistemaInf.DataSource = data1;
            //ddlSistemaInf.DataValueField = "ResourceID";
            //ddlSistemaInf.DataTextField = "ResourceFullName";
            //ddlSistemaInf.DataBind();

            //
            ResourceCategoriesRepository repo2 = new ResourceCategoriesRepository(this.sessionData);
            IQueryable<ResourceCategoriesDTO> lista2 = repo2.GetResourceCategories();
            List<ResourceCategoriesDTO> data2 = new List<ResourceCategoriesDTO>();

            foreach (ResourceCategoriesDTO cv in lista2)
            {
                data2.Add(cv);
            }
            data2.Insert(0, new ResourceCategoriesDTO() { CategoryID = 0, CategoryName = "<TODOS>" });
            ddlCategoria.DataSource = data2;
            ddlCategoria.DataValueField = "CategoryID";
            ddlCategoria.DataTextField = "CategoryName";
            ddlCategoria.DataBind();
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

            //Limpiando
            ddlAtributo.Items.Clear();
            List<AccessTypeValuesDTO> data1 = new List<AccessTypeValuesDTO>();
            data1.Add(new AccessTypeValuesDTO() { AccessTypeID = 0, TypeValueDisplay = "<TODOS>" });
            ddlAtributo.DataSource = data1;
            ddlAtributo.DataValueField = "TypeValueID";
            ddlAtributo.DataTextField = "TypeValueDisplay";
            ddlAtributo.DataBind();
        }

        protected void ddlRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            int resourceID = Convert.ToInt32(ddlRecurso.SelectedValue);
            List<AccessTypeValuesDTO> data = new List<AccessTypeValuesDTO>();
            data.Add(new AccessTypeValuesDTO() { AccessTypeID = 0, TypeValueDisplay = "<TODOS>" });

            AccessResourcesDTO resource = (new AccessResourcesRepository(this.sessionData)).GetAccessResourceById(resourceID).FirstOrDefault();
            if (resource != null)
            {
                AccessTypesDTO accessType = (new AccessTypesRepository(this.sessionData)).GetAccessTypeById(resource.ResourceAccessType).FirstOrDefault();
                if (accessType != null)
                {
                    data.AddRange(accessType.AccessTypeValues.ToList());
                }
            }
            
            ddlAtributo.DataSource = data;
            ddlAtributo.DataValueField = "TypeValueID";
            ddlAtributo.DataTextField = "TypeValueDisplay";
            ddlAtributo.DataBind();
        }
    }
}