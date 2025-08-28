import { html } from '../../../core/core.js';
import { UIMainComponent } from '../common/ui-main.js';
import { WorkflowApproveHierarchyResource, WorkflowApproveHierarchy, WorkflowHierarchyMember } from '../../resources/resource-approvehierarchy.js';
import { BootstrapTableUtils } from '../common/bootstraptable-utils.js';
import { ControlSelect } from '../../ui-control-select.js';
import './ui-approvehierarchy-edit.js';

export class ApproveHierarchyComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
    }

    get $template() {
        return html `
        <style>
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
        <h3>Jerarquías de Aprobación</h3>
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
        <approvehierarchy-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></approvehierarchy-edit>`;
    }

    get Resource() {
        return WorkflowApproveHierarchyResource;
    }
    get Entity() {
        return WorkflowApproveHierarchy;
    }
    get entityID() {
        return 'WfApproveHierarchyID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title: "Nombre", field: "WfApproveHierarchyName", align: "left", valign: "middle", class: "col-nombre" },
            { title: "Área por Defecto", field: "WfApproveHierarchyDepartmentName", align: "left", valign: "middle", class: "col-desc" },
            { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }
    
    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }

    async loadData() {
        this.$presenter.showLoading();
        const filters = BootstrapTableUtils.prepareFilters(this.table);
        this.prepareFilters(filters);

        let request;
        if(this.__options.pagination) {
            request = this.$resource.getPaginate(filters);
        } else {
            request = this.$resource.get(filters);
        }
        await request.then(
            //({data}) => {
            (data) => {
                // if(this.__options.pagination) {
                //     $(this.table).bootstrapTable('load', {
                //         rows: data.Rows.filter(f => f.WfApproveHierarchyID > 0), //this.Entity.fromList(data.Rows || []),
                //         total: data.TotalRows
                //     });
                // } else {
                //     $(this.table).bootstrapTable('load', data.filter(f => f.WfApproveHierarchyID > 0));
                // }
                $(this.table).bootstrapTable('load', data.filter(f => f.WfApproveHierarchyID > 0));
                BootstrapTableUtils.setFilters(this.table, filters);
                this.$presenter.hideLoading();
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: 'Error al cargar' });
            }
        );
    }
}
customElements.define("approvehierarchy-main", ApproveHierarchyComponent);