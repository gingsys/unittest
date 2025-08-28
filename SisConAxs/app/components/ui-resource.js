import { Component, html } from '../core.js';
import { ResourceResource, Resource, ResourceParameter } from './resource-resource.js';
import { CommonValueResource, CommonValueSet, CommonValue } from './resources/resource-commonvalue.js';
import { ControlSelect } from './ui-control-select.js';
import { SelectCommonValue } from './ui/commonvalues/ui-commonvalue.js';
import { SelectCategory } from './ui/categories/ui-category.js';
import { SelectWorkflow } from './ui-workflow.js';
import { Metadata, MetadataResource } from './resource-metadata.js';
import { SelectMetadata } from './ui-metadata.js';
import { AccessTypeHelper } from './resources/helpers/accesstype-helper.js';


export class ResourceComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.$data = [];
        this.$optResourceData = [];
        this.$entity = new Resource();
        this.$resource = new ResourceResource();

        this.filters = {
            ResourceActive: 1,
            showResourceWithoutWorkflow: 1
        };

        this.addComponent('editResource', new ResourceEditComponent(presenter, this));
        this.addComponent('editMultipleResource', new ResourceEditMultipleComponent(presenter, this));        
    }

    __render() {        
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
        <h3>Recursos</h3>
        <div id="toolbar">
            <div class="btn-group">
                <button id="btnAdd" class="btn btn-warning" @click="${this.btnAddClick}">
                    <i class="fas fa-plus"></i> Agregar
                </button>
                <button id="btnEdit" class="btn btn-secondary" @click="${this.btnEditClick}">
                    <i class="fas fa-edit"></i> Editar
                </button>
            </div>
            <div class="btn-group" data-toggle="buttons">
                <button class="btn btn-outline-success active" id="optFilterStatusActive" @click="${this.optFilterStatusActiveClick}">
                    <i class="fas fa-check"></i> Activos <input type="radio" name="optFilterStatus" value="1" autocomplete="off" checked style="display: none;" />
                </button>
                <button class="btn btn-outline-secondary" id="optFilterStatusDeactive" @click="${this.optFilterStatusDeactiveClick}">
                    <i class="fas fa-minus-circle"></i> Inactivos <input type="radio" name="optFilterStatus" value="0" autocomplete="off" checked style="display: none;" />
                </button>
                <button class="btn btn-outline-dark" id="optFilterStatusAll" @click="${this.optFilterStatusAllClick}">
                    <i class="fas fa-border-all"></i> Todos <input type="radio" name="optFilterStatus" value="-1" autocomplete="off" style="display: none;" />
                </button>
            </div>
            <div class="btn-group">
                ${this.$data.some(r => r.ResourceWorkflow == null) ?
                html`<div class="alert alert-danger m-0 px-3 p-2" role="alert">
                            <i class="fas fa-exclamation-triangle"></i> Tiene recursos sin workflow
                        </div>` : ''}
            </div>
        </div>
        <table
            id="table"
            data-locale="es-ES"
            data-toolbar="#toolbar"
            data-search="false"
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
        <div data-component="editMultipleResource"></div>
        <div data-component="editResource"></div>`;
    }

    __ready() {        
        let $this = this;        
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 130,
            filterControl: true,
            searchOnEnterKey: true,
            columns: [
                { checkbox: true },
                { title: "Categoría", field: "ResourceCategoryName", filterControl: "input", filterControlPlaceholder: "Categoría", align: "left", valign: "middle" },
                { title: "Nombre", field: "ResourceName", filterControl: "input", filterControlPlaceholder: "Nombre", align: "left", valign: "middle" },
                { title: "Ruta", field: "ResourceFullName", align: "left", filterControl: "input", filterControlPlaceholder: "Ruta", valign: "middle" },
                { title: "Tipo Acceso", field: "ResourceAccessTypeName", filterControl: "input", filterControlPlaceholder: "Tipo Acceso", align: "center", valign: "middle" },
                { title: "Workflow", field: "ResourceWorkflowName", filterControl: "input", filterControlPlaceholder: "Workflow", align: "center", valign: "middle" },
                { title: "Activo", field: "ResourceActive", align: "center", valign: "middle", formatter: $this.tableFormatterStatus },
                // { title:"Estado", field:"Estado", filterControl: "input", filterControlPlaceholder: "Número",  align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            rowStyle: (row, index) => {
                if (row.ResourceWorkflow == null) {
                    return {
                        classes: ['table-danger']
                    };
                }
                return {};
            },
            onDblClickRow: (row, element, field) => {
               
                this.edit(row.ResourceID);
            },
            onColumnSearch: function (filterColumn, textColumn) {
                //debugger
                $this.loadData();
            },
            onRefresh: function () {
                //$this.loadData();
                $this.cleanFilters()                
            }
        });
        this.loadData();
        this.loadOptResourceData();
    }

    loadDataOriginal() {
        this.$presenter.showLoading();

        this.$resource.get().then(r => {
           
            this.$data = Resource.fromList(r);            
            this.$$.table.bootstrapTable('load', this.$data.filter(x => this.filters.ResourceActive < 0 || x.ResourceWorkflow == null || x.ResourceActive == this.filters.ResourceActive));
            this.render();
            this.$presenter.hideLoading();
            iziToast.success({ title: 'Cargado correctamente' });
        },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err });
            });
    }

    loadOptResourceData() {        
        const filters = this.prepareFilters();
        var params = Object.assign({
        }, filters);

        this.$resource.get(params).then(
            r => {
                this.$optResourceData = Resource.fromList(r);                  
            },
            err => {                
            }
        );
    }


    loadData() {
        
        this.$presenter.showLoading();
        const filters = this.prepareFilters();
        var params = Object.assign({
        }, filters);

        this.$resource.get(params).then(
            r => {
                this.$data = Resource.fromList(r);                
                this.$$.table.bootstrapTable('load', this.$data);
                this.setFilters(filters);
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: 'Error al cargar' });
            }
        );
    }

    setFilters(filters) {
        for (let field in filters) {
            let control = this.$one(`th[data-field] input[type="text"].bootstrap-table-filter-control-${field}`);
            if (!!control) {
                control.value = filters[field];
                control.addEventListener('click', function (e) {
                    e.target.focus();
                    e.target.select();
                });
            }
        }
    }

    prepareFilters() {
        
        const filters = {};
        const options = this.$$.table.bootstrapTable('getOptions');
        options.valuesFilterControl.filter(v => v.value.trim() != '')
            .forEach(v => {
                filters[v.field] = v.value;
            });
        filters.CurrentPage = options.pageNumber;
        filters.PageSize = options.pageSize;
        filters.ResourceActive = this.filters.ResourceActive;
        return filters;
    }

    async cleanFilters() {
        await this.$$.table.bootstrapTable('clearFilterControl');
        await this.loadData();
    }

    async btnAddClick(e) {
        this.$presenter.showLoading();
        this.$component.editResource.add();
    }
    //SELECCIÓN MÚLTIPLE
    async btnEditClick(e) {
        
        let selected = this.$$.table.bootstrapTable('getSelections');
        console.warn(selected);
        if (selected.length == 0) {
            iziToast.error({ title: 'No ha seleccionado un registro.' });
        } else if (selected.length == 1) {
            this.edit(selected[0].ResourceID);
        } else {
            this.$component.editMultipleResource.edit(selected);
        }
    }

    optFilterStatusActiveClick() {
        this.filters.ResourceActive = 1;
        this.loadData();
    }

    optFilterStatusDeactiveClick() {
        this.filters.ResourceActive = 0;
        this.loadData();
    }

    optFilterStatusAllClick() {
        this.filters.ResourceActive = -1;
        this.loadData();
    }

    edit(ResourceID) {
      
        this.$presenter.showLoading();
        this.$resource.get(ResourceID).then(r => {
            let entity = new Resource();
            entity.fromDTO(r[0]);
            this.$component.editResource.edit(entity);
        },
            err => {
                this.$presenter.hideLoading();
                console.error('Error', err);
                iziToast.error({ title: 'Error al cargar' });
            });
    }

    tableFormatterCheck(value, row, index) {

        return `<input type="checkbox" ${!!value ? 'checked' : ''} disabled></div>`;
    }

    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }

    tableFormatterOperate(value, row, index) {
        
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
     
        let _self = this;
        return {
            'click .btn-edit': _self.btnColumnEditClick.bind(_self)
        }
    }
    btnColumnEditClick(e, value, row, index) {
     
        this.edit(row.ResourceID);
    }
}


class ResourceEditMultipleComponent extends Component {
   
    constructor(presenter, parent) {
        super(presenter);
        this.$resource = new ResourceResource();        

        this.values = {
            ResourceActive: null
        }
    }

    __render() {
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit="${this.save}">
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Recurso Edición Múltiple</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <div class="form-group row">
                                <label for="cboResourceParent" class="col-sm-2 col-form-label">Recurso padre</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboResourceParent" class="custom-bootstrapselect" data-live-search="true" data-size="8"></select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnParentResourceDelete" title="Quitar recurso padre" @click="${this.deleteResourceParent}">
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <!-- <label for="cboResourceCategory" class="col-sm-2 col-form-label">Categoría</label>
                                <div class="col-sm-4">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboResourceCategory" class="custom-bootstrapselect" data-live-search="true" data-size="8" required></select>   
                                        <div class="invalid-feedback">
                                            Por favor, ingrese Categoría.
                                        </div> 
                                    </div>
                                </div> -->
                            </div>
                            <div class="form-group row">
                                <label for="cboResourceAccessType" class="col-sm-2 col-form-label">Acceso</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <control-select id="cboResourceAccessType" data-live-search="true" data-size="8"></control-select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnParentResourceDelete" title="Quitar tipo de acceso" @click="${this.deleteAccessType}">
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="cboResourceWorkflow" class="col-sm-2 col-form-label">Workflow</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboResourceWorkflow" class="custom-bootstrapselect" data-live-search="true" data-size="8"></select>    
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnParentResourceDelete" title="Quitar workflow" @click="${this.deleteWorkflow}">
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="btn-group offset-sm-2 col-sm-5" data-toggle="buttons">
                                    <button type="button" class="btn btn-outline-success" id="optMultipleStatusActive" @click="${this.optMultipleStatusActiveClick}">
                                        <i class="fas fa-check"></i> Activo <input type="radio" name="optMultipleStatus" value="1" autocomplete="off" checked style="display: none;" />
                                    </button>
                                    <button type="button" class="btn btn-outline-secondary" id="optMultipleStatusDeactive" @click="${this.optMultipleStatusDeactiveClick}">
                                        <i class="fas fa-minus-circle"></i> Inactivo <input type="radio" name="optMultipleStatus" value="0" autocomplete="off" checked style="display: none;" />
                                    </button>
                                </div>
                                <button type="button" class="btn btn-outline-dark" id="optMultipleStatusNone" @click="${this.clearActiveValue}" title="Quitar activo">
                                    <i class="fas fa-minus"></i> <input type="radio" name="optMultipleStatus" value="-1" autocomplete="off" checked style="display: none;" />
                                </button>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }

    __ready() {
        const $this = this;
        this.selectResourceParent = new SelectResource(this.$.cboResourceParent);
        // this.selectResourceCategory = new SelectCategory(this.$.cboResourceCategory);
        AccessTypeHelper.fillSelect(this.$.cboResourceAccessType);
        // this.selectResourceDepartment = new SelectCommonValue(this.$.cboResourceDepartment, CommonValueSet.SET_AREAS);
        // this.selectResourceRequired = new SelectResource(this.$.cboResourceRequired);
        this.selectResourceWorkflow = new SelectWorkflow(this.$.cboResourceWorkflow);
    }

    deleteResourceParent() {
        this.selectResourceParent.value = null;
    }
    deleteAccessType() {
        this.$.cboResourceAccessType.value = null;
    }
    deleteWorkflow() {
        this.selectResourceWorkflow.value = null;
    }

    optMultipleStatusActiveClick(e) {
        this.values.ResourceActive = 1;
    }
    optMultipleStatusDeactiveClick(e) {
        this.values.ResourceActive = 0;
    }
    clearActiveValue() {
       
        this.$.optMultipleStatusActive.classList.remove('active');
        this.$.optMultipleStatusDeactive.classList.remove('active');
        this.values.ResourceActive = null;
    }

    async edit(list) {
        
        this.$list = list;
        this.clearActiveValue();

        this.$presenter.showLoading();
        // await this.selectResourceCategory.refresh();
        await AccessTypeHelper.fillSelect(this.$.cboResourceAccessType);
        // await this.selectResourceDepartment.refresh();
        await this.selectResourceWorkflow.refresh();

        let filteredResources = this.$parent.$optResourceData.filter(r => r.ResourceWorkflow != null);  // Se quita la referencia a los recursos sin workflow
        this.selectResourceParent.refreshFromData(filteredResources);
        // this.selectResourceRequired.refreshFromData(filteredResources);

        this.selectResourceParent.value = null;
        // this.selectResourceCategory.value = entity.ResourceCategory;
        this.$.cboResourceAccessType.value = null;
        this.selectResourceWorkflow.value = null;

        this.$$.formModal.modal({ backdrop: 'static' });
        this.$presenter.hideLoading();
    }

    save(e) {
        
        e.preventDefault();
        if (this.selectResourceParent.value == null &&
            this.$.cboResourceAccessType.value == null &&
            this.selectResourceWorkflow.value == null &&
            this.values.ResourceActive == null
        ) {
            iziToast.error({ title: 'No ha seleccionado algún valor.' });
            return;
        }


        this.$presenter.question(
            'Edición múltiple',
            'Desea modificar los registros seleccionados?',
            (instance, toast) => {
                this.$presenter.showLoading();
                this.$resource.saveMultiple({
                    Resources: this.$list.map(r => r.ResourceID),
                    ResourceParent: this.selectResourceParent.value,
                    ResourceAccessType: this.$.cboResourceAccessType.value,
                    ResourceWorkflow: this.selectResourceWorkflow.value,
                    ResourceActive: this.values.ResourceActive
                }).then(r => {
                    this.$$.formModal.modal('hide');
                    this.$presenter.hideLoading();
                    console.log('grabado!');
                    iziToast.success({ title: 'Grabado correctamente' });
                    //this.$parent.loadData();
                    this.$parent.cleanFilters();
                },
                    err => {
                        this.$presenter.hideLoading();
                        console.error('error!', err);
                        iziToast.error({ title: err });
                    });
            }
        );
    }
}


class ResourceEditComponent extends Component {
    constructor(presenter, parent) {
        super(presenter);
        this.$parent = parent;
        this.$resource = new ResourceResource();

        this.selectResourceDepartment = null;
        this.addComponent('parameterSelectorComponent', new ParameterSelectorComponent(presenter, this));
    }

    __render() {
        return html`<div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit="${this.save}">
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Recurso</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <ul class="nav nav-tabs" id="myTab" role="tablist">
                                <li class="nav-item">
                                    <a class="nav-link active" id="tabData" data-toggle="tab" href="#home" role="tab" aria-controls="home" aria-selected="true"><i class="fab fa-wpforms"></i> Datos</a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" id="tabParams" data-toggle="tab" href="#profile" role="tab" aria-controls="profile" aria-selected="false"><i class="fas fa-th-list"></i> Parámetros</a>
                                </li>
                            </ul>
                            <div class="tab-content" id="myTabContent">
                                <div class="tab-pane fade show active pt-3" id="home" role="tabpanel" aria-labelledby="tabData">
                                
                                    <div class="form-group row">
                                        <label for="cboResourceParent" class="col-sm-2 col-form-label">Recurso padre</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceParent" class="custom-bootstrapselect" data-live-search="true" data-size="8"></select>
                                                <!-- <input type="text" class="form-control" id="txtResourceParentName" /> -->
                                                <div class="input-group-append">
                                                    <!-- <button class="btn btn-outline-secondary" type="button" id="btnParentResourceSearch" title="Buscar recurso padre">
                                                        <i class="fas fa-search"></i>
                                                    </button> -->
                                                    <button class="btn btn-outline-secondary" type="button" id="btnParentResourceDelete" title="Quitar recurso padre" @click="${this.btnParentResourceDeleteClick}">
                                                        <i class="fas fa-minus"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <label for="cboResourceCategory" class="col-sm-2 col-form-label">Categoría</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceCategory" class="custom-bootstrapselect" data-live-search="true" data-size="8" required></select>   
                                                <div class="invalid-feedback">
                                                    Por favor, ingrese Categoría.
                                                </div> 
                                                <!-- <input type="text" class="form-control" id="txtResourceCategoryName" required /> 
                                                <div class="input-group-append">
                                                    <button class="btn btn-outline-secondary" type="button" id="btnCategorySearch" title="Buscar categoría">
                                                        <i class="fas fa-search"></i>
                                                    </button>
                                                </div> -->
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtResourceName" class="col-sm-2 col-form-label">Nombre</label>
                                        <div class="col-sm-4">
                                            <input type="text" class="form-control" id="txtResourceName"  minlength="3" maxlength="200" required />
                                            <div class="invalid-feedback">
                                                Por favor, ingrese Nombre.
                                            </div> 
                                        </div>
                                        <label for="txtResourceDescription" class="col-sm-2 col-form-label">Descripción</label>
                                        <div class="col-sm-4">
                                            <input type="text" class="form-control" id="txtResourceDescription" maxlength="2000" />
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="cboResourceAccessType" class="col-sm-2 col-form-label">Acceso</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <control-select id="cboResourceAccessType" data-live-search="true" data-size="8" required></control-select>
                                                <!-- <input type="text" class="form-control" id="txtResourceAccessTypeName" required />
                                                <div class="input-group-append">
                                                    <button class="btn btn-outline-secondary" type="button" id="btnAccessTypeSearch" title="Buscar tipo acceso">
                                                        <i class="fas fa-search"></i>
                                                    </button>
                                                </div> -->
                                            </div>
                                        </div>
                                        <label for="cboResourceWorkflow" class="col-sm-2 col-form-label">Workflow</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceWorkflow" class="custom-bootstrapselect" data-live-search="true" data-size="8" required></select>    
                                                <!-- <input type="text" class="form-control" id="txtResourceWorkflowName" required />
                                                <div class="input-group-append">
                                                    <button class="btn btn-outline-secondary" type="button" id="btnWorkflowSearch" title="Buscar workflow">
                                                        <i class="fas fa-search"></i>
                                                    </button>
                                                </div> -->
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="cboResourceRequired" class="col-sm-2 col-form-label">Recurso Obligatorio</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceRequired" class="custom-bootstrapselect" data-live-search="true" data-size="8"></select>
                                                <!-- <input type="text" class="form-control" id="txtResourceRequiredName" /> -->
                                                <div class="input-group-append">
                                                    <!-- <button class="btn btn-outline-secondary" type="button" id="btnRequiredResourceSearch" title="Buscar recurso">
                                                        <i class="fas fa-search"></i>
                                                    </button> -->
                                                    <button class="btn btn-outline-secondary" type="button" id="btnRequiredResourceDelete" title="Quitar recurso" @click="${this.btnRequiredResourceDeleteClick}">
                                                        <i class="fas fa-minus"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                        <label for="cboResourceDepartment" class="col-sm-2 col-form-label">Área/Proyecto</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceDepartment" class="custom-bootstrapselect" data-none-selected-text="Seleccione área" data-live-search="true" data-size="8"></select>
                                                <div class="input-group-append">
                                                    <!-- <button class="btn btn-outline-secondary" type="button" id="btnAreaProjectSearch" title="Buscar área/proyecto">
                                                        <i class="fas fa-search"></i>
                                                    </button> -->
                                                    <button class="btn btn-outline-secondary" type="button" id="btnAreaProjectDelete" title="Quitar área/proyecto" @click="${this.btnAreaProjectDeleteClick}">
                                                        <i class="fas fa-minus"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="cboResourceTemporal" class="col-sm-2 col-form-label">Temporal</label>
                                        <div class="col-sm-4">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboResourceTemporal" class="custom-select" required>
                                                    <option value="0">No</option>
                                                    <option value="1">Obligatorio</option>
                                                    <option value="2">Opcional</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="form-check offset-md-2" style="padding-left: 32px;">
                                            <input class="form-check-input" type="checkbox" id="chkResourceActive">
                                            <label class="form-check-label" for="chkResourceActive">
                                                Activo
                                            </label>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="form-check offset-md-2" style="padding-left: 32px;">
                                            <input class="form-check-input" type="checkbox" id="chkResourceSendAtEnd">
                                            <label class="form-check-label" for="chkResourceSendAtEnd">
                                                Envía al ejecutor al finalizar
                                            </label>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <div class="form-check offset-md-2" style="padding-left: 32px;">
                                            <input class="form-check-input" type="checkbox" id="chkResourceOnlyAssignable">
                                            <label class="form-check-label" for="chkResourceOnlyAssignable">
                                                Sólo asignable una vez
                                            </label>
                                        </div>
                                    </div>

                                </div>
                                <div class="tab-pane fade pt-3" id="profile" role="tabpanel" aria-labelledby="tabParams">
                                    <div id="toolbarTableParams">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="btn-toolbar mb-3" role="toolbar" aria-label="Toolbar with button groups">
                                                    <!-- <div class="input-group mr-2">
                                                        <select class="custom-select" id="cboParam"></select>
                                                        <input id="txtParam" type="text" class="form-control" placeholder="Valor parámetro">
                                                        <div class="input-group-append">
                                                            <button class="btn btn-outline-secondary" type="button" title="Borrar" @click=${this.clearInputParam}>
                                                                <i class="fas fa-eraser"></i>
                                                            </button>
                                                        </div>
                                                    </div> -->
                                                    <div class="btn-group" role="group">
                                                        <button id="btnAddParam" class="btn btn-warning" @click=${this.addParam}>
                                                            <i class="fas fa-plus"></i> Agregar
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <table
                                        id="tableParams"
                                        data-toolbar="#toolbarTableParams"
                                        data-locale="es-ES"
                                        >
                                        <thead>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <button type="button" class="btn btn-danger" id="btnDelete" @click="${this.delete}"><i class="fas fa-eraser"></i> Borrar</button>
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <div data-component="parameterSelectorComponent"></div>`;
    }

    __ready() {
        let $this = this;
        this.$component.parameterSelectorComponent.onSelect = this.onSelectParam.bind(this);
        this.$$.tabData.on('shown.bs.tab', (e) => {
            // 
        });
        this.$$.tabParams.on('shown.bs.tab', (e) => {
            this.loadTable();
        });

        this.selectResourceParent = new SelectResource(this.$.cboResourceParent);
        this.selectResourceCategory = new SelectCategory(this.$.cboResourceCategory);
        AccessTypeHelper.fillSelect(this.$.cboResourceAccessType);
        this.selectResourceDepartment = new SelectCommonValue(this.$.cboResourceDepartment, CommonValueSet.SET_AREAS);
        this.selectResourceRequired = new SelectResource(this.$.cboResourceRequired);
        this.selectResourceWorkflow = new SelectWorkflow(this.$.cboResourceWorkflow);

        // table
        this.$$.tableParams.bootstrapTable({
            data: [],
            height: window.innerHeight - 295,
            columns: [
                { title: "Parámetro", field: "ResourceParameterDisplay", align: "left", valign: "middle", width: 250 },
                { title: "Etiqueta", field: "ValueDisplay", align: "left", valign: "middle", width: 250 },
                { title: "Valor", field: "Value", align: "left", valign: "middle", width: 250 },
                { title: "", field: "", align: "center", valign: "middle", width: 90, formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ]
        });
    }

    btnParentResourceDeleteClick(e) {
        this.selectResourceParent.value = null;
    }

    btnAreaProjectDeleteClick(e) {
        this.selectResourceDepartment.value = null;
    }

    btnRequiredResourceDeleteClick(e) {
        this.selectResourceRequired.value = null;
    }

    add() {
        this.edit(new Resource());
    }

    async edit(entity) {
       
        await this.selectResourceCategory.refresh();
        await AccessTypeHelper.fillSelect(this.$.cboResourceAccessType);
        await this.selectResourceDepartment.refresh();
        await this.selectResourceWorkflow.refresh();
        
        let filteredResources = this.$parent.$optResourceData.filter(r => !(r.ResourceID == entity.ResourceID || r.ResourceWorkflow == null));  // Se quita la referencia a sí mismo y los recursos sin workflow
        this.selectResourceParent.refreshFromData(filteredResources);
        this.selectResourceRequired.refreshFromData(filteredResources);

        this.$entity = entity;
        this.selectResourceParent.value = entity.ResourceParent;
        // this.$.txtResourceParentName.value = entity.ResourceParentName;
        this.selectResourceCategory.value = entity.ResourceCategory;
        // this.$.txtResourceCategoryName.value = entity.ResourceCategoryName;
        this.$.txtResourceName.value = entity.ResourceName;
        this.$.txtResourceDescription.value = entity.ResourceDescription;

        this.$.cboResourceAccessType.value = entity.ResourceAccessType;

        //deshabilitar btn que contiene select de tipo acceso
        let buttonElement = this.$.cboResourceAccessType.parentNode.querySelector("button.btn.dropdown-toggle")
        buttonElement.classList.add('disabled');
        //buttonElement.style.backgroundColor = '#e9ecef';

        // this.$.txtResourceAccessTypeName.value = entity.ResourceAccessTypeName;
        this.selectResourceWorkflow.value = entity.ResourceWorkflow;
        // this.$.txtResourceWorkflowName.value = entity.ResourceWorkflowName;
        this.selectResourceDepartment.value = entity.ResourceDepartment;
        this.selectResourceRequired.value = entity.ResourceRequired;
        // this.$.txtResourceRequiredName.value = entity.ResourceRequiredName;
        this.$.chkResourceActive.checked = entity.ResourceActive;
        this.$.chkResourceSendAtEnd.checked = entity.ResourceSendAtEnd;
        this.$.cboResourceTemporal.value = entity.ResourceTemporal;
        this.$.chkResourceOnlyAssignable.checked = entity.ResourceOnlyAssignable;

        this.$$.tabData.tab('show');

        this.$$.btnDelete.show();
        if (entity.ResourceID <= 0) {
            buttonElement.classList.remove('disabled');
            this.$$.btnDelete.hide();
        }

        this.$$.formModal.modal({ backdrop: 'static' });
        this.$.txtResourceName.select();
        this.$presenter.hideLoading();
    }

    save(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        this.$entity.ResourceParent = this.selectResourceParent.value;
        // this.$entity.ResourceParentName = this.$.txtResourceParentName.value.trim();
        this.$entity.ResourceCategory = this.selectResourceCategory.value;
        // this.$entity.ResourceCategoryName = this.$.txtResourceCategoryName.value.trim();
        this.$entity.ResourceName = this.$.txtResourceName.value.trim();
        this.$entity.ResourceDescription = this.$.txtResourceDescription.value.trim();
        this.$entity.ResourceAccessType = this.$.cboResourceAccessType.value;
        // this.$entity.ResourceAccessTypeName = this.$.txtResourceAccessTypeName.value.trim();
        this.$entity.ResourceWorkflow = this.selectResourceWorkflow.value;
        // this.$entity.ResourceWorkflowName = this.$.txtResourceWorkflowName.value.trim();
        this.$entity.ResourceDepartment = this.selectResourceDepartment.value;
        this.$entity.ResourceRequired = this.selectResourceRequired.value;
        // this.$entity.ResourceRequiredName = this.$.txtResourceRequiredName.value.trim();
        this.$entity.ResourceActive = this.$.chkResourceActive.checked ? 1 : 0;
        this.$entity.ResourceSendAtEnd = this.$.chkResourceSendAtEnd.checked ? 1 : 0;
        this.$entity.ResourceTemporal = this.$.cboResourceTemporal.value;
        this.$entity.ResourceOnlyAssignable = this.$.chkResourceOnlyAssignable.checked ? 1 : 0;

        this.$presenter.showLoading();
        this.$parent.$resource.post(this.$entity).then(
            r => {
                this.$$.formModal.modal('hide');
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
                //this.$parent.loadData();
                this.$parent.cleanFilters()
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: err }); //'Error al grabar'});
            }
        );
    }

    delete(e) {
        if (this.$entity.ResourceID > 0) {
            this.$presenter.question(
                'Borrar',
                'Desea borrar este registro?',
                (instance, toast) => {
                    this.$presenter.showLoading();
                    this.$parent.$resource.delete(this.$entity.ResourceID).then(
                        r => {
                            this.$$.formModal.modal('hide');
                            this.$presenter.hideLoading();
                            console.log('borrado!');
                            iziToast.success({ title: 'Borrado correctamente' });
                            //this.$parent.loadData();
                            this.$parent.cleanFilters();
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

    // parameters -------------------------------------------------------- //
    addParam(e) {
        e.preventDefault();
        this.$component.parameterSelectorComponent.show();
    }

    onSelectParam(selectedParam, data) {
        if (this.$entity.ResourceParameters.some(x => x.ResourceParameterID == selectedParam.CommonValueID)) {
            iziToast.error({ title: 'Ya tiene este parámetro.' });
            return;
        }

        let param = new ResourceParameter();
        param.ResourceParameterID = selectedParam.CommonValueID;
        param.ResourceParameterDisplay = selectedParam.CommonValueDisplay;
        param.ResourceParameterMetadataID = data.MetadataID;
        param.ValueDisplay = data.MetadataDisplay;
        param.Value = data.MetadataValue;
        this.$entity.ResourceParameters.push(param);
        this.loadTable();
    }

    deleteParam(row) {
        let index = this.$entity.ResourceParameters.indexOf(row);
        this.$entity.ResourceParameters.splice(index, 1);
        this.loadTable();
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-danger btn-delete" title="Eliminar">
                        <i class="fas fa-times"></i> Eliminar
                    </button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let $this = this;
        return {
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }

    btnColumnDeleteClick(e, value, row, index) {
        this.deleteParam(row);
    }

    loadTable() {
        this.$$.tableParams.bootstrapTable('load', this.$entity.ResourceParameters);
    }
}


export class SelectResource extends ControlSelect {
    constructor(select) {
        super(select, {
            noneSelectedText: 'Seleccione recurso'
        });
    }

    refresh() {
        return ResourceResource.loadSelect(this.__select).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
    refreshFromData(data) {
        const select = this.__select;
        select.innerHTML = '';
        data.forEach(item => {
            let opt = document.createElement('option');
            //opt.innerHTML = item.ResourceName;
            opt.value = item.ResourceID;
            opt.title = item.ResourceName;
            opt.dataset.content = `<span class='badge badge-success'>${item.ResourceCategoryName}</span>
                                   ${item.ResourceName} <i class="fas fa-angle-right"></i><small>${item.ResourceFullName}</small>`;
            opt.$data = item;
            select.appendChild(opt);
        });
        // select.value = null;
        select.value = this.value;
        $(select).selectpicker('refresh');
    }
}



export class TreeSelectResource extends React.Component {
    static get SELECTED_THRESHOLD() { return 0 };

    constructor(props = {}) {
        super();
        this.state = {
            value: [],
            data: [],
            treeData: [],
            filter: ''
        };
        this.onChange = this.onChange.bind(this);
        this.onSelect = this.onSelect.bind(this);
        this.onSearch = this.onSearch.bind(this);
        this.filterTreeNode = this.filterTreeNode.bind(this);
        this.clearSelected = this.clearSelected.bind(this);
        this.clearFilter = this.clearFilter.bind(this);
    }

    onChange(value) {
        // console.log("onChange ", value);
        this.setState({ value });
    };
    onSelect(value, node, extra) {
        // this.setState({ value });
        // console.log("onSelect ", value, node, extra);
        this.setState({ nodes: node });
    }
    onSearch(value) {
        this.setState({ searchValue: value });
    }
    filterTreeNode(inputValue, treeNode) {
        if (inputValue.length == 0) return true;
        if (inputValue.length >= 3) {
            inputValue = this.__removeAccents(inputValue).trim().toUpperCase();
            let nodeValue = this.__removeAccents(String(treeNode.props.$data.ResourceFullName).trim().toUpperCase()); //treeNode.props['title']).trim().toUpperCase());
            // return nodeValue.indexOf(inputValue) !== -1;

            return inputValue.split(' ').reduce((prev, curr, index) => {
                return prev && (new RegExp(curr, 'ig')).test(nodeValue);
            }, true);
        }
    }

    __getTree(data, parentID = null) {
        return data.filter(x => x.ResourceParent == parentID).map(x => {
            let children = this.__getTree(data, x.ResourceID);
            x.__haveChildren = children.length > 0;
            return {
                title: x.ResourceName,
                value: x.ResourceID,
                key: x.ResourceID,
                $data: x,
                children: children
            }
        });
    }
    __removeAccents(string) {
        const accents = "ÀÁÂÃÄÅĄàáâãäåąßÒÓÔÕÕÖØÓòóôõöøóÈÉÊËĘèéêëęðÇĆçćÐÌÍÎÏìíîïÙÚÛÜùúûüÑŃñńŠŚšśŸÿýŽŻŹžżź";
        const accentsOut = "AAAAAAAaaaaaaaBOOOOOOOOoooooooEEEEEeeeeeeCCccDIIIIiiiiUUUUuuuuNNnnSSssYyyZZZzzz";
        return string
            .split("")
            .map(letter => {
                const accentIndex = accents.indexOf(letter);
                return accentIndex !== -1 ? accentsOut[accentIndex] : letter;
            })
            .join("");
    }

    setData(data) {
        this.setState({
            data: data,
            treeData: this.__getTree(data || []),
            value: []
        });
    }
    getData() {
        this.state.data;
    }
    getTreeData() {
        this.state.treeData;
    }
    getSelected() {
        
        return this.state.value;
    }
    clearSelected() {
     
        this.setState({ value: [] });
    }
    getSelectedData() {
        return this.state.data.filter(x => this.state.value.includes(x.ResourceID) && !x.__haveChildren);
    }
    clearFilter() {
        this.setState({ searchValue: '' });
    }

    render() {
        const tProps = {
            treeData: this.state.treeData,
            value: this.state.value,
            searchValue: this.state.searchValue,
            onChange: this.onChange,
            onSelect: this.onSelect,
            onSearch: this.onSearch,
            allowClear: true,
            autoClearSearchValue: false,
            filterTreeNode: this.filterTreeNode,
            maxTagPlaceholder: `+ ${this.state.value.length - TreeSelectResource.SELECTED_THRESHOLD} Seleccionados`,
            maxTagCount: TreeSelectResource.SELECTED_THRESHOLD,
            treeCheckable: true,
            treeNodeFilterProp: 'title',
            showCheckedStrategy: antd.TreeSelect.SHOW_CHILD,
            searchPlaceholder: "Seleccione un recurso",
            style: {
                //width: 570
            },
            dropdownStyle: {
                height: 300
            }
        };
        return React.createElement(antd.TreeSelect, tProps); //<TreeSelect {...tProps} />;
    }
}


class ParameterSelectorComponent extends Component {
    constructor(presenter, parent) {
        super(presenter);
        this.$data = null;
        this.$resource = new MetadataResource();

    }

    __render() {
        return html`
        <div class="modal" id="formParameterModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate @submit="${this.submit}">
                        <div class="modal-header">
                            <h4 class="modal-title">Seleccionar Parámetro</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                                
                            <div class="form-group row">
                                <label for="cboParam" class="col-sm-2 col-form-label">Parámetro</label>
                                <div class="col-sm-10">
                                    <select class="custom-select" id="cboParam" @change="${this.changeParam}" required></select>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="cboParamValue" class="col-sm-2 col-form-label">Valor</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboParamValue" class="custom-bootstrapselect" data-live-search="true" data-size="8" required></select>  
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <button type="submit" class="btn btn-success"><i class="fas fa-save"></i> Aceptar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }


    async __ready() {
        await CommonValueResource.loadSelect(this.$.cboParam, "PARAMETROS_RECURSOS");
        // await MetadataResource.loadSelect(this.$.cboParamValue, Metadata.ORACLE_RESPONSABILITIES);

        this.selectParamValue = new SelectMetadata(this.$.cboParamValue);
        this.selectParamValue.onChange = (e, data) => {
            this.$data = data.$newValueData;
        };
    }

    show() {
        this.clearControls();
        this.$$.formParameterModal.modal({ backdrop: 'static' });
    }

    clearControls() {
        this.$.cboParam.selectedIndex = -1;
        this.selectParamValue.clear();
        this.$data = null;
    }

    async changeParam(e) {
        this.selectParamValue.clear();
        if (this.$.cboParam.value == ResourceParameter.INTEGRACION_ORACLE_COMPANY_MOUNT) {
            await this.selectParamValue.refresh(Metadata.ORACLE_COMPANIES, (opt, item) => {
                opt.innerHTML = `${item.MetadataStr1} - ${item.MetadataDisplay}`;
                opt.value = item.MetadataID;
            });
        } else if (this.$.cboParam.value == ResourceParameter.INTEGRACION_ORACLE_ID) {
            await this.selectParamValue.refresh(Metadata.ORACLE_RESPONSABILITIES);
        } else if (this.$.cboParam.value == ResourceParameter.INTEGRATION_ICARUS_ACCESS) {
            await this.selectParamValue.refresh(Metadata.ICARUS_ACCESS);
        }
    }

    submit(e) {
        e.preventDefault();
        if (this.$.cboParam.selectedIndex == -1) {
            iziToast.error({ title: 'No ha seleccionado el parámetro.' });
            return;
        }
        if (this.selectParamValue.value == null) {
            iziToast.error({ title: 'El parámetro no tiene valor.' });
            return;
        }

        this.$$.formParameterModal.modal('hide');

        let param = this.$.cboParam.options[this.$.cboParam.selectedIndex].$data;
        if (this.$.cboParam.value == ResourceParameter.INTEGRACION_ORACLE_COMPANY_MOUNT) {
            this.$data.MetadataValue = this.$data.MetadataStr1;
        } else if (this.$.cboParam.value == ResourceParameter.INTEGRACION_ORACLE_ID) {
            this.$data.MetadataValue = this.$data.MetadataInt1;
        } else if (this.$.cboParam.value == ResourceParameter.INTEGRATION_ICARUS_ACCESS) {
            this.$data.MetadataValue = this.$data.MetadataInt1;
        }
        this.onSelect(param, this.$data);
    }

    onSelect(param, data) {
        console.warn(param, data);
    }
}