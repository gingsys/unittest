import { UIMainComponent } from '../common/ui-main.js';
import { PeopleResource, People } from '../../resources/resource-people.js';
import { CommonValueResource, CommonValueSet } from '../../resources/resource-commonvalue.js';
import { SessionResource, Session } from '../../resources/resource-session.js';
import { PeopleView } from './ui-people.view.js';
//import sheet from './../../../../Content/app.css' assert { type: 'css' } ;
import './ui-people-edit.js';

export class PeopleComponent extends UIMainComponent {
    constructor() {
        super();
        this.$allCompaniesFlag = false;
    }

    get $template() {
        return new PeopleView(this);
    }

    get Resource() {
        return PeopleResource;
    }
    get Entity() {
        return People;
    }
    get entityID() {
        return 'PeopleID';
    }

    async connectedCallback() {
        super.connectedCallback();
        this.$presenter.showLoading();
        await this.loadFilters();
        this.loadData();
    }
    
    createTable() {
        $(this.table).bootstrapTable({
            toolbar: this.querySelector('[data-table-toolbar]'),
            height: window.innerHeight - 130,
            filterControl: true,
            searchOnEnterKey: true,
            // showRefresh: "true",
            sidePagination: "server",
            pagination: "true",
            pageSize: "12",
            pageList: "[12, 25, 50, 100]",
            columns: this.tableColumns,
            onDblClickRow: (row, element, field) => {
                if (row.PeopleCompany == SessionResource.session.CompanyID || row.PeopleStatus == 0) {
                    this.edit(row.PeopleID);
                } else if (row.PeopleCompany != SessionResource.session.CompanyID && row.PeopleStatus == 1) {
                    iziToast.info({ title: 'No es posible editar, la persona esta activa en otra empresa.', timeout: 10000 });
                }
            },
            onRefresh: () => {
                this.loadData();
            },
            onPageChange: () => {
                this.loadData();
            }
        });
    }
    get tableColumns() {
        const $this = this;
        return [
            { title: "Número", field: "PeopleInternalID", filterControl: "input", filterControlPlaceholder: "Número", align: "left", valign: "middle" },
                { title: "Apellidos", field: "PeopleFullLastName", filterControl: "input", filterControlPlaceholder: "Apellidos", align: "left", valign: "middle" },
                { title: "Nombres", field: "PeopleFullFirstName", filterControl: "input", filterControlPlaceholder: "Nombres", align: "left", valign: "middle" },
                { title: "Tipo Doc.", field: "PeopleDocTypeName", filterControl: "input", filterControlPlaceholder: "Tipo Doc.", align: "center", valign: "middle" },
                { title: "Número Doc", field: "PeopleDocNum", filterControl: "input", filterControlPlaceholder: "Número Doc.", align: "center", valign: "middle" },
                { title: "Email", field: "PeopleEmail", filterControl: "input", filterControlPlaceholder: "Email", align: "center", valign: "middle" },
                { title: "Clasificación", field: "PeopleTypeClasificacionName", filterControl: "input", filterControlPlaceholder: "Clasificación", align: "center", valign: "middle" },
                { title: "Área", field: "PeopleDepartmentName", filterControl: "input", filterControlPlaceholder: "Área", align: "left", valign: "middle" },
                { title: "Cargo", field: "PeoplePositionName", filterControl: "input", filterControlPlaceholder: "Cargo", align: "left", valign: "middle" },
                { title: "Empresa", field: "PeopleCompanyName", filterControl: "input", filterControlPlaceholder: "Empresa", align: "left", valign: "middle" },
                { title: "Activo", field: "PeopleStatusDesc", filterControl: "input", filterControlPlaceholder: "Activo", align: "center", valign: "middle" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }

    async loadFilters() {
        await CommonValueResource.loadSelect(this.$.cboClasificationType, CommonValueSet.SET_TIPO_CLASIFICACION);
        this.$.cboClasificationType.value = People.CLASIFICACION_COLABORADOR_EN_PROCESO;
    }
    
    prepareFilters(filters) {
        super.prepareFilters(filters);
        filters.PeopleTypeClasificacion = this.$.cboClasificationType.value;
        filters.allCompanies = this.$allCompaniesFlag;
        filters.status = -1;
    }

    add(classificationType) {
        this.$.editor.add(classificationType);
    }

    btnViewAllClick(e) {
        if (this.$allCompaniesFlag) {
            this.$allCompaniesFlag = false;
            this.$.btnViewAll.innerHTML = `<i class='fas fa-binoculars'></i> Ver Todas las Empresas`;
        } else {
            this.$allCompaniesFlag = true;
            this.$.btnViewAll.innerHTML = `<i class='fas fa-binoculars'></i> Ver sólo ${SessionResource.session.CompanyName}`;
        }
        $(this.table).bootstrapTable('selectPage', 1);
        this.loadData();
    }

    tableFormatterStatus(value, row, index) {
        const className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
    tableFormatterOperate(value, row, index) {
        if (row.PeopleCompany == SessionResource.session.CompanyID || row.PeopleStatus == 0) {
            return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                        <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                    </div>`;
        }
    }
    applyFilter() {
        this.loadData();
    }
}
customElements.define('people-main', PeopleComponent);