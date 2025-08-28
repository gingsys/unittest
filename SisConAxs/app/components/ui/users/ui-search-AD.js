import { Component, ComponentView, html } from '../../../core/core.js';
import { msalConfig } from '../../authConfig.js';
// import { Entity, Component, html } from '../../../core.js';
import { UserResource, User } from '../../resources/resource-user.js';

export class UserSearchADComponent extends Component {
    constructor() {
        super(false);
        this.$entity = new User();
        this.$resource = new UserResource();
        this.filters = {};
        this.cboTypeFiltro = [];
        this.txtFiltro = "";
    }

    get $template() {
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Buscar Usuario de Azure Active Directory</h4>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                    <!-- <div class="alert alert-primary" role="alert">
                        Para seleccionar un registro, hacer doble click sobre la fila.
                    </div> -->
                        <div id="toolbarAD">
                        </div>
                        <table
                            id="tableAD"
                            data-locale="es-ES"
                            data-toolbar="#toolbarAD"
                            data-pagination="true"
                            data-filter-control="true"
                            >
                            <div class="form-group row">
                                <label for="cboTypeFiltro" class="col-sm-2 col-form-label">Tipo Filtro</label>
                            <div></div>
                            <div >
                                <div class="input-group mb-3" style="margin: 0px !important;">
                                 <select id="cboTypeFiltro" class="custom-select" required>
                                         <option value="1">-- Seleccionar --</option>
                                         <option value="2">Por alias</option>
                                         <option value="3">Por nombre</option>
                                        </select>                                          
                                 <div class="invalid-feedback">
                                            Por favor, ingrese un Tipo de Filtro.
                                 </div>
                                </div>
                             </div>
                                <div style="color: white;"> &&&&&&&</div>
                                <div > 
                                <label for="txtFiltro" class="col-sm-2 col-form-label">Filtro</label>
                                </div>
                                <div class="col-sm-4">
                                    <input type="text" class="form-control" id="txtFiltro" filterControl=true  /> 
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un filtro correcto.
                                    </div>
                                </div>
                                  <button class="btn btn-secondary" type="button" id="btnSearchAD" @click=${this.loadDataAzure} title="Buscar usuario en Active Directory">
                                  Buscar <i class="fas fa-search"></i>
                                  </button>
                                </div>                                    

                            <thead>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                    </div>
            </div>
        </div>
    </div>
    <div id="edit"></div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        let $this = this;
        this.$$.formModal.on('hidden.bs.modal', this.__onClose.bind(this));

        this.$$.tableAD.bootstrapTable({
            //filterControl: true,
            //searchOnEnterKey: true,
            //sidePagination: "server",
            pageSize: "10",
            totalNotFiltered: 1,
            columns: [
               /* { title: "IdServicioAD", field: "id", align: "left", valign: "middle", class: "col-desc", filterControlPlaceholder: "idad" },*/
                { title: "Nombres", field: "givenName", align: "left", valign: "middle", class: "col-desc", filterControlPlaceholder: "Nombre" },
                { title: "Apellidos", field: "surname", align: "left", valign: "middle", class: "col-desc", filterControlPlaceholder: "Apellido" },
                { title: "Puesto", field: "jobTitle", align: "left", valign: "middle" },  //class: "col-desc", filterControl: "input", filterControlPlaceholder: "Puesto" },
                { title: "Teléfono", field: "mobilePhone", align: "left", valign: "middle" },  //class: "col-desc", filterControl: "input", filterControlPlaceholder: "Puesto" },
                { title: "Email", field: "mail", align: "left", valign: "middle", class: "col-desc", filterControlPlaceholder: "Email" },
                //{ title: "DOCUMENTO", field: "postalCode", align: "left", valign: "middle", class: "col-desc", filterControlPlaceholder: "Email" },

                { title: "", field: "", align: "center", valign: "middle", width: "40", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onRefresh: () => { },


            onColumnSearch: (filterColumn, textColumn) => {
                // if (textColumn != "") {
                //this.loadData(filterColumn, textColumn);
                /*      this.loadDataAzure(filterColumn, textColumn);*/
                // }
            },
            onDblClickRow: (row, element, field) => {
                this.__onSelect(row);
            }
        });
    }

    searchAD() {
        if (this.$entity.UserID == 0) {
            this.$$.formModal.addClass('modal-opened');
            this.$component.searchAD.show();
        }
    }
  

    setValues() {
        this.$$.formModal.removeClass('modal-opened');

        this.$.txtFiltro.value = User.fromList.givenName;
    }

    prepareFilters() {
        let filters = {};
        let controls = Array.from(this.$all('#tableAD th[data-field] input[type="text"]'));
        controls.forEach(control => {
            ['UserInternalID', 'UserLastName', 'UserFirstName', 'UserDocNum', 'UserCompany'].forEach(field => {
                if (control.classList.contains(`bootstrap-table-filter-control-${field}`)) {
                    if (control.value.trim() != '')
                        filters[field] = control.value.trim();
                }
            });
        });

        return filters;
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

    setFiltros(filters) {
        for (let field in filters) {
            let control = this.$one(`th[data-field] input[type="text"].bootstrap-table-filter-control-${field}`);
            /*   let control = this.$one(`th[data-field] input[type="text"].bootstrap-table-filter-control-${field}`);*/
            if (!!control) {
                control.value = filters[field];
                control.addEventListener('click', function (e) {
                    e.target.focus();
                    e.target.select();
                });
            }
        }
    }

    prepareFiltros() {
        let filters = {};
        let controls = Array.from(this.$all('#tableAD th[data-field] input[type="text"]'));
        controls.forEach(control => {
            ['txtFiltro'].forEach(field => {
                if (control.classList.contains(`bootstrap-table-filter-control-${field}`)) {
                    if (control.value.trim() != '')
                        filters[field] = control.value.trim();
                }
            });
        });

        return filters;
    }

    loadDataAzure() {
      
        this.$presenter.showLoading();
        var tipo = this.$.cboTypeFiltro.value;
        var filtro = this.$.txtFiltro.value;

        this.graphMeEndpoint = 'https://graph.microsoft.com/v1.0/me';
        this.$myMSALObj = new Msal.UserAgentApplication(msalConfig);

        if (tipo == "2") {
            var url = "https://graph.microsoft.com/v1.0/users?$filter=startswith(mail,(" + "'" + filtro + "'" + "))";
        }
        else if (tipo == "3") {
            var url = "https://graph.microsoft.com/v1.0/users?$filter=startswith(displayName,(" + "'" + filtro + "'" + "))";
        }
        else {
            this.$presenter.hideLoading();
            iziToast.error({ title: 'Debe seleccionar un tipo.' });
        }

        if (this.$myMSALObj.getAccount()) {
            var myHeaders = new Headers();
            myHeaders.append("Authorization", `Bearer ${localStorage.getItem("Token")}`);

            var requestOptions = {
                method: 'GET',
                headers: myHeaders,
                redirect: 'follow'
            };

            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => this.showInfo(result))
                .catch(error => console.log('error', error));
        }

    }

    showInfo(response) {
        if (response.value.length > 0) {
            let filters = this.prepareFilters();
            this.$$.tableAD.bootstrapTable('load', User.fromList(response.value || []));
            this.$$.tableAD.bootstrapTable('refreshOptions', {
                totalRows: response.value.length,
            });
            this.setFilters(filters);
            this.$presenter.hideLoading();
            iziToast.success({ title: 'Cargado correctamente' });
        }
        else {
            this.$presenter.hideLoading();
            iziToast.error({ title: 'No se encontraron coincidencias.' });
        }
    }

    /*ya no se usa*/
    loadData(filterColumn, textColumn) {
        this.$presenter.showLoading();
        let filters = this.prepareFilters();

        let options = this.$$.tableAD.bootstrapTable('getOptions');

        let params = Object.assign({
            ViewType: this.viewType,
            CurrentPage: options.pageNumber,
            PageSize: options.pageSize
        }, filters);

        this.$resource.getFromAD(params).then(
            r => {
                this.$$.tableAD.bootstrapTable('load', User.fromList(r || []));
                this.$$.tableAD.bootstrapTable('refreshOptions', {
                    totalRows: r.length,
                    // height: window.innerHeight - this.$.tableAD.getBoundingClientRect().top - 80
                });
                this.setFilters(filters);
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err }); //'Error al cargar'});
            });
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-outline-primary btn-select" title="Seleccionar">
                        <i class="fas fa-check"></i>
                    </button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let _self = this;
        return {
            'click .btn-select': _self.btnColumnSelectClick.bind(_self)
        }
    }

    async getExtraInfoUser(row) {
       
        var url = "https://graph.microsoft.com/v1.0/users/{" + row.id + "}?$select=displayName,companyName,postalCode,givenName,surname,jobTitle";
        this.seacrhEmpresaDocumento(localStorage.getItem("Token"), url)
            .then(data => {
                
                row.postalCode = data.postalCode;
                this.__onSelect(row);
                
            }).catch(err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err.data, position: 'center' });
            });
    }

    async seacrhEmpresaDocumento(token, endpoint) {
        var myHeaders = new Headers();
        myHeaders.append("Authorization", "Bearer " + token);
        myHeaders.append("Cookie", "ARRAffinity=0a3517ba6ed8bb14ffe517099672a3eb4ea3c4b710ad8c6e0edaa70c2d244335; ARRAffinitySameSite=0a3517ba6ed8bb14ffe517099672a3eb4ea3c4b710ad8c6e0edaa70c2d244335");

        var requestOptions = {
            method: 'GET',
            headers: myHeaders,
            redirect: 'follow'
        };

        return fetch(endpoint, requestOptions)
            .then(function (response) {
                return response.json();
            })
            .catch(
                error => console.log('error', error)
            );
    }

    btnColumnSelectClick(e, value, row, index) {
       
        this.getExtraInfoUser(row)
            .then(function (response) {
                this.render(true);
                this.$presenter.hideLoading();
            })
            .catch(
                error => console.log('error', error)
            );
    }

    __onSelect(entity) {
        console.log('Selected', entity);
        this.$$.formModal.modal('hide');
        this.dispatchEvent(new CustomEvent('selectitem', {
            detail: entity
        }));
    }

    __onClose() {
        this.dispatchEvent(new CustomEvent('close'));
    }

    clearControls() {
        this.filters = {};
        this.$$.tableAD.bootstrapTable('load', []);
        this.$$.tableAD.bootstrapTable('clearFilterControl');
        this.$.cboTypeFiltro.selectedIndex = -1;
        this.$.txtFiltro.value = null;
    }

    show() {
        this.clearControls();
        this.$$.formModal.modal({ backdrop: 'static' });
    }
}
customElements.define('user-search-ad', UserSearchADComponent);