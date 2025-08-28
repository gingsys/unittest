import { Component, html } from '../../core/core.js';
import { ResourcePeopleResource, ResourcePeople } from '../resource-resourcepeople.js';
import { RequestEditComponent } from '../ui-request-edit.js';
import { RequestResource, Request } from '../resource-request.js';
import '../ui/people/ui-people-search.js';

export class ReportResourcePeopleComponent extends Component {
    constructor() {
        super(false);
        this.$data = [];
        this.$people = null;
        this.$resource = new ResourcePeopleResource();
        this.$requestResource = new RequestResource();
        this.filterStatus = 1;

        this.addComponent('requestEdit', new RequestEditComponent(this.$presenter));
    }

    get $template() {
        return html `
        <style>
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
            .btn-show {
                width: 90px;
            }
        </style>
        <h3>Recursos por Destinatario</h3>
        <div class="form-group row mt-2">
            <div class="input-group col-sm-2">
                <div class="input-group-prepend">
                    <button class="btn btn-secondary" type="button" id="btnPeopleSearch" title="Buscar Usuario" @click=${this.btnPeopleSearchClick}>
                        Buscar <i class="fas fa-search"></i>
                    </button>
                </div>
                <!-- <label>Nombre</label> -->
                <input type="text" readonly class="form-control-plaintext pl-2" value="Nombre" />
            </div>
            <div class="col-sm-10">
                <input type="text" readonly class="form-control" id="txtPeopleName" />
            </div>
        </div>
        <div class="form-group row">
            <label for="txtCompany" class="col-sm-2 col-form-label">Empresa/Proyecto</label>
            <div class="col-sm-5">
                <input type="text" readonly class="form-control" id="txtCompanyProject" />
            </div>
            <label for="txtPosition" class="col-sm-1 col-form-label">Cargo</label>
            <div class="col-sm-4">
                <input type="text" readonly class="form-control" id="txtPosition" />
            </div>
        </div>
        <div id="toolbar">
            <div class="btn-group" data-toggle="buttons">
                <button class="btn btn-outline-success active" id="optFilterStatusActive" @click="${this.optFilterStatusActiveClick}">
                    <i class="fas fa-check"></i> Activos <input type="radio" name="optFilterStatus" value="1" autocomplete="off" checked style="display: none;" />
                </button>
                <button class="btn btn-outline-dark" id="optFilterStatusAll" @click="${this.optFilterStatusAllClick}">
                    <i class="fas fa-border-all"></i> Todos <input type="radio" name="optFilterStatus" value="0" autocomplete="off" style="display: none;" />
                </button>
            </div>
        </div>
        <table
            id="table"
            data-locale="es-ES"
            data-toolbar="#toolbar"
            data-search="true"
            data-show-refresh="true"
            data-pagination="true"
            data-page-size="400"
            data-page-list="[400, 800, 1200, Unlimited]"
            data-pagination-h-align="left"
            data-pagination-detail-h-align="right"
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
        <people-search id="peopleSearchResourcePeople" @selectitem=${this.onPeopleSelect}></people-search>
        <div data-component="requestEdit"></div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        const $this = this;
        this.$$.table.bootstrapTable({
            detailView: true,
            filterControl: true,
            searchOnEnterKey: true,
            height: window.innerHeight - 220,
            columns: [
                { title: "Empresa", field: "PresCompanyName", filterControl: "input", filterControlPlaceholder: "Empresa", align: "left", valign: "middle", width: 130 },
                { title: "Categoría", field: "CategoryName", filterControl: "input", filterControlPlaceholder: "Categoría", align: "left", valign: "middle" },
                { title: "Recurso", field: "ResourceFullName", filterControl: "input", filterControlPlaceholder: "Recurso", align: "left", valign: "middle", formatter: $this.tableFormatterResource },
                { title: "Valor", field: "PresDisplayValue", filterControl: "input", filterControlPlaceholder: "Valor", align: "left", valign: "middle" },
                { title: "Sol. Alta", field: "AddedRequestNum", fieldID: "AddedRequestID", filterControl: "input", filterControlPlaceholder: "", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterRequestNum, events: $this.tableEventColumnOperate },
                { title: "Fecha Alta", field: "PresDateStart", filterControl: "input", filterControlPlaceholder: "F. Alta", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterDate },
                { title: "Sol. Baja", field: "RemovedRequestNum", fieldID: "RemovedRequestID", filterControl: "input", filterControlPlaceholder: "", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterRequestNum, events: $this.tableEventColumnOperate },
                { title: "Fecha Baja", field: "PresDateEnd", filterControl: "input", filterControlPlaceholder: "F. Baja", align: "center", valign: "middle", width: 100, formatter: $this.tableFormatterDate },
                { title:"Estado", field:"PresActive", align:"center", valign:"middle", class:"col-estado", width: 50, formatter: $this.tableFormatterStatus },
                { title:"Sin Sol.", field:"", align:"center", valign:"middle", width: 50, class:"col-estado", formatter: $this.tableFormatterLog },
                // { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onRefresh: () => {
                this.loadData();
            },
            onExpandRow: this.tableOnExpandRow.bind(this)
        });
    }

    tableOnExpandRow(index, row, $detail) {
        if(row.ResourcePeopleLog.length > 0) {
            const $this = this;
            const subTable = document.importNode(this.$.templateSubtable.content.querySelector('table'), true);
            $(subTable).bootstrapTable({
                // showHeader: false,
                data: row.ResourcePeopleLog,
                columns: [
                    { title: "Fecha", field: "CreateDate", filterControl: "input", align: "center", valign: "middle", formatter: $this.tableFormatterDate },
                    { title: "Acción", field: "ActionDisplay", filterControl: "input", align: "center", valign: "middle" },
                    { title: "Origen", field: "Source", filterControl: "input", align: "center", valign: "middle" },
                    { title: "Descripción", field: "Description", filterControl: "input", align: "left", valign: "middle" },
                    { title: "Usuario", field: "CreateUser", filterControl: "input", align: "center", valign: "middle" },
                ]
            });
            $detail.html(subTable);
        }
    }

    tableFormatterResource(value, row, index) {
        let i = row.ResourceFullName.lastIndexOf('/') + 1;
        let resourceName = row.ResourceFullName.substring(i);
        return `<strong>${resourceName}</strong><br/><small>${row.ResourceFullName}</small>`;
    }

    tableFormatterRequestNum(value, row, index) {
        let width = 8;
        if(value != null && !isNaN(value)) {
            let number = new Array(width - value.toString().length + 1).join('0') + value;
            return `<button type="button" class="btn btn-sm btn-outline-secondary btn-show" data-requestid="${row[this.fieldID]}" title="Ver Solicitud">
                        <i class="fas fa-search"></i> ${number}
                    </button>`;
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

    tableFormatterLog(value, row, index) {
        return `${row.ResourcePeopleLog.length}`;
    }

    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-show': $this.btnColumnShowClick.bind($this)
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

    btnPeopleSearchClick(e) {
        this.$.peopleSearchResourcePeople.show();
    }
    onPeopleSelect(e) {
        this.$people = e.detail;
        this.$.txtPeopleName.value     = this.$people.PeopleFullname;
        this.$.txtCompanyProject.value = this.$people.PeopleCompanyName +'/'+ this.$people.PeopleAttribute2;
        this.$.txtPosition.value       = this.$people.PeopleAttribute3;
        this.$$.table.bootstrapTable('refresh');
    }

    optFilterStatusActiveClick() {
        this.filterStatus = 1;
        this.loadData();
    }

    optFilterStatusAllClick() {
        this.filterStatus = 0;
        this.loadData();
    }

    loadData() {
        if(this.$people == null || this.$people.PeopleID <= 0) {
            return;
        }
        this.$presenter.showLoading();
        this.$resource.getByPeopleID(this.$people.PeopleID, { onlyActive: this.filterStatus }).then(
            r => {
                this.$data = ResourcePeople.fromList(r);
                this.$$.table.bootstrapTable('load', this.$data);
                this.$presenter.hideLoading();
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err });
            });
    }
}
customElements.define('report-resourcepeople', ReportResourcePeopleComponent);