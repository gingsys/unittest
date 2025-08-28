<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frm_rpt_integration_aad_log.aspx.cs" Inherits="SisConAxs.Reports.frm_rpt_integration_aad_log" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
 
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Reporte de Ejecuciones Sincronización de Azure AD</title>

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
            $('[data-datetime]').datetimepicker({
                format: 'DD/MM/YYYY',
                showClear: true,
                icons: {
                    time: "fas fa-clock",
                    date: "fas fa-calendar-alt",
                    previous: "fas fa-angle-left",
                    next: "fas fa-angle-right",
                    today: "fas fa-calendar-day",
                    clear: "fas fa-eraser",
                    close: "fas fa-window-close"
                },
                widgetPositioning:
                {
                    horizontal: 'left',
                    vertical: 'bottom'
                },
                locale: 'es'
            });

            

            $('#datetimeTypeStart').on('dp.change', e => {
                const date = moment(e.date._d).startOf('day').toDate();
                $('#datetimeTypeEnd').data("DateTimePicker").minDate(date);
            });
            $('#datetimeTypeEnd').on('dp.change', e => {
                const date = moment(e.date._d).endOf('day').toDate();
                $('#datetimeTypeStart').data("DateTimePicker").maxDate(date);
            });

            $('#datetimeStart').on('dp.change', e => {
                const date = moment(e.date._d).startOf('day').toDate();
                $('#datetimeEnd').data("DateTimePicker").minDate(date);
            });
            $('#datetimeEnd').on('dp.change', e => {
                const date = moment(e.date._d).endOf('day').toDate();
                $('#datetimeStart').data("DateTimePicker").maxDate(date);
            });

            $('#accordion').on('show.bs.collapse', function () {
                $('#icon').removeClass("fas fa-chevron-down");
                $('#icon').addClass("fas fa-chevron-up");
                $('.panel-title').prop('title', 'Contraer');
            });
            $('#accordion').on('hide.bs.collapse', function () {
                $('#icon').removeClass("fas fa-chevron-up");
                $('#icon').addClass("fas fa-chevron-down");
                $('.panel-title').prop('title', 'Expandir');
            });
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
                            <h5 class="panel-title" title="Contraer"><i class="fas fa-chart-bar"></i>Reporte Ejecuciones Sincronización Azure AD <i id="icon" class="fas fa-chevron-up" <%--style="float: right;"--%>></i></h5>
                        </div>
                    </a>
                    <div id="collapseOne" class="panel-collapse show" role="tabpanel" aria-labelledby="headingOne">
                        <div class="panel-body">

                            <div class="form-group row pt-2 pl-2 mb-0">
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-secondary" OnClick="LoadReport()"><i class="fas fa-search"></i> Buscar</button>
                                </div>
                            </div>

                            <%--<div class="form-group row pl-2 mb-0">
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0"><strong>Fecha Alta/Baja</strong></label>
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha desde</label>
                                <div class="col-sm-2">
                                    <div class="input-group date" id="datetimeTypeStart" data-datetime>
                                        <input id="dpTypeStart" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha hasta</label>
                                <div class="col-sm-2">
                                    <div class="input-group date" id="datetimeTypeEnd" data-datetime>
                                        <input id="dpTypeEnd" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>                                
                            </div>--%>

                            <div class="form-group row pl-2 mb-0">
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0"><strong>Fecha Ejecución</strong></label>
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha desde</label>
                                <div class="col-sm-2">
                                    <div class="input-group date" id="datetimeStart" data-datetime>
                                        <input id="dpStart" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>
                                <label class="offset-sm-0 col-sm-1 col-form-label pr-0">Fecha hasta</label>
                                <div class="col-sm-2">
                                    <div class="input-group date" id="datetimeEnd" data-datetime>
                                        <input id="dpEnd" runat="server" type="text" class="fecha form-control" />
                                        <span class="input-group-addon">
                                            <span class="btn btn-outline-secondary">
                                                <i class="far fa-calendar-alt " aria-hidden="true"></i>
                                            </span>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group row pl-2 mb-0">
                                <%--<label class="offset-1 col-sm-1 col-form-label">Estados:</label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="" Text="<TODOS>"></asp:ListItem>
                                        <asp:ListItem Value="46" Text="ALTAS"></asp:ListItem>
                                        <asp:ListItem Value="47" Text="BAJAS"></asp:ListItem>
                                        <asp:ListItem Value="48" Text="MODIFICACIONES"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>--%>
                                <label class="offset-1 col-sm-1 col-form-label">Tipo:</label>
                                <div class="col-sm-2">
                                    <asp:DropDownList ID="ddlResult" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="" Text="<TODOS>"></asp:ListItem>
                                        <asp:ListItem Value="SUCCESS" Text="Ejecución Completa"></asp:ListItem>
                                        <asp:ListItem Value="ERROR" Text="Error"></asp:ListItem>
                                        <asp:ListItem Value="WITHOUT_CHANGES" Text="Sin Cambios"></asp:ListItem>
                                        <asp:ListItem Value="PARCIAL" Text="Ejecución Parcial"></asp:ListItem>
                                    </asp:DropDownList>
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
     
