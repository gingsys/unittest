import { html } from '../../../core/core.js';
import { UIMainComponent } from '../common/ui-main.js';
import { CompanyResource, Company } from '../../resources/resource-company.js';
import { ControlSelect } from '../../ui-control-select.js';
import './ui-company-edit.js';

export class CompanyComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
        this.$resource = new CompanyResource();
        this.$entity = new Company();
    }

    get $template() {
        return html `
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
        </style>
        <h3>Empresas</h3>
        <div data-table-toolbar>
            <div class="btn-group">
                <button id="btnAdd" class="btn btn-warning" @click=${this.add}>
                    <i class="fas fa-plus"></i> Agregar
                </button>
            </div>
        </div>
        <table
            data-table-main
            data-locale="es-ES"
            data-search="true"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <company-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></company-edit>`;
    }

    get Resource() {
        return CompanyResource;
    }
    get Entity() {
        return Company;
    }
    get entityID() {
        return 'CompanyID'
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title: "ID Contribuyente", field: "CompanyTaxpayerID", align: "left", valign: "middle", class: "" },
            { title: "Nombre", field: "CompanyName", align: "left", valign: "middle", class: "" },
            { title: "Etiqueta", field: "CompanyDisplay", align: "left", valign: "middle", class: "" },
            { title:"Estado", field:"CompanyActive", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
            { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }
    

    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
}
customElements.define('company-main', CompanyComponent);




export class SelectCompany extends ControlSelect {
    constructor(select) {
        super(select, {
            noneSelectedText: 'Seleccione empresa'
        });
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
    }

    refresh() {
        return CompanyResource.loadSelect(this.__select).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
    refreshFromData(data) {
        // let select = this.__select;
        // select.innerHTML = '';
        // data.forEach(item => {
        //     let opt = document.createElement('option');
        //     opt.innerHTML = item.CompanyName;
        //     opt.value = item.CompanyID;
        //     opt.$data = item;
        //     select.appendChild(opt);
        // });
        // select.value = null;
    }
}