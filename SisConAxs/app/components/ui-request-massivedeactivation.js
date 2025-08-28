import { Component, html } from '../core.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';
import { PeopleResource, People } from './resources/resource-people.js';
import { AlertComponent } from './ui-alert.js';

export class RequestMassiveDeactivationComponent extends Component {
    static get SOURCE_SYSTEM() { return 'SOURCE_SYSTEM'; }
    static get SOURCE_EXCEL() { return 'SOURCE_EXCEL'; }

    constructor(presenter) {
        super(presenter);
        this.$entity = new Request();
        this.$resource = new RequestResource();
        this.$peopleResource = new PeopleResource();
        this.addComponent('alert', new AlertComponent(presenter, this));

        this.$source = RequestMassiveDeactivationComponent.SOURCE_SYSTEM;
    }

    __render(params) {
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

            .source-icon {
                padding-left: .5em;
            }
            .custom-file-label:after {
                content: "Elegir" !important;
            }
        </style>
        <h3>Baja Masiva de Permisos</h3>
        <div id="toolbar">
            <div class="row">
                <div class="col-auto pr-0">
                    <div class="input-group">
                        <span class="input-group-text" id="basic-addon1">Fuente de Datos:
                        <strong>
                        ${this.$source == RequestMassiveDeactivationComponent.SOURCE_SYSTEM?
                            html`<i class="fas fa-database source-icon"></i> Sistema` :
                            html`<i class="fas fa-file-excel source-icon"></i> Excel`
                        }
                        </strong>
                        </span>
                    </div>
                </div>
                <div class="col-auto">
                    <button id="btnLoad" class="btn btn-secondary" title="Cargar desde Sistema" @click=${this.loadData}>
                        <i class="fas fa-database"></i> Cargar desde Sistema
                    </button>
                </div>
                <div class="col-auto px-0">
                    <div class="input-group">
                        <div class="custom-file">
                            <input type="file" class="custom-file-input" id="fileExcel" onchange="$(this).next().after().text($(this).val().split('\\\\').slice(-1)[0])" @change=${this.loadFromExcel} lang="es">
                            <label class="custom-file-label" for="fileExcel">Elija un archivo</label>
                        </div>
                        <div class="input-group-append">
                            <a id="btnImport" class="btn btn-success" title="Descargar formato" href="Files/personas-formato.xlsx">
                                <i class="fas fa-file-excel"></i> Descargar formato
                            </a>
                        </div>
                    </div>

                </div>
                <div class="col-auto">
                    <button id="btnSave" class="btn btn-warning" title="Grabar" @click=${this.save}>
                        <i class="fas fa-save"></i> Grabar
                    </button>
                </div>
            </div>
        </div>
        <table
            id="table"
            data-locale="es-ES"
            data-toolbar="#toolbar"
            data-search="false"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <div id="edit"></div>
        <div id="alert"></div>`;
    }

    __ready() {
        let $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 130,
            filterControl: true,
            searchOnEnterKey: true,
            columns: [
                { checkbox: true, field: 'select', align: 'center', valign: 'middle', formatter: this.tableCheckFormatter.bind(this) },
                { title: "Número ID", field: "PeopleInternalID", filterControl: "input", align:"left", valign:"middle" },
                { title: "Apellidos", field: "PeopleFullLastName", filterControl: "input", align:"left", valign:"middle", width:225 },
                { title: "Nombres", field: "PeopleFullFirstName", filterControl: "input", align:"left", valign:"middle", width:225 },
                { title: "Tipo Doc", field: "PeopleDocTypeName", filterControl: "input", align:"center", valign:"middle" },
                { title: "Núm Doc", field: "PeopleDocNum", filterControl: "input", align:"left", valign:"middle" },
                { title: "Email", field: "PeopleEmail", filterControl: "input", align:"left", valign:"middle" },
                { title: "Área", field: "PeopleDepartmentName", filterControl: "input", align:"left", valign:"middle", width:250 },
                { title: "Cargo", field: "PeoplePositionName", filterControl: "input", align:"left", valign:"middle", width:250 },
                { title:"Nº Activos", field:"AssignedItems", align:"center", valign:"middle", class:"col-operate", formatter: $this.tableFormatterActivos, events:$this.tableEventColumnOperate, width:140  }
            ],
            detailView: true,
            // detailViewIcon: false,
            // detailViewByClick: true,
            detailFormatter: this.tableDetailFormatter.bind(this),
            onColumnSearch: function (filterColumn, textColumn) {
                $this.loadData();
            },
            onRefresh: function() {
                $this.loadData();
            }
        });
        this.loadData();
        this.$component.alert.attach(this.$.alert);
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
        return filters;
    }

    loadData() {
        this.$presenter.showLoading();
        const filters = this.prepareFilters();
        var params = Object.assign({
        }, filters);
        this.$peopleResource.listHasAccess(params).then(
        r => {
            this.$source = RequestMassiveDeactivationComponent.SOURCE_SYSTEM;
            this.render();
            this.$$.table.bootstrapTable('load', People.fromList(r).filter(x => x.AssignedItems.length > 0));
            this.setFilters(filters);
            this.clearFile();
            this.$presenter.hideLoading();
            iziToast.success({title: 'Cargado correctamente'});
        },
        err => {
            console.error('Error', err);
            iziToast.error({title: err});
            this.$presenter.hideLoading();
        });
    }

    loadFromExcel(e) {
        console.warn("prb");
        $(this).next().after().text(e.currentTarget.value.split('\\').slice(-1)[0]);
        if(!this.$.fileExcel.value) {
            iziToast.error({title: 'No ha seleccionado un archivo.'});
            return;
        }

        this.$presenter.showLoading();
        let formData = new FormData();
        formData.append('file', this.$.fileExcel.files[0]);
        this.$resource.massiveDeactivationLoadExcel(formData)
        .then(r => {
            this.$source = RequestMassiveDeactivationComponent.SOURCE_EXCEL;
            this.render();

            let data = People.fromList(r)
                                .map(x => {
                                    x.select = true;
                                    return x;
                                });
            this.$$.table.bootstrapTable('load', data.filter(x => x.Failed == false));

            var loadSuccess = data.filter(x => x.Failed == false).length;
            var loadFailed = data.filter(x => x.Failed == true).length;
            var htmlMessage = "";

            if(data.filter(x => x.Failed == true).length > 0){
             htmlMessage = `Se cargaron correctamente <b>${loadSuccess}</b> registro(s) e incorrectamente <b>${loadFailed}</b> registro(s): </br><table border="1" cellpadding="5" width="100%">
                            <thead><tr style="color: white; background-color: black;"><th width="20%">Número ID</th><th width="60%">Mensaje</th></tr></thead>
                            <tbody>
                                ${data.map(
                                    x => {
                                        if(x.Failed == true)
                                            return `<tr>${"<td>" + x.PeopleInternalID + "</td><td>" + x.FailedDescription + "</td>"}</tr>`;
                                    }
                                )}
                            </tbody>
                        </table>`;
                            }else{
                                htmlMessage = `Se cargaron correctamente ${loadSuccess} registro(s).`;
                            }

                            this.$component.alert.show($.parseHTML(htmlMessage));
            this.clearFile();
            this.$presenter.hideLoading();
            iziToast.success({title: 'Cargado correctamente'});
        },
        err => {
            console.error('Error', err);
            iziToast.error({title: err});
            this.clearFile();
            this.$presenter.hideLoading();
        });
    }

    save() {
        let selection = this.$$.table.bootstrapTable('getSelections');
        if(selection.length == 0) {
            iziToast.error({title:'No ha seleccionado algún registro.'});
            return;
        }

        this.$presenter.question(
            'Baja Masiva',
            'Esta seguro que desea dar de baja a las personas seleccionadas?',
            (instance, toast) => {
                // if(this.$source == RequestMassiveDeactivationComponent.SOURCE_SYSTEM) {
                    this.saveFromSystem(selection);
                // } else {
                //     this.saveFromExcel();
                // }
            }
        );
        
    }

    saveFromSystem(selection) {
        this.$presenter.showLoading();
        this.$resource.massiveDeactivation({
            listPeopleID: selection.map(x => x.PeopleID)
        }).then(r => {
            setTimeout(() => {
                this.$presenter.hideLoading();
                this.loadData();
                iziToast.success({title: 'Grabado correctamente.'});
            }, 2500);
        },
        err => {
            this.$presenter.hideLoading();
            iziToast.error({title: err});
        });
    }
    saveFromExcel() {
        this.$presenter.showLoading();
        this.$resource.massiveDeactivationExcel()
        .then(r => {
            setTimeout(() => {
                this.$presenter.hideLoading();
                this.loadData();
                iziToast.success({title: 'Grabado correctamente.'});
            }, 2500);
        },
        err => {
            this.$presenter.hideLoading();
            iziToast.error({title: err});
        });
    }

    tableCheckFormatter(value, row, index) {
        if (this.$source == RequestMassiveDeactivationComponent.SOURCE_EXCEL) {
            return {
                disabled: true,
                checked: !!value
            }
        } else {
            return {
                disabled: false,
                checked: !!value
            }
        }
    }
    tableDetailFormatter(index, row) {
        return `<ul>${row.AssignedItems.sort((a,b) => a.localeCompare(b)).map(x => '<li>' + x + '</li>').join('')}</ul>`;
    }
    tableFormatterActivos(value, row, index) {
        return `<button type="button" class="btn btn-block btn-outline-primary btn-toggle" >
                    Detalle <span class="badge badge-secondary">${row.AssignedItems.length}</span>
                </button>`;
    }
    get tableEventColumnOperate() {
        let _self = this;
        return {
            'click .btn-toggle': _self.toggleDetail.bind(_self)
        }
    }

    clearFile() {
        this.$.fileExcel.value = null;
        this.$.fileExcel.onchange();
    }

    toggleDetail(e, value, row, index) {
        this.$$.table.bootstrapTable('toggleDetailView', index);
    }
}