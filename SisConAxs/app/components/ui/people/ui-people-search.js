import { Component, html } from '../../../core/core.js';
import { CommonValueResource, CommonValueSet } from '../../resources/resource-commonvalue.js';
import { PeopleResource, People } from '../../resources/resource-people.js';
import { SessionResource, Session } from '../../resources/resource-session.js';
import { BootstrapTableUtils } from '../common/bootstraptable-utils.js';

export class PeopleSearchComponent extends Component {
    constructor() {
        super(false);
        this.$resource = new PeopleResource();
        this.$allCompaniesFlag = false;
    }

    $template() {
        return html`
        <style>
            #sectionTablePeople .fixed-table-toolbar:first-child .bs-bars.float-left {
                width: calc(100% - 40px) !important;
            }
        </style>
        <div class="modal" id="formPeopleSearchModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="exampleModalLabel">Seleccionar Destinatario</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div id="toolbarPeopleSearch">
                            <div class="row pr-3">
                                <div class="col-auto mr-auto"></div>
                                <div class="col-auto pl-2 pr-0 mb-2">
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <label class="input-group-text" for="cboClasificationType">Tipo</label>
                                        </div>
                                        <select id="cboClasificationType" class="custom-select" @change=${this.loadData}>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-auto pl-2 pr-0">
                                    <button type="button" class="btn btn-outline-info float-right" id="btnViewAll" @click=${this.btnViewAllClick}>
                                        <i class='fas fa-binoculars'></i> Ver Todas las Empresas
                                    </button>
                                </div>
                            </div>
                        </div>
                        <section id="sectionTablePeople">
                            <table
                                id="table"
                                data-locale="es-ES"
                                data-show-refresh="true"
                                >
                                <thead>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </section>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cerrar</button>
                        <!-- <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button> -->
                    </div>
                </div>
            </div>
        </div>`;
    }

    connectedCallback() {        
        super.connectedCallback();

        const $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 255,
            toolbar: this.$.toolbarPeopleSearch,
            filterControl: true,
            searchOnEnterKey: true,
            showRefresh: "true",
            sidePagination: "server",
            pagination: "true",
            pageSize: "10",
            columns: [
                { title: "NúmeroID", field: "PeopleInternalID", filterControl: "input", filterControlPlaceholder: "Número", align: "left", valign: "middle" },
                { title: "Apellidos", field: "PeopleFullLastName", filterControl: "input", filterControlPlaceholder: "Apellidos", align: "left", valign: "middle" },
                { title: "Nombres", field: "PeopleFullFirstName", filterControl: "input", filterControlPlaceholder: "Nombres", align: "left", valign: "middle" },
                // { title:"Tipo Doc.", field:"PeopleDocTypeName", align:"center", valign:"middle" },
                // { title:"Número Doc", field:"PeopleDocNum", align:"center", valign:"middle" },
                { title: "Email", field: "PeopleEmail", filterControl: "input", filterControlPlaceholder: "Email", align: "center", valign: "middle" },
                { title: "Área", field: "PeopleDepartmentName", filterControl: "input", filterControlPlaceholder: "Área", align: "left", valign: "middle" },
                { title: "Cargo", field: "PeoplePositionName", filterControl: "input", filterControlPlaceholder: "Cargo", align: "left", valign: "middle" },
                { title: "Empresa", field: "PeopleCompanyName", filterControl: "input", filterControlPlaceholder: "Empresa", align: "left", valign: "middle", width: 150 },
                // { title:"Activo", field:"PeopleStatusDesc", align:"center", valign:"middle" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title: "", field: "", align: "center", valign: "middle", width: "120", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onRefresh: () => {
                this.loadData();
            },
            onDblClickRow: (row, element, field) => {
                if (row.PeopleStatus == 1) {
                    this.__onSelect(row);
                }
            },
            onPageChange: () => {
                this.loadData();
            },
        });

        this.loadFilters();
    }

    async loadFilters() {
        await CommonValueResource.loadSelect(this.$.cboClasificationType, CommonValueSet.SET_TIPO_CLASIFICACION);
        this.$.cboClasificationType.value = People.CLASIFICACION_COLABORADOR;
    }

    show() {
        this.$allCompaniesFlag = false;

        this.$$.table.bootstrapTable('load', []);
        const filters = BootstrapTableUtils.getFilters(this.$.table);
        if(filters.some(f => f.value != '')) {
            this.$$.table.bootstrapTable('clearFilterControl');
        } else {
            this.loadData();
        }

        this.$$.formPeopleSearchModal.modal({ backdrop: 'static' });
    }

    btnViewAllClick(e) {
        this.$.btnViewAll.innerHTML = this.$allCompaniesFlag ?
            `<i class='fas fa-binoculars'></i> Ver Todas las Empresas` :
            `<i class='fas fa-binoculars'></i> Ver sólo ${SessionResource.session.CompanyName}`;
        this.$allCompaniesFlag = !this.$allCompaniesFlag;
        this.loadData();
    }

    prepareFilters(filters) {
        filters.PeopleTypeClasificacion = this.$.cboClasificationType.value;
        filters.allCompanies = this.$allCompaniesFlag;
    }

    loadData() {
        this.$presenter.showLoading();
        const filters = BootstrapTableUtils.prepareFilters(this.$.table);
        this.prepareFilters(filters);
        this.$resource.getPaginate(filters).then(
            data => {
                this.$$.table.bootstrapTable('load', {
                    rows: People.fromList(data.Rows),
                    total: data.TotalRows
                });
                BootstrapTableUtils.setFilters(this.$.table, filters);
                this.$presenter.hideLoading();
            },
            err => {
                iziToast.error({ title: 'Error al cargar' });
            });
    }

    tableFormatterOperate(value, row, index) {
        if (row.PeopleStatus == 1) {
            return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-outline-primary btn-select" title="Seleccionar">
                        <i class="fas fa-check"></i> Seleccionar
                    </button>
                </div>`;
        } else {
            return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button disabled type="button" class="btn btn-outline-danger" title="Inactivo">
                        <i class="fas fa-times"></i> Inactivo
                    </button>
                </div>`;
        }
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let _self = this;
        return {
            'click .btn-select': _self.btnColumnSelectClick.bind(_self)
        }
    }
    btnColumnSelectClick(e, value, row, index) {
        this.__onSelect(row);
    }

    __onSelect(entity) {
        console.log('PeopleSearch.selectitem >', entity);
        this.$$.formPeopleSearchModal.modal('hide');
        this.dispatchEvent(new CustomEvent('selectitem', {
            detail: entity
        }));
    }
}
customElements.define('people-search', PeopleSearchComponent);