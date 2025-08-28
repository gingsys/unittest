<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frm_rpt_req_approved_by_approver.aspx.cs" Inherits="SisConAxs.Reports.frm_rpt_req_approved_by_approver" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Reporte de Solicitudes por Aprobador</title>

    <link rel="stylesheet" href="../dist/libraries.min.css" />
    <link rel="stylesheet" href="../content/app.css" />

    <script src="../dist/libraries.min.js"></script>
    <script src="../legacy/jquery-ui.min.js"></script>

    <style type="text/css">
        .btnVolver {
            width: 30px;
            border-width: 0px;
            /*background-image: url('../Content/images/mnu_retornar.png') !important;*/
            height: 30px;
            background-color: transparent;
            border: none !important;
        }

        .btnBuscar {
            width: 30px;
            border-width: 0px;
            /*background-image: url('../Content/images/mnu_lampara.ico') !important;*/
            height: 30px;
            background-color: transparent;
            border: none !important;
        }

        .trHeight {
            height: 32px;
        }

        .fecha {
            width: 100px;
        }
    </style>
    <script type="text/javascript">
        $(function () {

            $('#datetimepickeri').datetimepicker({
                format: 'DD/MM/YYYY',
                widgetPositioning:
                {
                    horizontal: 'left',
                    vertical: 'bottom'
                },
                locale: 'es'
            });

            $('#datetimepickeri').on('dp.change', (e) => {
                $('#datetimepickerf').data("DateTimePicker").minDate(new Date(e.date._d));
            });

            $('#datetimepickerf').datetimepicker({
                format: 'DD/MM/YYYY',
                widgetPositioning:
                {
                    horizontal: 'left',
                    vertical: 'bottom'
                },
                locale: 'es'
            });

            $('#datetimepickerf').on('dp.change', (e) => {
                $('#datetimepickeri').data("DateTimePicker").maxDate(new Date(e.date._d));
            });

            $('#accordion').on('show.bs.collapse', function () {
                $('#icon').removeClass("fas fa-chevron-down");
                $('#icon').addClass("fas fa-chevron-up");
                $('.panel-title').prop('title', 'Contraer');
            })

            $('#accordion').on('hide.bs.collapse', function () {
                $('#icon').removeClass("fas fa-chevron-up");
                $('#icon').addClass("fas fa-chevron-down");
                $('.panel-title').prop('title', 'Expandir');
            })
        });

        function LoadReport() {
            __doPostBack('CallServerSideFunctionPostBack', 'LoadReport');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="margin: 0px">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="col-sm-12 sidebar">
            <div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">
                <div class="panel panel-default">
                    <a role="button" data-toggle="collapse" data-parent="#accordion" href="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                        <div class="panel-heading" role="tab" id="headingOne">
                            <h5 class="panel-title" title="Contraer"><i class="fas fa-chart-bar"></i> Reporte de Solicitudes por Aprobador <i id="icon" class="fas fa-chevron-up" <%--style="float: right;"--%>></i></h5>
                        </div>
                    </a>
                    <div id="collapseOne" class="panel-collapse show" role="tabpanel" aria-labelledby="headingOne">
                        <div class="panel-body">

                            <div class="form-group row pt-2 pl-2 mb-0">
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-secondary" OnClick="LoadReport()"><i class="fas fa-search"></i> Buscar</button>
                                </div>
                            </div>
                            <div class="form-group row pl-2 mb-0">
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha desde</label>
                                <div class="col-sm-2">
                                    <div class='input-group date' id='datetimepickeri'>
                                        <input id="dpi" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha hasta</label>
                                <div class="col-sm-2">
                                    <div class='input-group date' id='datetimepickerf'>
                                        <input id="dpf" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>                                
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div style="width: 100%; height: 100%; text-align: center">
            <div style="text-align: center; width: 100%; height: 100%; align-content: center">
                <rsweb:reportviewer id="ReportViewer01" runat="server" width="100%" height="550px" overflow="inherit" processingmode="Remote" showcredentialprompts="False" visible="false"></rsweb:reportviewer>
            </div>
        </div>
    </form>
</body>
</html>
