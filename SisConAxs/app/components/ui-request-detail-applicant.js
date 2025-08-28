import { Component, html } from '../core.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';
import { RequestDetailHistoryComponent } from './ui-request-detail-history.js';

export class RequestDetailApplicantComponent extends Component {
    constructor(presenter, parent) {
        super(presenter);
        this.$parent = parent;
        this.$entity = new Request();
        this.$resource = new RequestResource();
        this.$dataResource = [];
        this.detailExpanded = true;

        this.addComponent('requestApplicantDetailHistory', new RequestDetailHistoryComponent(presenter, this));
    }

    __render() {
        return html `
        <style>
        </style>
        <div class="modal" id="formApplicantModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title request-title" id="exampleModalLabel">
                                <div class="row">
                                    <div class="col-sm-12"><i class="fas fa-user"></i> ${this.$entity.RequestToName}
                                    <div class="float-right"><i class="fas fa-briefcase"></i> ${this.$entity.RequestToPosition}</div></div>
                                </div>
                                </h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close" @click=${this.__onClose}>
                                    <span aria-hidden="true">&times;</span>
                                </button>
                        </div>
                        <div class="modal-body pt-1">
                            <div class="row">
                                <div class="col">
                                    <div class="form-group mb-0" id="toolbarNewAccesses">
                                        <label class="" style="width: 250px;">Accesos Solicitados:</label>
                                        <!-- <button id="btnExpandDetail" type="button" class="btn btn-outline-secondary float-right mr-0" @click=${this.btnExpandDetailClick}>
                                            ${this.detailExpanded? 
                                                html`<i class="fas fa-compress-arrows-alt"></i> Contraer` :
                                                html`<i class="fas fa-expand-arrows-alt"></i> Expandir`
                                            }
                                        </button> -->
                                    </div>
                                    <table
                                        id="tableDetailNew"
                                        data-search="true"
                                        data-locale="es-ES"
                                        >
                                        <thead>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                    <div class="form-group mb-0" id="toolbarCurrentAccesses">
                                        <label class="" style="width: 250px;">Accesos Actuales:</label>
                                        <select id="cboCategoryDetailResource" class="custom-select col-sm-6" required="" @change=${this.cboCategoryResourceChange}>
                                        </select>
                                    </div>
                                    <table
                                        id="tableDetailNow"
                                        data-search="true"
                                        data-locale="es-ES">
                                        <thead>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal" @click=${this.__onClose}><i class="fas fa-window-close"></i> Cerrar</button>
                        </div>
                </div>
            </div>
        </div>
        <div data-component="requestApplicantDetailHistory"></div>`;
    }

    __ready() {
        let $this = this;
        this.$$.formApplicantModal.on('hidden.bs.modal', this.__onClose.bind(this));
        this.$$.tableDetailNew.bootstrapTable({
            toolbar: this.$.toolbarNewAccesses,
            // searchOnEnterKey: true,
            // height: window.innerHeight - 516,
            columns: [
                { title: "Categoría", field: "ResourceCategoryDisplay", align: "left", valign: "middle", width: 250, class: "" },
                { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterRecurso },
                { title: "Valor", field: "RequestDetDisplayValue", align: "left", valign: "middle", formatter: $this.tableFormatterValor },
                { title:"Vigencia", field:"ReqDetTemporal", align:"center", valign:"middle", width: 180, formatter: $this.tableFormatterValidity.bind($this) },
                { title: "Tipo", field: "RequestDetTypeDisplay", align: "center", valign: "middle", cellStyle: $this.tableCellStyleType },
                { title: "", field: "ResourceID", align: "center", valign: "middle", width: 200, formatter: $this.tableFormatterApprove, events: $this.tableEventColumnApprove }
            ],
            onRefresh: () => {},
        });

        this.$$.tableDetailNow.bootstrapTable({
            toolbar: this.$.toolbarCurrentAccesses,
            // searchOnEnterKey: true,
            // pagination: "true",
            // height: window.innerHeight - 516,
            columns: [
                // { title: "", field: "RequestDetPrevData", align: "center", valign: "middle", width: 30, cellStyle: $this.tableCellStyleCurrentResource, formatter: $this.tableFormatterCurrentResource },
                { title: "Categoría", field: "ResourceCategoryName", align: "left", valign: "middle", width: 250, cellStyle: $this.tableCellStyle },
                { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterRecurso, cellStyle: $this.tableCellStyle },
                { title: "Valor", field: "", align: "left", valign: "middle", formatter: $this.tableFormatterValue.bind($this) },
                { title:"Vigencia", field:"ReqDetTemporal", align:"center", valign:"middle", width: 180, formatter: $this.tableFormatterValidity.bind($this) }
            ],
            rowStyle: $this.rowStyle,
            onRefresh: () => {},
        });
    }

    show(entity, type) {
        this.$entity = entity;
        this.$presenter.showLoading();
        this.resetCategoryResource();
        this.render();
        this.$$.tableDetailNew.bootstrapTable('load', entity.AccessRequestDetails);
        this.generateDetailFromPeople(entity.RequestTo, type, entity.RequestToCompany);
        this.$$.formApplicantModal.modal({ backdrop: 'static' });
        
        this.$$.tableDetailNew.bootstrapTable('refreshOptions', {
            height: window.innerHeight - 452
        });
        this.$$.tableDetailNow.bootstrapTable('refreshOptions', {
            height: window.innerHeight - 452
        });
    }

    onClose() {
    }

    __onClose() {
        this.$parent.$tableView.refreshView();
        this.onClose();
    }

    btnExpandDetailClick(e) {
        this.detailExpanded = !this.detailExpanded;
        this.render();
        this.refreshView();
    }

    refreshView() {
        if(this.detailExpanded) {
            this.$$.tableDetailNew.bootstrapTable('refreshOptions', {
                height: window.innerHeight - 400
            });
        }
        else {
            this.$$.tableDetailNew.bootstrapTable('refreshOptions', {
                height: window.innerHeight - 570
            });
        }
    }

    tableFormatterRecurso(value, row, index) {
        return `<strong>${row.ResourceName}</strong><br/><small>${row.ResourceFullName}</small>`;
    }

    tableCellStyleType(value, row, index, field) {
        let classes = '';
        if (row.RequestDetType == Request.TYPE_ALTA)
            classes = 'request-alta';
        else if (row.RequestDetType == Request.TYPE_MODIFICACION)
            classes = 'request-modificacion';
        else if (row.RequestDetType == Request.TYPE_BAJA)
            classes = 'request-baja';
        return {
            classes: classes,
            css: {}
        };
    }

    tableFormatterValidity(value, row, index) {
        if(row.ReqDetTemporal > 0) {
            return moment(row.ReqDetValidityFrom).format('DD/MM/YYYY') + " - " + moment(row.ReqDetValidityUntil).format('DD/MM/YYYY');
        }
    }

    tableFormatterApprove(value, row, index) {
        return `<div class="btn-group btn-group-sm check-approve" data-toggle="buttons">
                    <button class="btn btn-outline-success btn-accept ${row.RequestResponse == RequestDetail.REPONSE_ACCEPT ? 'active' : ''}" title="Aprobar" ${row.FlagIsApprover == 1 ? 'disabled' : ''}>
                        <input type="radio" name="options-${row.RequestDetID}" value="1" autocomplete="off" />
                        <i class="fas fa-check fa-sm btn-approve"></i>
                    </button>
                    <button class="btn btn-outline-danger btn-reject ${row.RequestResponse == RequestDetail.REPONSE_REJECT ? 'active' : ''}" title="Rechazar" ${row.FlagIsApprover == 1 ? 'disabled' : ''}>
                        <input type="radio" name="options-${row.RequestDetID}" value="0" autocomplete="off" />
                        <i class="fas fa-times fa-sm btn-approve"></i>
                    </button>
                </div>
                <div class="btn-group btn-group-sm" role="group" aria-label="Historial">
                    <button type="button" class="btn btn-outline-primary btn-history" title="Historial"><i class="fas fa-history"></i> Historial</button>
                </div>`;
    }

    tableFormatterValor(value, row, index) {
        if(row.RequestDetDisplayValue != null){
            var array = row.RequestDetDisplayValue.split(',');
            if(array.length > 1){
                return array.join("</br>");
            }else{
                return row.RequestDetDisplayValue;
            }
        }else{
            return row.RequestDetDisplayValue;
        }
    }
    
    get tableEventColumnApprove() {
        let $this = this;
        return {
            'click .btn-accept': $this.btnColumnAcceptClick.bind($this),
            'click .btn-reject': $this.btnColumnRejectClick.bind($this),
            'click .btn-history': $this.btnColumnHistoryClick.bind($this.$parent.$component)
        }
    }
    btnColumnAcceptClick(e, value, row, index) {
        row.RequestResponse = RequestDetail.REPONSE_ACCEPT;
        if (this.$entity.AccessRequestDetails.every(x => x.RequestResponse == RequestDetail.REPONSE_ACCEPT)) {
            this.$parent.$$.btnApproveAll.addClass('active')
        }
        this.$parent.$$.btnRejectAll.removeClass('active');
    }
    btnColumnRejectClick(e, value, row, index) {
        row.RequestResponse = RequestDetail.REPONSE_REJECT;
        if (this.$entity.AccessRequestDetails.every(x => x.RequestResponse == RequestDetail.REPONSE_REJECT)) {
            this.$parent.$$.btnRejectAll.addClass('active');
        }
        this.$parent.$$.btnApproveAll.removeClass('active');
    }

    btnColumnHistoryClick(e, value, row, index) {
        this.detailApplicant.$component.requestApplicantDetailHistory.show(this.detailApplicant.$entity, row);
    }

    tableFormatterValue(value, row, index) {
            if (row.RequestDetType == Request.TYPE_ALTA) {
                if (row.ResourceAccessTypeType == AccessType.TYPE_TEXTO) {
                    return `<input type="text" value="${row.RequestDetStrValue || ''}" class="form-control input-text" min="3" placeholder="Ingrese un texto" required />`;
                } else if (row.ResourceAccessTypeType == AccessType.TYPE_SELECT_SIMPLE) {
                    return `<select class="custom-select input-select-simple" data-index="${index}" required>
                        ${row.ResourceAccessTypeValues.map(x => `<option value="${x.TypeValueID}" ${row.RequestDetIntValue == x.TypeValueID? 'selected' : ''}>${x.TypeValueDisplay}</option>`).join('')}
                    </select>`;
        } else if(row.ResourceAccessTypeType == AccessType.TYPE_SELECT_MULTIPLE) {
            return `<select multiple class="custom-select input-select-multiple" required>
                        ${row.ResourceAccessTypeValues.map(x => `<option value="${x.TypeValueID}">${x.TypeValueDisplay}</option>`).join('')}
                    </select>`;
        } else if(row.ResourceAccessTypeType == AccessType.TYPE_ENTERPRISE_EMAIL) {
            return `<input type="email" value="${row.RequestDetStrValue || ''}" class="form-control input-email" placeholder="ejemplo: admin@gym.com" required />`;
        }
    } else {
        return value;
    }
    }

    cboCategoryResourceChange(e) {
        let data = [];
        if(this.$.cboCategoryDetailResource.value == "null"){
            data = this.$dataResource;
        }else{
            data = this.$dataResource.filter(x => x.ResourceCategoryName == this.$.cboCategoryDetailResource.value);
        }
        this.$$.tableDetailNow.bootstrapTable('load', data);
        // this.render();
    }

    resetCategoryResource(){
        // Se cargan las categorías
        let select = this.$.cboCategoryDetailResource;
        select.innerHTML = "";
        let opt = document.createElement('option');
        opt.innerHTML = "Seleccione categoría";
        opt.value = null;
        opt.$data = null;
        select.appendChild(opt);

        // Se limpia el tree control
        this.cboCategoryResourceChange(select);
    }

    async generateDetailFromPeople(peopleID, type, companyID) {
        let entity = this.$entity;
        await this.$resource.generateDetailFromPeople(peopleID, type, companyID)
            .then(r => {
                let detail = RequestDetail.fromList(r.data.filter(x => x.RequestDetPrevData && entity.AccessRequestDetails.map(x => x.ResourceID).indexOf(x.ResourceID) < 0));//&& x.RequestDetPending == false
                let requestCategory = r.data.map(x => x.ResourceCategoryName).filter((x, i, a) => a.indexOf(x) == i);

                //Se cargan las categorías
                this.resetCategoryResource();

                requestCategory.forEach(item => {
                    let opt = document.createElement('option');
                    opt.innerHTML = item;
                    opt.value = item;
                    opt.$data = item;
                    this.$.cboCategoryDetailResource.appendChild(opt);
                });
                this.$.cboCategoryDetailResource.value = null;

                this.$dataResource = detail;
                this.$$.tableDetailNow.bootstrapTable('load', detail);

                // if request BAJA: add to detail
                // if (this.$.cboType.value == Request.TYPE_BAJA) {
                //     this.$entity.AccessRequestDetails = detail;
                // }
            })
            .catch(err => {
                iziToast.error({ title: err });
            });

        this.$presenter.hideLoading();
    }

    rowStyle(row, index) {
        if (row.RequestDetType == Request.TYPE_BAJA) {
            return {
                css: {
                    color: '#F55'
                }
            }
        } else if (row.RequestDetType == Request.TYPE_ALTA) {
            return {
                css: {
                    color: '#04B404'
                }
            }
        }else if (row.RequestDetType == null && row.RequestDetPending == true) {
            return {
                css: {
                    color: '#007bff'
                }
            }
        }
        return {
            css: {}
        }
    }
}