import { Component, html } from '../core.js';
import { WorkflowResource, Workflow, WorkflowItem } from './resource-workflow.js';
import { WorkflowApproveHierarchyResource } from './resources/resource-approvehierarchy.js';
import { ControlSelect } from './ui-control-select.js';
import { ApproveHierarchyHelper } from './resources/helpers/approvehierarchy-helper.js';


export class WorkflowComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.$entity = new Workflow();
        this.$resource = new WorkflowResource();

        this.addComponent('editWorkflow', new WorkflowEditComponent(presenter));
    }

    __render() {
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
        <h3>Workflows</h3>
        <div id="toolbar">
            <div class="btn-group">
                <button id="btnAdd" class="btn btn-warning" @click=${this.add}>
                    <i class="fas fa-plus"></i> Agregar
                </button>
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
        <div data-component="editWorkflow"></div>`;
    }

    __ready() {
        const $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 130,
            sortName: "WfName",
            columns: [
                { title: "Nombre", field: "WfName", align: "left", valign: "middle" },
                { title: "Descripción", field: "WfDescription", align: "left", valign: "middle" },
                { title: "Jerarquía de Aprobación", field: "WfApproveHierarchyName", align: "left", valign: "middle" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onDblClickRow: (row, element, field) => {
                this.edit(row.WfID);
            },
            onRefresh: () => {
                this.loadData();
            }
        });
        this.loadData();
    }

    loadData() {
        this.$presenter.showLoading();
        this.$resource.get().then(r => {
                this.$$.table.bootstrapTable('load', Workflow.fromList(r));
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                console.error('Error', err);
                iziToast.error({ title: 'Error al cargar' });
            });
    }

    add(e) {
        this.$component.editWorkflow.add();
    }

    edit(WfID) {
        this.$presenter.showLoading();
        this.$resource.get(WfID).then(async r => {
                const entity = Workflow.fromObject(r);
                await this.$component.editWorkflow.edit(entity);
                this.$presenter.hideLoading();
            },
            err => {
                this.$presenter.hideLoading();
                console.error('Error', err);
                iziToast.error({ title: 'Error al cargar' });
            });
    }

    tableFormatterStatus(value, row, index) {
        const className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                </div>
                <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-secondary btn-copy" title="Copiar Workflow"><i class="far fa-copy"></i></button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .btn-copy': $this.btnColumnCopyClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        this.edit(row.WfID);
    }

    btnColumnCopyClick(e, value, row, index) {
        this.$presenter.showLoading();
        this.$resource.copyWorkflow(row.WfID).then(
            r => {
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
                this.loadData();
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: err });
            }
        );
    }
}

class WorkflowEditComponent extends Component {
    constructor(presenter) {
        super(presenter);

        this.$wfHierarchyResource = new WorkflowApproveHierarchyResource();
        this.$wfHierachy = null;
        this.$entity = new Workflow();
        this.addComponent('editDetail', new WorkflowEditDetailComponent(presenter, this));
    }

    __render() {
        return html `
        <style>
            #sectionGraph {
                height: 410px;
            }
        </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${this.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Workflow</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-5" id="sectionGraph">
                                    <div ></div>
                                </div>
                                <div class="col-7">
                                    <div class="row">
                                        <div class="col">
                                            <div class="form-group row">
                                                <label for="txtName" class="col-sm-2 col-form-label">Nombre</label>
                                                <div class="col-sm-10">
                                                    <input type="text" class="form-control" id="txtName" required>
                                                    <div class="invalid-feedback">
                                                        Por favor, ingrese un nombre correcto.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label for="txtDescription" class="col-sm-2 col-form-label">Descripción</label>
                                                <div class="col-sm-10">
                                                    <input type="text" class="form-control" id="txtDescription" .value=${this.$entity.WfDescription} required>
                                                    <div class="invalid-feedback">
                                                        Por favor, ingrese una descripción correcta.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                <label for="cboApproveHierarchy" class="col-sm-2 col-form-label">Jerarquía de Aprobación</label>
                                                <div class="col-sm-10">
                                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                                        <control-select id="cboApproveHierarchy" data-live-search="true" data-size="8" @select=${this.selectApproveHierarchy} required></control-select>
                                                        <div class="input-group-append">
                                                            <!-- <button class="btn btn-outline-secondary" type="button" id="btnApproveHierarchySearch" title="Buscar área">
                                                                <i class="fas fa-search"></i>
                                                            </button> -->
                                                            <!-- <button class="btn btn-outline-secondary" type="button" id="btnApproveHierarchyDelete" @click=${this.btnApproveHierarchyDeleteClick} title="Quitar jerarquía">
                                                                <i class="fas fa-minus"></i>
                                                            </button> -->
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="toolbarDetail">
                                                <div class="btn-group">
                                                    <button id="btnAddDetail" class="btn btn-warning" @click=${this.btnAddDetailClick}>
                                                        <i class="fas fa-plus"></i> Agregar
                                                    </button>
                                                </div>
                                            </div>
                                            <div id="tableDetailWrap">
                                                <table
                                                    id="tableDetail"
                                                    data-locale="es-ES"
                                                    data-toolbar="#toolbarDetail"
                                                    data-reorderable-rows="true"
                                                    >
                                                    <thead>
                                                    </thead>
                                                    <tbody></tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <button type="button" class="btn btn-danger" id="btnDelete" ?data-hidden=${this.$entity.WfID <= 0} @click=${this.delete}><i class="fas fa-eraser"></i> Borrar</button>
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <div data-component="editDetail"></div>`;
    }

    __ready() {
        const $this = this;
        this.$$.tableDetail.bootstrapTable({
            columns: [
                { title: "#", field: "WfItemStep", align: "center", valign: "middle" },
                { title: "Nombre", field: "WfItemName", align: "left", valign: "middle" },
                { title: "Tipo", field: "WfItemTypeName", align: "left", valign: "middle" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onDblClickRow: (row, element, field) => {
                this.$component.editDetail.edit(row);
            },
            // onReorderRowsDrop: (table, droppedRow) => {
            // },
            onReorderRow: (reorderData) => {
                this.$entity.WorkflowItems = reorderData;
                this.$entity.recalcOrder();
                this.updateGraph();
                this.loadDetail();
            }
        });
        
        ApproveHierarchyHelper.fillSelect(this.$.cboApproveHierarchy, true);
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                    <button type="button" class="btn btn-danger btn-delete" title="Eliminar"><i class="fas fa-times"></i></button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        // let entity = new Workflow();
        // entity.fromDTO(row);
        this.$component.editDetail.edit(row);
    }
    btnColumnDeleteClick(e, value, row, index) {
        this.$presenter.question(
            'Borrar',
            'Desea borrar este detalle?',
            (instance, toast) => {
                // let index = this.$entity.WorkflowItems.indexOf(row);
                // this.$entity.WorkflowItems.splice(index, 1);
                this.$entity.deleteItem(row);
                this.updateGraph();
                this.loadDetail();
            }
        );
    }

    selectApproveHierarchy(e) {
        const select = this.$.cboApproveHierarchy;
        
        this.$wfHierachy = null;
        if (select.value != null) {
            this.$wfHierachy = select.data;
        }

        // Esto es para que los valores de las acciones de los items (WfItemActionValue),
        // no se actualicen en la primera asignación (en la carga inicial)
        if (!select.__firstValue) {
            this.$entity.changeApproveHierarchy(select.value);
        }
        select.__firstValue = false;
    }

    // btnApproveHierarchyDeleteClick(e) {
    //     this.$.cboApproveHierarchy.value = null;
    // }

    btnAddDetailClick(e) {
        e.preventDefault();
        let entity = new WorkflowItem();
        this.$component.editDetail.edit(entity);
    }

    loadDetail() {
        this.$$.tableDetail.bootstrapTable('load', this.$entity.WorkflowItems || []);
    }

    updateGraph() {
        let network = new vis.Network(
            this.$.sectionGraph,
            this.$entity.getGraphData(), {
                layout: {
                    randomSeed: undefined,
                    hierarchical: {
                        enabled: false,
                        levelSeparation: 50,
                        nodeSpacing: 80,
                        treeSpacing: 120,
                        blockShifting: true,
                        edgeMinimization: true,
                        parentCentralization: true,
                        direction: 'LR', // UD, DU, LR, RL
                        sortMethod: 'hubsize' // hubsize, directed
                    }
                },
                edges: {
                    smooth: true,
                    arrows: { to: true }
                }
            }
        );
        setTimeout(() => {
            network.redraw();
        }, 500);
    }

    // Actions ------------------------------------------------------------------------- //

    add() {
        this.edit(new Workflow());
    }

    async edit(entity) {
        await ApproveHierarchyHelper.fillSelect(this.$.cboApproveHierarchy, true);
        this.$.cboApproveHierarchy.__firstValue = true;

        this.$entity = entity;
        this.$.txtName.value             = entity.WfName;
        this.$.txtDescription.value      = entity.WfDescription;
        this.$.cboApproveHierarchy.value = entity.WfApproveHierarchyID;
        this.render();
        this.loadDetail();

        this.$$.formModal.modal({ backdrop: 'static' });
        this.$$.tableDetail.bootstrapTable('refreshOptions', {
            height: window.innerHeight - this.$.tableDetail.getBoundingClientRect().top - 45
        });
        this.updateGraph();
        this.$.txtName.select();
    }

    save(e) {
        if(!this.validate(e)) return;

        this.$entity.WfName = this.$.txtName.value.trim();
        this.$entity.WfDescription = this.$.txtDescription.value.trim();
        this.$entity.WfApproveHierarchyID = this.$.cboApproveHierarchy.value;

        this.$presenter.showLoading();
        this.$parent.$resource.post(this.$entity).then(
            r => {
                this.$$.formModal.modal('hide');
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
                this.$parent.loadData();
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: err });
            }
        );
    }

    validate(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };
        if (this.$entity.WorkflowItems.length == 0) {
            iziToast.error({ title: 'No ha ingresado el detalle' });
            return;
        }

        return true;
    }

    delete(e) {
        if (this.$entity.WfID > 0) {
            this.$presenter.question(
                'Borrar',
                'Desea borrar este registro?',
                (instance, toast) => {
                    this.$presenter.showLoading();
                    this.$parent.$resource.delete(this.$entity.WfID).then(
                        r => {
                            this.$$.formModal.modal('hide');
                            this.$presenter.hideLoading();
                            console.log('borrado!');
                            iziToast.success({ title: 'Borrado correctamente' });
                            this.$parent.loadData();
                        },
                        err => {
                            this.$presenter.hideLoading();
                            console.error('error!', err);
                            iziToast.error({ title: err });
                        }
                    );
                }
            );
        }
    }
}


class WorkflowEditDetailComponent extends Component {
    constructor(presenter, parent) {
        super(presenter);
        this.$parent = parent;
        this.$entityDetail = null;
        this.editor = null;
    }

    __render() {
        return html `
        <style>
        .form-group {
            margin-bottom: .4rem;
        }
        </style>
        <div class="modal" id="formModalDetail" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-nested" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="formDetail" @submit=${this.formSubmit}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Acción para Workflow</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="txtDetailName" class="col-sm-2 col-form-label">Nombre</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailName" required minlength="3" maxlength="200">
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un nombre correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="cboDetailType" class="col-sm-2 col-form-label">Tipo</label>
                                <div class="col-sm-2">
                                    <select id="cboDetailType" class="custom-select" required @change=${this.showViewSwitch}>
                                        <option value="39">Acción</option>
                                        <option value="38">Consulta</option>
                                        <option value="37">Notificación</option>
                                    </select>
                                    <div class="invalid-feedback">
                                        Por favor, seleccione un tipo.
                                    </div>
                                </div>
                                <label for="cboDetailDestType" class="col-sm-1 col-form-label" data-view="query notification">Destinatario</label>
                                <div class="col-sm-3" data-view="query notification">
                                    <select id="cboDetailDestType" class="custom-select" required @change=${this.showEmail}>
                                        <option value="1">[aprobador]</option>
                                        <option value="2">[solicitante]</option>
                                        <option value="3">[solicitado_para]</option>
                                        <option value="4">[otro]</option>
                                        <option value="5">[ejecutor]</option>
                                    </select>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un destinatario.
                                    </div>
                                </div>
                                <label for="cboDetailCcType" class="col-sm-1 col-form-label" data-view="query notification">Con copia</label>
                                <div class="col-sm-3" data-view="query notification">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailCcType" class="custom-select"  @change=${this.showEmailCc}>
                                            <option value="1">[aprobador]</option>
                                            <option value="2">[solicitante]</option>
                                            <option value="3">[solicitado_para]</option>
                                            <option value="4">[otro]</option>
                                            <option value="5">[ejecutor]</option>
                                        </select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnCcDelete" @click=${this.btnCcDeleteClick} title="Quitar Cc">
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- <div class="form-group row">
                                
                            </div> -->
                            <div class="form-group row" data-view="action">
                                <label for="cboDetailActionProperty" class="col-sm-2 col-form-label">Propiedad</label>
                                <div class="col-sm-5">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailActionProperty" class="custom-select" @change=${this.showViewActionSwitch}>
                                            <option value="1">Cambiar de Aprobador</option>
                                            <option value="2">Ejecutar en Servicio</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-5">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailActionValue" class="custom-select">
                                            <option value="1">[siguiente_aprobador]</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query notification email">
                                <label for="txtDetailEmail" class="col-sm-2 col-form-label">Email</label>
                                <div class="col-sm-10">
                                    <input type="email" class="form-control" id="txtDetailEmail" required minlength="3" maxlength="200">
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un email.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query notification emailcc">
                                <label for="txtCcEmail" class="col-sm-2 col-form-label">Email Cc</label>
                                <div class="col-sm-10">
                                    <input type="email" class="form-control" id="txtCcEmail" required minlength="3" maxlength="200">
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un email.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query notification">
                                <label for="txtDetailSubject" class="col-sm-2 col-form-label">Asunto</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailSubject" required minlength="10" maxlength="2000" @contextmenu=${this.showContextMenuSubject}>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un asunto correcto, como mínimo debe tener 10 caracteres.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query notification">
                                <label for="txtDetailMessage" class="col-sm-2 col-form-label">Mensaje</label>
                                <div class="col-sm-10">
                                    <textarea id="txtDetailMessage" class="ckeditor"></textarea>
                                </div>
                            </div>
                            <div class="form-group row" data-view="action notification">
                                <label for="cboDetailNextItem" class="col-sm-2 col-form-label">Siguiente</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailNextItem" class="custom-select"></select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnDetailNextItemDelete" title="Quitar" @click=${this.btnDetailNextItemDeleteClick}>
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query">
                                <label for="cboDetailApproveItem" class="col-sm-2 col-form-label">Aprobado</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailApproveItem" class="custom-select"></select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnDetailApproveItemDelete" title="Quitar" @click=${this.btnDetailApproveItemDeleteClick}>
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query">
                                <label for="cboDetailRejectItem" class="col-sm-2 col-form-label">Rechazado</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailRejectItem" class="custom-select"></select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnDetailRejectItemDelete" title="Quitar" @click=${this.btnDetailRejectItemDeleteClick}>
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-view="query">
                                <label for="cboDetailTimeoutItem" class="col-sm-2 col-form-label">Tiempo expirado</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboDetailTimeoutItem" class="custom-select" style="width:70%" @change=${this.cboDetailTimeoutItemChange}></select>
                                        <input type="number" min="0" step="1" class="form-control" id="txtDetailTimeoutItemDueTime" required />
                                        <select id="cboDetailTimeoutItemDueUnits" class="custom-select" required>
                                            <option value="0">minutos</option>
                                            <option value="1">horas</option>
                                            <option value="2">días</option>
                                        </select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnDetailTimeoutItemDelete" title="Quitar" @click=${this.btnDetailTimeoutItemDeleteClick}>
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- <button type="button" class="btn btn-danger" id="btnDetailDelete"><i class="fas fa-eraser"></i> Borrar</button> -->
                            <button type="submit" class="btn btn-success" id="btnDetailAceptar"><i class="fas fa-check"></i> Aceptar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        
        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton" id="contextmenuSubject">
            <button class="dropdown-item" data-value="[[solicitud_numero]]">Número de Solicitud</button>
            <button class="dropdown-item" data-value="[[solicitud_solicitante]]">Solicitante</button>
            <button class="dropdown-item" data-value="[[solicitud_para]]">Solicitado para</button>
            <button class="dropdown-item" data-value="[[solicitud_ticket]]">Ticket Atención</button>
        </div>`;
    }

    __ready() {
        const $this = this;

        // context menu
        $(document.body).on('click', () => {
            $(this.$.contextmenuSubject).removeClass("show").hide();
        });
        $(this.$.txtDetailSubject).on("click", () => {
            $(this.$.contextmenuSubject).removeClass("show").hide();
        });
        $(this.$all('#contextmenuSubject button')).on("click", this.addSubjectValue.bind(this));

        // editor
        CKEDITOR.replace(this.$.txtDetailMessage, {
            height: '120px',
            extraPlugins: 'justify, base64image, font, colorbutton, colordialog, axscontokens',
            removeButtons: 'Subscript,Superscript',
            toolbar: [
                { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline'] },
                { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
                { name: 'others', items: ['base64image', 'Table', 'HorizontalRule', '-', 'Link', 'Unlink'] },
                { name: 'document', items: ['Source'] },
                '/',
                { name: 'format', items: ['Styles', 'Format', 'Font', 'FontSize', 'TextColor', 'BGColor'] },
                { name: 'others1', items: ['axscontokens'] },
                { name: 'others2', items: ['Maximize'] }
            ]
        });
        this.editor = CKEDITOR.instances.txtDetailMessage;
    }


    edit(entity) {
        this.$entityDetail = entity;
        this.fillSelectItems(this.$.cboDetailNextItem);
        this.fillSelectItems(this.$.cboDetailApproveItem);
        this.fillSelectItems(this.$.cboDetailRejectItem);
        this.fillSelectItems(this.$.cboDetailTimeoutItem);

        this.$.txtDetailName.value = entity.WfItemName;
        this.$.cboDetailType.value = entity.WfItemType;
        this.$.cboDetailDestType.value = entity.WfItemDestType;
        this.$.cboDetailActionProperty.value = entity.WfItemActionProperty;
        this.showViewActionSwitch();
        this.$.cboDetailActionValue.value = entity.WfItemActionValue;
        this.$.txtDetailEmail.value = entity.WfItemDestMail;
        this.$.txtDetailSubject.value = entity.WfItemSubject;

        this.editor.setData(entity.WfItemMessage);

        this.$.cboDetailNextItem.value = entity.WfItemNextItem;
        this.$.cboDetailApproveItem.value = entity.WfItemApproveItem;
        this.$.cboDetailRejectItem.value = entity.WfItemRejectItem;
        this.$.cboDetailTimeoutItem.value = entity.WfItemTimeoutItem;
        this.$.txtDetailTimeoutItemDueTime.value = entity.WfItemTimeoutDueTime;
        this.$.cboDetailTimeoutItemDueUnits.value = entity.WfItemTimeoutDueUnits;
        this.$.txtDetailTimeoutItemDueTime.disabled = this.$.cboDetailTimeoutItem.value == "";
        this.$.cboDetailTimeoutItemDueUnits.disabled = this.$.cboDetailTimeoutItem.value == "";
        this.$.btnDetailTimeoutItemDelete.disabled = this.$.cboDetailTimeoutItem.value == "";

        this.$$.formModalDetail.modal({ backdrop: 'static' });
        this.showViewSwitch();
        //Luego que se carga el combo cc, seteamos el valor
        this.$.cboDetailCcType.value = entity.WfItemCcType;
        this.$.txtCcEmail.value = entity.WfItemCcMail != undefined ? entity.WfItemCcMail : "";
        this.showEmailCc();
        this.$.txtDetailName.select();
    }

    showViewSwitch() {
        $(this.$all('[data-view]')).hide();
        let sections = [];
        if (this.$.cboDetailType.value == WorkflowItem.TYPE_ACCION) {
            sections = this.$all('[data-view~=action]');
        } else if (this.$.cboDetailType.value == WorkflowItem.TYPE_CONSULTA) {
            sections = this.$all('[data-view~=query]');
            this.$.cboDetailDestType.value = WorkflowItem.DEST_APROBADOR;
        } else if (this.$.cboDetailType.value == WorkflowItem.TYPE_NOTIFICACION) {
            sections = this.$all('[data-view~=notification]');
        }
        this.setEditorHeight();

        this.$.cboDetailActionValue.required = this.$.cboDetailType.value == WorkflowItem.TYPE_ACCION;
        this.$.txtDetailSubject.required = this.$.cboDetailType.value != WorkflowItem.TYPE_ACCION;
        this.$.cboDetailDestType.required = this.$.cboDetailType.value != WorkflowItem.TYPE_ACCION;
        this.$.cboDetailDestType.disabled = this.$.cboDetailType.value == WorkflowItem.TYPE_CONSULTA;
        
        $(sections).show();
        this.showEmail();
        this.showEmailCc();
    }

    showViewActionSwitch() {
        const action = parseInt(this.$.cboDetailActionProperty.value);
        if(action == 1) {
            this.fillSelectActionApprover();
        } else if(action == 2) {
            this.fillSelectActionExecution();
        }
    }

    showEmail() {
        const section = $(this.$all('[data-view~=email]'));
        if (this.$.cboDetailType.value == WorkflowItem.TYPE_NOTIFICACION && [WorkflowItem.DEST_OTRO, WorkflowItem.DEST_EJECUTOR].includes(parseInt(this.$.cboDetailDestType.value))) {
            section.show();
            this.$.txtDetailEmail.required = true;
            this.$.txtDetailEmail.select();
        } else {
            this.$.txtDetailEmail.required = false;
            section.hide();
        }
        this.setEditorHeight();

        //Carga combo Cc
        var list = [
            { text: '[aprobador]', id: 1 },
            { text: '[solicitante]', id: 2 },
            { text: '[solicitado_para]', id: 3 },
            { text: '[otro]', id: 4 },
            { text: '[ejecutor]', id: 5 }
        ];
        var idDest = this.$.cboDetailDestType.value;
        this.$.cboDetailCcType.options.length = 0;
        list.filter(function(element) { return element.id != (idDest == 4 ? 0 : idDest) }).map(x => {
            var option = document.createElement("option");
            option.value = x.id;
            option.text = x.text;
            this.$.cboDetailCcType.appendChild(option);
        });
        this.$.cboDetailCcType.value = null;
        this.showEmailCc();
    }

    showEmailCc() {
        const section = $(this.$all('[data-view~=emailcc]'));
        if ((this.$.cboDetailType.value == WorkflowItem.TYPE_NOTIFICACION || this.$.cboDetailType.value == WorkflowItem.TYPE_CONSULTA) && [WorkflowItem.DEST_OTRO].includes(parseInt(this.$.cboDetailCcType.value))) {
            section.show();
            this.$.txtCcEmail.required = true;
            this.$.txtCcEmail.select();
        } else {
            this.$.txtCcEmail.required = false;
            section.hide();
        }
        this.setEditorHeight();
    }

    btnCcDeleteClick(e) {
        this.$.cboDetailCcType.value = null;
        this.$.txtCcEmail.value = null;
        this.showEmailCc();

    }

    setEditorHeight() {
        let editor = this.$one('#txtDetailMessage + .cke_chrome .cke_contents');
        if (this.$.cboDetailType.value == WorkflowItem.TYPE_CONSULTA) {
            editor.style.height = '122px';
        } else if (this.$.cboDetailType.value == WorkflowItem.TYPE_NOTIFICACION) {
            if (this.$.cboDetailDestType.value == WorkflowItem.DEST_OTRO)
                editor.style.height = '158px';
            else
                editor.style.height = '195px';
        }
    }

    showContextMenuSubject(e) {
        e.preventDefault();
        let top = e.pageY - 60;
        let left = e.pageX;
        let menu = $(this.$.contextmenuSubject);
        menu.css({
            display: "block",
            top: top,
            left: left,
            'z-index': 9999999
        }).addClass('show');
        this.$.txtDetailSubject.focus();
        return false;
    }

    addSubjectValue(e) {
        this.$.txtDetailSubject.focus();
        let value = e.target.dataset.value;
        let start = this.$.txtDetailSubject.selectionStart;
        let end = this.$.txtDetailSubject.selectionEnd;
        let before = this.$.txtDetailSubject.value.slice(0, start);
        let after = this.$.txtDetailSubject.value.slice(end);
        this.$.txtDetailSubject.value = before + value + after;
        $(this.$.contextmenuSubject).removeClass("show").hide();
        this.$.txtDetailSubject.focus();
    }

    getOtherItems() {
        return this.$parent.$entity.WorkflowItems.filter(x => x != this.$entityDetail);
    }

    fillSelectActionApprover() {
        const select = this.$.cboDetailActionValue;
        select.innerHTML = '';

        // default
        const opt = document.createElement('option');
        opt.innerHTML = '[siguiente_aprobador]';
        opt.value = 1;
        select.appendChild(opt);

        if (this.$parent.$wfHierachy != null) {
            this.$parent.$wfHierachy.WorkflowHierarchyMembers
                .filter(x => x.WfHierarchyMemberOrder > 1)
                .forEach(item => {
                    const opt = document.createElement('option');
                    opt.innerHTML = `[${item.WfHierarchyMemberDepartmentName}] -> ${item.WfHierarchyMemberPositionName}`;
                    opt.value = item.WfHierarchyMemberOrder;
                    opt.$data = item;
                    select.appendChild(opt);
                });
        }
        select.value = null;
    }

    fillSelectActionExecution() {
        const select = this.$.cboDetailActionValue;
        select.innerHTML = '';

        const options = [
            {
                display: 'Ejecutar en ORACLE',
                value: 1
            }, {
                display: 'Ejecutar en SRA',
                value: 2
            }, {
                display: 'Ejecutar en ICARUS',
                value: 3
            }
        ];

        options.forEach(item => {
            const opt = document.createElement('option');
            opt.innerHTML = `${item.display}`;
            opt.value = item.value;
            opt.$data = item;
            select.appendChild(opt);
        });
        select.value = null;
    }

    fillSelectItems(select) {
        select.innerHTML = '';
        this.getOtherItems().forEach(item => {
            let opt = document.createElement('option');
            opt.innerHTML = item.WfItemName;
            opt.value = item.WfItemStep;
            opt.$data = item;
            select.appendChild(opt);
        });
        select.value = null;
    }

    btnDetailNextItemDeleteClick(e) {
        this.$.cboDetailNextItem.value = null;
    }
    btnDetailApproveItemDeleteClick(e) {
        this.$.cboDetailApproveItem.value = null;
    }
    btnDetailRejectItemDeleteClick(e) {
        this.$.cboDetailRejectItem.value = null;
    }
    btnDetailTimeoutItemDeleteClick(e) {
        this.$.cboDetailTimeoutItem.value = null;
        this.$.txtDetailTimeoutItemDueTime.value = 0;
        this.$.cboDetailTimeoutItemDueUnits.value = 2;
        // this.$.txtDetailTimeoutItemDueTime.value = null;
        // this.$.cboDetailTimeoutItemDueUnits.value = null;

        this.$.txtDetailTimeoutItemDueTime.disabled = true;
        this.$.cboDetailTimeoutItemDueUnits.disabled = true;
        this.$.btnDetailTimeoutItemDelete.disabled = true;
    }

    cboDetailTimeoutItemChange(e) {
        this.$.txtDetailTimeoutItemDueTime.disabled = false;
        this.$.cboDetailTimeoutItemDueUnits.disabled = false;
        this.$.btnDetailTimeoutItemDelete.disabled = false;
    }

    formSubmit(e) {
        e.preventDefault();
        // validaciones
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        if (this.$.cboDetailType.value == WorkflowItem.TYPE_CONSULTA && this.$.cboDetailTimeoutItem.value != "") {
            if (this.$.cboDetailTimeoutItem.value != null && this.$.txtDetailTimeoutItemDueTime.value <= 0) {
                iziToast.error({ title: 'No ha ingresado el tiempo de expiración' });
                return;
            }
        }

        // data
        let tmp = new WorkflowItem();
        tmp.WfItemId = this.$entityDetail.WfItemId;
        tmp.WfItemStep = this.$entityDetail.WfItemStep;
        this.$entityDetail.fromDTO(tmp);

        this.$entityDetail.WfItemName = this.$.txtDetailName.value;
        this.$entityDetail.WfItemType = this.$.cboDetailType.value;

        if (this.$entityDetail.WfItemType == WorkflowItem.TYPE_ACCION) {
            this.$entityDetail.WfItemTypeName = 'ACCION';
            this.$entityDetail.WfItemActionProperty = this.$.cboDetailActionProperty.value;
            this.$entityDetail.WfItemActionValue = this.$.cboDetailActionValue.value;
            this.$entityDetail.WfItemNextItem = this.$.cboDetailNextItem.value;
        }
        if (this.$entityDetail.WfItemType == WorkflowItem.TYPE_CONSULTA) {
            this.$entityDetail.WfItemTypeName = 'CONSULTA';
            this.$entityDetail.WfItemDestType = this.$.cboDetailDestType.value;
            this.$entityDetail.WfItemCcType = this.$.cboDetailCcType.value;
            if ([WorkflowItem.DEST_OTRO].includes(parseInt(this.$entityDetail.WfItemCcType)))
                this.$entityDetail.WfItemCcMail = this.$.txtCcEmail.value;
            // this.$entityDetail.WfItemDestMail = this.$.txtDetailEmail.value;
            this.$entityDetail.WfItemSubject = this.$.txtDetailSubject.value.trim();
            this.$entityDetail.WfItemMessage = this.editor.getData();
            this.$entityDetail.WfItemApproveItem = this.$.cboDetailApproveItem.value;
            this.$entityDetail.WfItemRejectItem = this.$.cboDetailRejectItem.value;
            this.$entityDetail.WfItemTimeoutItem = this.$.cboDetailTimeoutItem.value;
            this.$entityDetail.WfItemTimeoutDueTime = this.$.txtDetailTimeoutItemDueTime.value;
            this.$entityDetail.WfItemTimeoutDueUnits = this.$.cboDetailTimeoutItemDueUnits.value;
        }
        if (this.$entityDetail.WfItemType == WorkflowItem.TYPE_NOTIFICACION) {
            this.$entityDetail.WfItemTypeName = 'NOTIFICACION';
            this.$entityDetail.WfItemDestType = this.$.cboDetailDestType.value;
            if ([WorkflowItem.DEST_OTRO, WorkflowItem.DEST_EJECUTOR].includes(parseInt(this.$entityDetail.WfItemDestType)))
                this.$entityDetail.WfItemDestMail = this.$.txtDetailEmail.value;

            this.$entityDetail.WfItemCcType = this.$.cboDetailCcType.value;
            if ([WorkflowItem.DEST_OTRO].includes(parseInt(this.$entityDetail.WfItemCcType)))
                this.$entityDetail.WfItemCcMail = this.$.txtCcEmail.value;

            this.$entityDetail.WfItemSubject = this.$.txtDetailSubject.value.trim();
            this.$entityDetail.WfItemMessage = this.editor.getData();
            this.$entityDetail.WfItemNextItem = this.$.cboDetailNextItem.value;
        }

        this.$parent.$entity.addItem(this.$entityDetail);
        this.$parent.updateGraph();
        this.$parent.loadDetail();
        this.$$.formModalDetail.modal('hide');
    }
}


export class SelectWorkflow extends ControlSelect {
    constructor(select) {
        super(select, {
            noneSelectedText: 'Seleccione workflow'
        });
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
    }

    refresh() {
        return WorkflowResource.loadSelect(this.__select).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
    refreshFromData(data) {
        // let select = this.__select;
        // select.innerHTML = '';
        // data.forEach(item => {
        //     let opt = document.createElement('option');
        //     opt.innerHTML = item.WfItemName;
        //     opt.value = item.WfID;
        //     opt.$data = item;
        //     select.appendChild(opt);
        // });
        // select.value = null;
    }
}