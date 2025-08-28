import { Component, html } from '../../core/core.js';
import { PeopleResource } from '../resources/resource-people.js';
import { RequestResource, Request } from '../resource-request.js';
import { RequestEditComponent } from '../ui-request-edit.js';

export class ReportApproverRequestForApprovalComponent extends Component {
    constructor() {
        super(false);
        this.$resource = new PeopleResource();
        this.$requestResource = new RequestResource();

        this.addComponent('requestEdit', new RequestEditComponent(this.$presenter));
    }

    get $template() {
        return html`
        <style>
            /* cols */
            /* .col-check {
                box-sizing: border-box;
                width: 25px;
            } */
            .col-name {
                box-sizing: border-box;
                width: 120px;
            }
            .col-desc {
                box-sizing: border-box;
                /* width: 120px; */
            }
            .col-estado {
                box-sizing: border-box;
                width: 50px;
            }
            .col-operate {
                box-sizing: border-box;
                width: 70px;
            }
            /* cols */

            .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }

            .detail-view > td {
                padding: 0 !important;
                padding-left: 29px !important;
            }
            .detail-view .subtable {
                font-size: 1.1em;
            }
        </style>
        <h3>Aprobadores vs Solicitudes Pendientes</h3>
        <div id="toolbar">
            <div class="btn-group">
                <!-- <button id="btnAdd" class="btn btn-warning">
                    <i class="fas fa-plus"></i> Agregar
                </button> -->
            </div>
        </div>
        <table
            id="table"
            data-locale="es-ES"
            data-toolbar="#toolbar"
            data-search="true"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <template id="templateSubtable">
            <table  data-show-refresh="true"
                    data-locale="es-ES"
                    class="subtable">
                <thead>
                </thead>
            </table>
        </template>
        <div data-component="requestEdit"></div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        const $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 130,
            detailView: true,
            columns: [
                { title: "Cod. Persona", field: "PeopleInternalID", align: "left", valign: "middle", formatter: $this.tableFormatterBold },
                { title: "Nombres y Apellidos", field: "PeopleFullname", align: "left", valign: "middle", formatter: $this.tableFormatterBold },
                { title: "GH Cargo", field: "PeopleAttribute2", align: "left", valign: "middle" },
                { title: "GH Cargo", field: "PeopleAttribute3", align: "left", valign: "middle" },
                { title: "WF Área", field: "PeopleDepartmentName", align: "left", valign: "middle" },
                { title: "WF Cargo", field: "PeoplePositionName", align: "left", valign: "middle" },
                { title: "Por Aprobar", field: "ForApprove", align: "center", valign: "middle" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                // { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onRefresh: () => {
                this.loadData();
            },
            onExpandRow: this.tableOnExpandRow.bind(this)
        });
        this.loadData();
    }

    tableOnExpandRow(index, row, $detail) {
        if(row.Requests.length > 0) {
            let $this = this;
            let subTable = document.importNode(this.$.templateSubtable.content.querySelector('table'), true);
            $(subTable).bootstrapTable({
                // showHeader: false,
                data: row.Requests,
                columns: [
                    // { title: "Empresa", field: "RequestCompanyName", filterControl: "input", align: "left", valign: "middle", width: 180 },
                    { title: "Fecha", field: "RequestDate", filterControl: "input", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterDate },
                    { title: "Número", field: "RequestNumber", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterRequestNum, events: $this.tableEventColumnOperate },
                    { title: "Para", field: "RequestToName", filterControl: "input", align: "left", valign: "middle" }
                ]
            });
            $detail.html(subTable);
        }
    }

    loadData() {
        this.$presenter.showLoading();
        this.$resource.approvedRequestPending().then(
            r => {
                this.$$.table.bootstrapTable('load', r);
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err }); //'Error al cargar'});
            });
    }

    tableFormatterRequestNum(value, row, index) {
        let width = 8;
        if(value != null && !isNaN(value)) {
            let number = new Array(width - value.toString().length + 1).join('0') + value;
            return `<button type="button" class="btn btn-sm btn-outline-secondary btn-show" data-requestid="${row.RequestID}" title="Ver Solicitud">
                        <i class="fas fa-search"></i> ${number}
                    </button>`;
        }
    }

    tableFormatterBold(value, row, index) {
        if(value != null) {
            return `<strong>${value}</strong>`;
        }
    }

    tableFormatterDate(value, row, index) {
        if(value != null) {
            return moment(value).format('DD/MM/YYYY');
        }
    }

    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }

    get tableEventColumnOperate() {
        let _self = this;
        return {
            'click .btn-show': _self.btnColumnShowClick.bind(_self)
        }
    }
    btnColumnShowClick(e, value, row, index) {
        this.$presenter.showLoading();
        this.$requestResource.getByID(e.currentTarget.dataset.requestid).then(
            r => {
                let entity = Request.fromObject(r);
                this.$component.requestEdit.edit(entity);
            },
            err => {
                this.$presenter.hideLoading();
                console.error('Error', err);
                iziToast.error({ title: 'Error al cargar' });
            });
    }
}
customElements.define('report-approver-requestforapproval', ReportApproverRequestForApprovalComponent);