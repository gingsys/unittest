import { html } from '../../../core/core.js';
import { UIMainComponent } from '../common/ui-main.js';
import { RequestTemplateResource, RequestTemplate } from '../../resources/resource-request-template.js';
import { SessionResource } from '../../resources/resource-session.js';
import './ui-request-template-edit.js';
// import '../common/ui-window.js';

export class RequestTemplateComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
        this.$resource = new RequestTemplateResource();
        this.$entity = new RequestTemplate();
    }

    get $template() {
        return html `
        <style>
            /* cols */
            request-template-main .col-name {
                box-sizing: border-box;
                width: 120px;
            }
            request-template-main .col-desc {
                box-sizing: border-box;
                /* width: 120px; */
            }
            request-template-main .col-estado {
                box-sizing: border-box;
                width: 50px;
            }
            request-template-main .col-operate {
                box-sizing: border-box;
                width: 70px;
            }
            /* cols */

            request-template-main .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            request-template-main .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }
        </style>
        <h3>Solicitudes Base</h3>
        <div data-table-toolbar>
            <div class="btn-group">
                <button class="btn btn-warning" @click=${this.add}>
                    <i class="fas fa-plus"></i> Agregar
                </button>
                <!--<button class="btn btn-dark" @click=${e => this.$.windowTest.show()}>
                    <i class="far fa-window-maximize"></i> Window
                </button>-->
            </div>
        </div>
        <table
            data-table-main
            data-locale="es-ES"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <request-template-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></request-template-edit>
        <!--<ui-window id="windowTest"></ui-window>-->
        `;
    }

    get Resource() {
        return RequestTemplateResource;
    }
    get Entity() {
        return RequestTemplate;
    }
    get entityID() {
        return 'ReqTemplateID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title: "Tipo de Empleado", field: "ReqTemplateEmployeeTypeName", filterControl: "input", filterControlPlaceholder: "Tipo de Empleado", align: "left", valign: "middle" }
            ,{ title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }

    prepareFilters(filters) {
        super.prepareFilters(filters);
    }

    tableFormatterStatus(value, row, index) {
        const className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
}
customElements.define('request-template-main', RequestTemplateComponent);