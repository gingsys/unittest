import { Component, html } from '../core.js';
import { ResourceResource } from './resource-resource.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';
import { RequestEditComponent, RequestDetailTableView } from './ui-request-edit.js';
import { SelectResource } from './ui-resource.js';

export class RequestComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.$entity = new Request();
        this.$resource = new RequestResource();

        this.viewType = Request.VIEW_REQUEST_SEND;
        this.params = {};
        this.loadingResourcesFilter = true;

        this.addComponent('editRequest', new RequestEditComponent(presenter));
    }

    __render() {
        return html `
        <style>
            /* cols */
            /* .col-check {
                box-sizing: border-box;
                width: 25px;
            } 
            .col-name {
                box-sizing: border-box;
                width: 120px;
            } */
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

            #sectionTable .fixed-table-toolbar:first-child .bs-bars.float-left {
                width: calc(100% - 45px) !important;
            }
        </style>
        <h3 id="title">Solicitudes</h3>
        <div id="toolbar">
            <div class="row">
                <div class="col-md-4">
                    <div class="btn-group">
                        <button id="btnAdd" class="btn btn-warning" @click=${this.btnAddClick} ?data-hidden=${this.viewType != Request.VIEW_REQUEST_SEND}>
                            <i class="fas fa-plus"></i> Agregar
                        </button>
                    </div>
                </div>
                <div class="col-md-4 pr-0">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <label class="input-group-text" for="txtFilterDetailCategory">Filtro Detalle: Categoría</label>
                        </div>
                        <input type="text" id="txtFilterDetailCategory" class="form-control" @keypress=${this.txtFilterDetailsKeyPress} />
                        <div class="input-group-append">
                            <button class="btn btn-outline-secondary" type="button" title="Borrar filtro" @click=${this.clearFilterDetailCategory}>
                                <i class="fas fa-eraser"></i>
                            </button>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <label class="input-group-text" for="txtFilterDetailResource">Filtro Detalle: Recurso</label>
                            <!-- <span class="input-group-text" ?data-hidden=${!this.loadingResourcesFilter}><i class="fas fa-circle-notch fa-spin"></i></span> -->
                            <!-- <select id="cboResourceFilter" ?data-hidden=${!!this.loadingResourcesFilter} data-live-search="true" data-size="8" data-width="300px"></select> -->
                        </div>
                        <input type="text" id="txtFilterDetailResource" class="form-control" @keypress=${this.txtFilterDetailsKeyPress} />
                        <div class="input-group-append">
                            <button class="btn btn-outline-secondary" type="button" title="Borrar filtro" @click=${this.clearFilterDetailResource}>
                                <i class="fas fa-eraser"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <section id="sectionTable">
            <table
                id="table"
                data-locale="es-ES"
                data-toolbar="#toolbar"
                >
                <!-- data-search="true" -->
                <thead>
                </thead>
                <tbody></tbody>
            </table>
        </section>
        <div data-component="editRequest"></div>`;
    }

    __ready() {
        let $this = this;
        if (this.params.type == 'send') {
            this.viewType = Request.VIEW_REQUEST_SEND;
            this.$.title.innerHTML = 'Solicitudes emitidas';
        } else if (this.params.type == 'forapprove') {
            this.viewType = Request.VIEW_REQUEST_FORAPPROVE;
            this.$.title.innerHTML = 'Solicitudes por revisar';
        } else if (this.params.type == 'search') {
            this.viewType = Request.VIEW_REQUEST_SEARCH;
            this.$.title.innerHTML = 'Consultar Solicitudes';
        }
        this.render();

        // filter detail resource
        // this.selectResourceFilter = new SelectResource(this.$.cboResourceFilter);
        // this.selectResourceFilter.onChange = (e, data) => {
        //     this.loadData();
        // };
        // this.selectResourceFilter.refresh().then(r => {
        //     this.loadingResourcesFilter = false;
        //     this.render();
        // });

        this.$$.table.bootstrapTable({
            height: window.innerHeight - 130,
            filterControl: true,
            searchOnEnterKey: true,
            // showSearchClearButton: true,
            // showSearchButton: true,
            showRefresh: "true",
            sidePagination: "server",
            pagination: "true",
            pageSize: "20",
            pageList: "[20, 40, 80, 160]",
            paginationHAlign: "left",
            paginationDetailHAlign: "right",
            columns: [
                { title: "Número", field: "RequestNumber", align: "left", valign: "middle", filterControl: "input", filterControlPlaceholder: "Número" },
                { title: "Solicitado para", field: "RequestToName", align: "left", valign: "middle", width: 225, filterControl: "input", filterControlPlaceholder: "Solicitado para" },
                { title: "Proyecto", field: "RequestToProject", align: "left", valign: "middle", width: 225, filterControl: "input", filterControlPlaceholder: "Proyecto" },
                { title: "Cargo", field: "RequestToPosition", align: "left", valign: "middle", width: 225, filterControl: "input", filterControlPlaceholder: "Cargo" },
                { title: "Tipo", field: "RequestTypeDisplay", align: "left", valign: "middle", filterControl: "input", filterControlPlaceholder: "Tipo" },
                { title: "Prioridad", field: "RequestPriorityName", align: "center", valign: "middle", filterControl: "input", filterControlPlaceholder: "Prioridad" },
                { title: "Estado", field: "RequestStatusName", align: "center", valign: "middle", width: 110, filterControl: "input", filterControlPlaceholder: "Estado", formatter: $this.tableFormatterStatus.bind($this) },
                // { title: "Items", field: "RequestNroItems", align: "center", valign: "middle", filterControl: "input", filterControlPlaceholder: "Items" },
                { title: "F. Solicitud", field: "RequestDateDisplay", align: "center", valign: "middle", filterControl: "input", filterControlPlaceholder: "F. Solicitud" }, //formatter: $this.tableFormatterDate },
                { title: "F. Termino", field: "RequestCompletedDateDisplay", align: "center", valign: "middle", filterControl: "input", filterControlPlaceholder: "F. Termino" }, //formatter: $this.tableFormatterDate },
                { title: "", field: "", align: "center", valign: "middle", width: 120, class: "", formatter: $this.tableFormatterOperate.bind($this), events: $this.tableEventColumnOperate }
            ],
            detailView: true,
            // detailViewIcon: false,
            // detailViewByClick: true,
            detailFormatter: this.tableDetailFormatter.bind(this),
            onDblClickRow: (row, element, field) => {
                this.edit(row.RequestID);
            },
            onRefresh: () => {
                this.loadData();
            },
            onColumnSearch: (filterColumn, textColumn) => {
                // this.loadData();
            },
            onPageChange: () => {
                this.loadData();
            },
        });

        this.loadData();
        // if (this.viewType == Request.VIEW_REQUEST_SEND && !!this.params.new) {
        //     this.add();
        // }
        if (this.viewType == Request.VIEW_REQUEST_FORAPPROVE && !!this.params.id) {
            this.edit(this.params.id);
        }
    }

    prepareFilters() {
        let filters = {};
        let controls = Array.from(this.$all('.fixed-table-header th[data-field] input[type="text"]'));
        controls.forEach(control => {
            ['RequestNumber', 'RequestToName', 'RequestToProject', 'RequestToPosition',
                'RequestTypeDisplay', 'RequestPriorityName', 'RequestStatusName', 'RequestNroItems',
                'RequestDateDisplay', 'RequestCompletedDateDisplay'
            ].forEach(field => {
                if (control.classList.contains(`bootstrap-table-filter-control-${field}`)) {
                    if (control.value.trim() != '')
                        filters[field] = control.value.trim();
                }
            });
        });
        // detail filter
        if (this.$.txtFilterDetailResource.value.trim() != '')
            filters.RequestDetailResource = this.$.txtFilterDetailResource.value.trim();
        if (this.$.txtFilterDetailCategory.value.trim() != '')
            filters.RequestDetailCategory = this.$.txtFilterDetailCategory.value.trim();

        return filters;
    }
    setFilters(filters) {
        for (let field in filters) {
            let control = this.$one(`th[data-field] input[type="text"].bootstrap-table-filter-control-${field}`);
            if (!!control) {
                control.value = filters[field];
                control.addEventListener('click', function(e) {
                    e.target.focus();
                    e.target.select();
                });
            }
        }
    }

    loadData() {
        this.$presenter.showLoading();

        let filters = this.prepareFilters();
        let options = this.$$.table.bootstrapTable('getOptions');
        let params = Object.assign({
            ViewType: this.viewType,
            CurrentPage: options.pageNumber,
            PageSize: options.pageSize
        }, filters);

        this.$resource.get(params).then(
            r => {
                this.$$.table.bootstrapTable('load', Request.fromList(r.data || []));
                this.$$.table.bootstrapTable('refreshOptions', {
                    totalRows: r.totalServerItems
                });
                this.setFilters(filters);
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                console.error('Error', err);
                iziToast.error({ title: err });
                this.$presenter.hideLoading();
            });
    }

    btnAddClick(e) {
        this.add();
    }

    txtFilterDetailsKeyPress(e) {
        if (event.keyCode == 13) {
            this.$$.table.bootstrapTable('selectPage', 1);
            e.target.select();
        }
    }

    clearFilterDetailCategory(e) {
        this.$.txtFilterDetailCategory.value = null;
        this.$$.table.bootstrapTable('selectPage', 1);
    }
    clearFilterDetailResource(e) {
        this.$.txtFilterDetailResource.value = null;
        this.$$.table.bootstrapTable('selectPage', 1);
    }

    tableDetailFormatter(index, row) {
            return `<ul>
            ${row.AccessRequestDetails
                .sort((a,b) => a.ResourceFullName.localeCompare(b.ResourceFullName))
                .map(x =>
                    `<li>
                        ${x.RequestDetType == Request.TYPE_ALTA ? `<span class="badge badge-success">${x.RequestDetDisplayValue}</span>` : ''}
                        ${x.RequestDetType == Request.TYPE_MODIFICACION ? `<span class="badge badge-warning">${x.RequestDetDisplayValue}</span>` : ''}
                        ${x.RequestDetType == Request.TYPE_BAJA ? `<span class="badge badge-danger">${x.RequestDetDisplayValue}</span>` : ''}
                        <span class="badge badge-secondary">${x.ResourceCategoryName}</span>
                        ${x.ResourceFullName}
                    </li>`)
                .join('')}
        </ul>`;
    }

    // tableFormatterDate(value, row, index) {
    //     return moment(value).format('DD/MM/YYYY');
    // }

    tableFormatterStatus(value, row, index) {
        let f = '';
        if (row.RequestStatus == Request.STATUS_ANULADO)
            f = `<i class="fas fa-ban"></i>`;
        else if (row.RequestStatus == Request.STATUS_APROBADO)
            f = `<i class="fas fa-check"></i>`;
        else if (row.RequestStatus == Request.STATUS_ATENDIDO)
            f = `<i class="fas fa-hand-holding"></i>`;
        else if (row.RequestStatus == Request.STATUS_EN_PROCESO)
            f = `<i class="fas fa-clock"></i>`;
        else if (row.RequestStatus == Request.STATUS_INDEFINIDO)
            f = `<i class="fas fa-question"></i>`;
        else if (row.RequestStatus == Request.STATUS_PARCIAL)
            f = `<i class="fas fa-hourglass-half"></i>`;
        else if (row.RequestStatus == Request.STATUS_PENDIENTE)
            f = `<i class="fas fa-clock"></i>`;
        else if (row.RequestStatus == Request.STATUS_RECHAZADO)
            f = `<i class="fas fa-times"></i>`;
        else if (row.RequestStatus == Request.STATUS_EXPIRADO)
            f = `<i class="fas fa-clock"></i>`;
        return `${f} ${value}`;
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" style="width:100px">
                    ${(this.viewType == Request.VIEW_REQUEST_FORAPPROVE)?
                    `<button type="button" class="btn btn-secondary btn-edit" title="Revisar">
                        <i class="fas fa-check"></i>
                    </button>` :
                    `<button type="button" class="btn btn-secondary btn-edit" title="Ver">
                        <i class="far fa-eye"></i>
                    </button>`}
                    <button type="button" class="btn btn-block btn-outline-primary btn-toggle-detail px-1" >
                        Detalle <span class="badge badge-secondary">${row.AccessRequestDetails.length}</span>
                    </button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let _self = this;
        return {
            'click .btn-edit': _self.btnColumnEditClick.bind(_self),
            'click .btn-toggle-detail': _self.toggleDetail.bind(_self)
        }
    }

    btnColumnEditClick(e, value, row, index) {
        this.edit(row.RequestID);
    }

    add() {
        if (this.viewType == Request.VIEW_REQUEST_SEND) {
            this.$presenter.showLoading();
            this.$resource.requestBy().then(r => {
                    this.$component.editRequest.add(r);
                },
                err => {
                    this.$presenter.hideLoading();
                    console.error('Error', err);
                    iziToast.error({ title: 'Error al cargar' });
                });
            //this.$component.editRequest.add();
        }
    }

    edit(requestID) {
        this.$presenter.showLoading();

        let params = undefined;
        if (this.viewType == Request.VIEW_REQUEST_FORAPPROVE) {
            params = { ViewType: 'REQUEST_FORAPPROVE' };
        }
        
        this.$resource.getByID(requestID, params).then(r => {
                let entity = Request.fromObject(r);
                let mode = RequestDetailTableView.MODE_VIEW;

                if (requestID == 0) {
                    mode = RequestDetailTableView.MODE_EDIT;
                } else if (this.viewType == Request.VIEW_REQUEST_FORAPPROVE) {
                    mode = RequestDetailTableView.MODE_APPROVE;
                }
                this.$component.editRequest.edit(entity, mode);
            },
            err => {
                this.$presenter.hideLoading();
                console.error('Error', err);
                iziToast.error({ title: 'Error al cargar' });
            });
    }

    toggleDetail(e, value, row, index) {
        this.$$.table.bootstrapTable('toggleDetailView', index);
    }
}