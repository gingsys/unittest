import { Component, html } from '../../../core.js';
import { AccessTypeResource, AccessType, AccessTypeValue } from '../../resources/resource-accesstype.js';
import { ControlSelect } from '../../ui-control-select.js';
import { UIMainComponent } from '../common/ui-main.js';
import "./ui-accesstype-edit.js";

export class AccessTypeComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
    }

    $template() {
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
        </style>
        <h3>Tipos de Acceso</h3>
        <div data-table-toolbar>
            <div class="btn-group">
                <button id="btnAdd" class="btn btn-warning" @click=${this.add}">
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
        <accesstype-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></accesstype-edit>`;
    }

    get Resource() {
        return AccessTypeResource;
    }
    get Entity() {
        return AccessType;
    }
    get entityID() {
        return 'AccessTypeID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title:"Nombre", field:"AccessTypeName", align:"left", valign:"middle", class:"col-nombre" },
            // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
            { title:"", field:"", align:"center", valign:"middle", class:"col-operate", formatter: $this.tableFormatterOperate, events:$this.tableEventColumnOperate }
        ];
    }

    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
}
customElements.define("accesstype-main", AccessTypeComponent);