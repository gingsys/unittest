using SisConAxs_DM.DTO;
using SisConAxs_DM.Models;
using SisConAxs_DM.Repository;
using SisConAxs_DM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SisConAxs_DM.Utils
{
    public class RequestEmailData
    {
        public string Key { get; set; } = null;

        public WorkflowItemsDTO WorkflowItem { get; set; } = new WorkflowItemsDTO();
        public int RequestID { get; set; }
        public int WorkflowID { get; set; }
        public int WorkflowStep { get; set; }
        public string DestMail { get; set; }
        public string DestMailCc { get; set; } //Adicionado el 20191004 <-- DIANA CAMUS
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public string DestNames { get; set; } = "";
        public bool SendAccountMailer { get; set; }
        public int Type { get; set; }
        public List<int> ExecutionsID { get; set; } = new List<int>();

        public int AttemptCount { get; set; } = 1;
        public DateTime? AttemptDate { get; set; } = null;
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public RequestEmailData()
        {
        }

        static public RequestEmailData PrepareEmailData(WorkflowExecution wfExec, string defaultSubject = "", string emailDest = "", bool sendAccountMailer = false, string destEmailCc = "", string body = "") //Modificado el 20191004 <-- DIANA CAMUS
        {
            People approver = new People();
            if (string.IsNullOrWhiteSpace(emailDest))
            {
                SisConAxsContext db = new SisConAxsContext();
                int? approverID = new WorkflowExecutionRepository().GetWfExecIntParam(wfExec.WfExecID, "approver");
                approver = db.People.FirstOrDefault(p => p.PeopleID == approverID && p.PeopleStatus == 1);
                if (approver == null || string.IsNullOrEmpty(approver.PeopleEmail)) { return null; }
            }

            RequestEmailData mail = new RequestEmailData();
            AutoMapper.Mapper.Map<WorkflowItems, WorkflowItemsDTO>(wfExec.WorkflowItem, mail.WorkflowItem);
            mail.RequestID = (int)wfExec.WfExecParentObjectID;
            mail.WorkflowID = wfExec.WfExecWfID;
            mail.WorkflowStep = wfExec.WorkflowItem.WfItemStep;
            mail.Subject = defaultSubject;
            mail.Body = body;
            mail.DestMail = string.IsNullOrWhiteSpace(emailDest) ? approver.PeopleEmail : emailDest;
            mail.DestMailCc = destEmailCc; //Adicionado el 20191004 <-- DIANA CAMUS
            mail.DestNames = string.IsNullOrWhiteSpace(emailDest) ? approver.PeopleFirstName + " " + approver.PeopleLastName : "";
            mail.Type = 1; // Tipo Consulta   //wfExec.WorkflowItems.WfItemType;
            mail.SendAccountMailer = sendAccountMailer;
            return mail;
        }

        public NotificationMailer BuildMailer()
        {
            NotificationMailer mailer = new NotificationMailer();
            mailer.Subject = this.Subject + this.FormatVariables(this.WorkflowItem.WfItemSubject ?? "");
            mailer.AddAttachment("Content/images/logo_full.png", "logo0001");

            string body = !String.IsNullOrWhiteSpace(this.Body) ? this.Body : this.WorkflowItem.WfItemMessage;
            body = String.Format("<img alt='ICARUS' src='cid:{0}' style='height:100px; width:806px; margin-bottom:0px; margin-left:0px; margin-right:0px; margin-top:0px' /><hr/>", "logo0001") + body;
            mailer.Body = this.FormatVariables(body);
            mailer.SendTo = this.DestMail;
            mailer.SendCc = this.DestMailCc; //Adicionado el 20191004 <-- DIANA CAMUS
            mailer.SendAccountMailer = this.SendAccountMailer;
            return mailer;
        }


        public string GetKey()
        {
            if (Key == null)
            {
                Key = "";
                Key += String.Format("{0:00000}", RequestID);
                Key += String.Format("{0:00000}", WorkflowID);
                Key += String.Format("{0:00000}", WorkflowStep);
                Key += String.Format("{0:00000}", Type);
                Key += "-" + String.Format("{0}", DestMail) + "-";
            }
            return Key;
        }

        /// <summary>
        /// Procesa un texto, reemplazando las variables de Objeto o parámetros de ejecución con sus respecivos valores.
        /// </summary>
        /// <param name="wfExec">Objeto Workflow Execution que contiene la información del Objeto</param>
        /// <param name="aText">Texto a transformar.</param>
        /// <returns></returns>
        public string FormatVariables(string aText) //WorkflowExecution wfExec, List<int> executions, string aText)
        {
            string proccesedText = aText;
            SisConAxsContext db = new SisConAxsContext();
            AccessRequests request = db.AccessRequests.FirstOrDefault(t => t.RequestID == this.RequestID); //wfExec.WfExecParentObjectID);
            Company company = db.Companies.FirstOrDefault(t => t.CompanyID == request.RequestCompany);
            var country = db.CommonValues.FirstOrDefault(t => t.CommonValueID == company.CompanyCountry);

            var webHostConf = new SystemConfigRepository().GetWebHostConfig();

            if (request != null)
            {
                string url = String.Format("{0}/#/request/forapprove/{1}",
                                            webHostConf.NotifConfHost.Trim(),
                                            request.RequestID);

                proccesedText = proccesedText.Replace("[[solicitud_numero]]", request.RequestNumber.ToString("00000000"));
                proccesedText = proccesedText.Replace("[[solicitud_fecha]]", ((DateTime)request.RequestDate).ToString("dd/MM/yyyy hh:mm:ss"));
                proccesedText = proccesedText.Replace("[[solicitud_solicitante]]", request.RequestBy);

                proccesedText = proccesedText.Replace("[[solicitud_para]]", request.PeopleRequestTo.GetFullName());
                proccesedText = proccesedText.Replace("[[solicitud_para_fecha_ingreso]]", request.PeopleRequestTo.PeopleStartDate?.ToString("dd/MM/yyyy"));
                proccesedText = proccesedText.Replace("[[solicitud_para_documento_identidad]]", request.PeopleRequestTo.PeopleDocNum);
                proccesedText = proccesedText.Replace("[[solicitud_para_cargo]]", request.RequestToPosition);

                proccesedText = proccesedText.Replace("[[solicitud_observacion]]", request.RequestNote);
                proccesedText = proccesedText.Replace("[[solicitud_aprobacion_enlace]]", String.Format("<a href='" + url + "'>" + url + "</a>", "forapprove"));
                proccesedText = proccesedText.Replace("[[solicitud_enlace]]", String.Format("<a href='" + url + "'>" + url + "</a>", "send"));
                proccesedText = proccesedText.Replace("[[solicitud_tipo]]", request.ReqType.CommonValueDisplay);
                proccesedText = proccesedText.Replace("[[archivo_adjunto]]", RequestEmailVariablesFormatter.FormatAttached(request.RequestAttached));
                proccesedText = proccesedText.Replace("[[solicitud_ticket]]", request.AttentionTicket);
                proccesedText = proccesedText.Replace("[[solicitud_usuario_oracle]]", request.OracleUser);
                proccesedText = proccesedText.Replace("[[solicitud_menu_oracle]]", request.OracleMenu);
                proccesedText = proccesedText.Replace("[[solicitud_empresa]]", company.CompanyName);                
                proccesedText = proccesedText.Replace("[[solicitud_pais]]", country.CommonValueDisplay);

                if (proccesedText.IndexOf("[[solicitud_detalle]]") > -1)
                    proccesedText = proccesedText.Replace("[[solicitud_detalle]]", RequestEmailVariablesFormatter.FormatRequestItems(request, this.ExecutionsID));
                if (proccesedText.IndexOf("[[solicitud_detalle_aprobados]]") > -1)
                    proccesedText = proccesedText.Replace("[[solicitud_detalle_aprobados]]", RequestEmailVariablesFormatter.FormatRequestItems(request, this.ExecutionsID, 1));
                //if (proccesedText.IndexOf("[[solicitud_items_rechazados]]") > -1)
                //    proccesedText = proccesedText.Replace("[[solicitud_items_rechazados]]", GetRequestItems(request, executions, 0));
                if (proccesedText.IndexOf("[[solicitud_historia]]") > -1)
                    proccesedText = proccesedText.Replace("[[solicitud_historia]]", RequestEmailVariablesFormatter.FormatHistory(request, this.ExecutionsID));
            }
            return proccesedText;
        }
    }


    class RequestEmailTreeResponse
    {
        public string name { get; set; }
        public string category { get; set; }
        public AccessRequestDetails accessRequestDetail { get; set; }
        public WorkflowExecution workflowExecution { get; set; }
        public RequestEmailTreeResponse parent { get; set; }
        public RequestEmailTreeResponse rootParent
        {
            get
            {
                RequestEmailTreeResponse p = this;
                while (p.parent != null)
                {
                    p = p.parent;
                }
                return p;
            }
        }
        public List<RequestEmailTreeResponse> children { get; set; }

        public RequestEmailTreeResponse(RequestEmailTreeResponse aparent = null)
        {
            this.parent = aparent;
            this.children = new List<RequestEmailTreeResponse>();
        }

        private List<WorkflowExecution> getExecutions()
        {
            List<WorkflowExecution> list = new List<WorkflowExecution>();
            if (this.workflowExecution != null)
                list.Add(this.workflowExecution);
            foreach (RequestEmailTreeResponse item in this.children)
            {
                list.AddRange(item.getExecutions());
            }
            return list;
        }

        public List<int> getListExecutionID()
        {
            List<int> list = getExecutions().Select(x => x.WfExecID).ToList();
            return list;
        }

        public string getTreeHtml()
        {
            string values = "";
            string type = "";
            string colorType = "inherit";
            if (accessRequestDetail != null)
            {
                values = RequestEmailVariablesFormatter.FormatRequestDetailDisplayValue(accessRequestDetail.RequestDetDisplayValue);
                if (accessRequestDetail.RequestDetAdditional > 0)
                    values += ": " + accessRequestDetail.RequestDetAdditionalStrValue;
                if (!String.IsNullOrEmpty(values))
                    values = "[" + values + "]";

                if (accessRequestDetail.RequestDetType == AccessRequestDTO.TYPE_ALTA)
                    colorType = "#009900";
                else if (accessRequestDetail.RequestDetType == AccessRequestDTO.TYPE_BAJA)
                    colorType = "#C80000";
                else if (accessRequestDetail.RequestDetType == AccessRequestDTO.TYPE_MODIFICACION)
                    colorType = "#FFA500";

                type = String.Format(" - <span style='color:{0}'><strong>{1}</strong></span>",
                                    colorType,
                                    accessRequestDetail.CommonValuesType.CommonValueDisplay.ToUpper());
            }

            string html = $"<li>{name} {values} {type}";
            if (this.children.Count > 0)
            {
                html += "<ul>";
                foreach (var child in this.children)
                    html += child.getTreeHtml();
                html += "</ul>";
            }
            html += "</li>";

            return html;
        }
    }

    class RequestEmailVariablesFormatter
    {
        public static string FormatRequestItems(AccessRequests request, List<int> executions, int approvedResponse = -1)
        {
            SisConAxsContext db = new SisConAxsContext();
            string message = "";

            IQueryable<WorkflowExecution> execution = db.WorkflowExecution.Where(
                    t => t.WfExecObjectName == "ACCESS_REQUEST"
                         && executions.Any(wfExecutionID => wfExecutionID == t.WfExecID)
                         && t.WfResponse == ((approvedResponse == -1) ? t.WfResponse : approvedResponse)
                );
            IEnumerable<AccessRequestDetails> detail = request.AccessRequestDetails.Where(
                    t => execution.Any(x => t.RequestDetID == x.WfExecObjectID)
                ).OrderBy(t => t.AccessResources.ResourceCategories.CategoryDescription)
                 .ThenBy(t => t.AccessResources.ResourceFullName);


            if (approvedResponse == (int)WfExecResponse.Approved)
            {
                List<RequestEmailTreeResponse> list;
                List<RequestEmailTreeResponse> currentList;
                RequestEmailTreeResponse currentItem = null;
                RequestEmailTreeResponse listItem;
                String[] path;
                int level = 0;

                var categories = detail.GroupBy(x => x.AccessResources.ResourceCategories.CategoryDescription);
                foreach (var category in categories)
                {
                    list = new List<RequestEmailTreeResponse>();
                    foreach (AccessRequestDetails item in category)
                    {
                        path = item.AccessResources.ResourceFullName.Split('/');
                        currentList = list;
                        currentItem = null;
                        level = 0;
                        foreach (var name in path)
                        {
                            level++;
                            listItem = currentList.FirstOrDefault(x => x.name == name);
                            if (listItem == null)
                            {
                                listItem = new RequestEmailTreeResponse(currentItem);
                                listItem.name = name;
                                currentList.Add(listItem);
                            }
                            if (path.Length == level)
                            {
                                listItem.accessRequestDetail = item;
                                listItem.workflowExecution = execution.FirstOrDefault(x => x.WfExecObjectID == item.RequestDetID);
                                listItem.rootParent.category = item.AccessResources.ResourceCategories.CategoryDescription;
                            }
                            currentItem = listItem;
                            currentList = listItem.children;
                        }
                    }

                    string treeHtml = "";
                    List<int> executionsID = new List<int>();
                    foreach (var item in list)
                    {
                        treeHtml += item.getTreeHtml();
                        executionsID.AddRange(item.getListExecutionID());
                    }
                    message += String.Format(
                            "<div style='border: 1px solid navy; font-family: Arial, Helvetica, sans-serif;'>" +
                                "<table style='width: 100%; font-family: Arial, Helvetica, sans-serif;'>" +
                                "<tbody>" +
                                    "<tr>" +
                                        "<td style='width:90px;vertical-align:top; font-family: Arial, Helvetica, sans-serif;'><strong><span>Categoría</span></strong></td>" +
                                        "<td style='width:10px;vertical-align:top; font-family: Arial, Helvetica, sans-serif;'><strong>:</strong></td>" +
                                        "<td style='vertical-align:top; font-family: Arial, Helvetica, sans-serif;'>{0}</td>" +
                                    "</tr>" +
                                    "<tr>" +
                                        "<td style='vertical-align:top; font-family: Arial, Helvetica, sans-serif;'><strong><span>Solicitud/Permisos</span></strong></td>" +
                                        "<td style='vertical-align:top; font-family: Arial, Helvetica, sans-serif;'><strong>:</strong></td>" +
                                        "<td style='vertical-align:top; font-family: Arial, Helvetica, sans-serif;'>{1}</td>" +
                                    "</tr>" +
                                    "</tbody>" +
                                "</table>" +
                                "<div style='padding: .5em;'>{2}</div>" +
                            "</div><br/>",
                            category.Key,
                            "<ul style='font-family: Arial, Helvetica, sans-serif;'>" + treeHtml + "</ul>",
                            FormatHistory(request, executionsID, false)
                        );
                }
            }
            else
            {
                message += "<table cellspacing='5'>";
                message += "<tr>" +
                               "<th style='font-family: Arial, Helvetica, sans-serif;'>Categoría</th>" +
                               "<th style='font-family: Arial, Helvetica, sans-serif;'>Recurso</th>" +
                               "<th style='font-family: Arial, Helvetica, sans-serif;'>Valor</th>" +
                               "<th style='font-family: Arial, Helvetica, sans-serif;'>Tipo</th>" +
                           "</tr>";
                foreach (AccessRequestDetails item in detail)
                {
                    message += String.Format("<tr>" +
                                                 "<td style='width:20%; font-family: Arial, Helvetica, sans-serif;'>{0}</td>" +
                                                 "<td style='width:45%; font-family: Arial, Helvetica, sans-serif;'>{1}</td>" +
                                                 "<td style='width:25%; font-family: Arial, Helvetica, sans-serif;'>{2}</td>" +
                                                 "<td style='font-family: Arial, Helvetica, sans-serif; text-align:center;'><strong>{3}</strong></td>" +
                                             "</tr>",
                                             item.AccessResources.ResourceCategories.CategoryDescription,
                                             item.AccessResources.ResourceFullName,
                                             FormatRequestDetailDisplayValue(item.RequestDetDisplayValue) + (item.RequestDetAdditional > 0 ? ": " + item.RequestDetAdditionalStrValue : ""),
                                             item.CommonValuesType.CommonValueDisplay);
                }
                message += "</table>";
            }
            return message;
        }

        // Obtiene el historial de aprobaciones de una lista de execution
        public static string FormatHistory(AccessRequests request, List<int> executions, bool showCategory = true)
        {
            SisConAxsContext db = new SisConAxsContext();
            string message = "";
            string category = "";

            IQueryable<IGrouping<int, WorkflowExecutionHistory>> history = db.WorkflowExecutionHistory.Where(
                    t => executions.Any(wfExecutionID => wfExecutionID == t.WfExecID)
                         && t.WorkflowItems.CommonValues.CommonValueName == "CONSULTA"
                         && t.WfResponse > -1
                )
                .OrderBy(x => x.WfExecStartDate)
                .GroupBy(x => x.WfExecObjectID);
            IEnumerable<AccessRequestDetails> detail = request.AccessRequestDetails.Where(
                    t => history.Any(x => t.RequestDetID == x.Key)
                ).OrderBy(t => t.AccessResources.ResourceCategories.CategoryDescription)
                 .ThenBy(t => t.AccessResources.ResourceFullName);

            IGrouping<int, WorkflowExecutionHistory> group = null;
            foreach (var item in detail)
            {
                group = history.FirstOrDefault(t => t.Key == item.RequestDetID);
                category = "";
                if (showCategory)
                    category = item.AccessResources.ResourceCategories.CategoryDescription + ":";
                if (item.ReqDetValidityFrom.HasValue && item.ReqDetValidityUntil.HasValue)
                {
                    message += String.Format("<strong>{0} {1}</strong>, Con vigencia: <strong>{2}</strong><br/>",
                                            category, 
                                            item.AccessResources.ResourceFullName,
                                            item.ReqDetValidityFrom.Value.ToString("dd/MM/yyyy") + " - " + item.ReqDetValidityUntil.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    message += String.Format("<strong>{0} {1}</strong><br/>", category, item.AccessResources.ResourceFullName);
                }
                foreach (var historyDetail in group)
                {
                    message += String.Format("{0}<br/>", historyDetail.WfExecHistoryMessage);
                }
                message += "<br/>";
            }
            return message;
        }

        public static string FormatAttached(string requestAttached)
        {
            string attachHtml = string.Empty;
            if (!string.IsNullOrWhiteSpace(requestAttached))
            {
                var list = requestAttached.Split(';');
                foreach (var i in list)
                {
                    attachHtml += "<a href='" + i + "'>" + i + "</a></br>";
                }
            }
            return attachHtml;
        }

        public static string FormatRequestDetailDisplayValue(string displayValue)
        {
            string formatted = "";
            if (!String.IsNullOrWhiteSpace(displayValue))
            {
                string[] formattedArray = displayValue.Split(',');
                if (formattedArray.Length == 1)
                {
                    formatted = formattedArray[0];
                }
                else if (formattedArray.Length > 1)
                {
                    for (int i = 0; i < formattedArray.Length; i++)
                    {
                        string value = formattedArray[i]; //.Replace("-", " - ");
                        formatted += "<li><span style='color:blue'><strong>" + value + "</strong></span></li>";
                    }
                    formatted = "<ul>" + formatted + "</ul>";
                }
            }
            return formatted;
        }
    }
}