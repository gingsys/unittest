import { Component, html } from '../core.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';

export class RequestConfirmComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.$entity = new Request();
    }

    __render() {
        return html `
            <style>
                .current-resource {
                background-color: #555 !important;
                color: #FFF;
                }
                .current-resource-delete {
                    background-color: #F55 !important;
                    color: #FFF;
                }
                .current-resource-add {
                    background-color: #04B404 !important;
                    color: #FFF;
                }
                .current-resource-modify {
                    background-color: orange !important;
                    color: #FFF;
                }

                .color-resource-delete {
                    color: #F55;
                }
                .color-resource-add {
                    color: #04B404;
                }
                .color-resource-modify {
                    color: orange;
                }
            </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Está seguro de guardar los siguientes cambios?</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <table
                            id="tableDetail"
                            data-locale="es-ES"
                            >
                            <thead></thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal" id="modal-btn-no" @click=${this.__onClose}><i class="fa fa-times" aria-hidden="true"></i> No</button>
                        <button type="button" class="btn btn-warning" id="modal-btn-si" @click=${this.__onConfirm}><i class="fa fa-check" aria-hidden="true"></i> Si</button>
                    </div>
                </div>
            </div>
        </div>
        `;
    }

    __ready() {
        let $this = this;
        this.$$.formModal.on('hidden.bs.modal', this.__onClose.bind(this));

        this.$$.tableDetail.bootstrapTable({
            height: window.innerHeight - 245,
            searchOnEnterKey: true,
            filterControl: true,
            // pagination: true,
            // pageSize: "5",
            columns: [
                { title: "", field: "RequestDetPrevData", align: "center", valign: "middle", width: 30, cellStyle: $this.tableCellStyleCurrentResource, formatter: $this.tableFormatterCurrentResource },
                { title: "Categoría", field: "ResourceCategoryDisplay", align: "left", valign: "middle", width: 150, cellStyle: $this.tableCellStyle },
                { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterRecurso, cellStyle: $this.tableCellStyle },
                { title: "Valor", field: "", align: "left", valign: "middle", formatter: $this.tableFormatterValue.bind($this) },
                { title: "Vigencia", field: "", align: "center", valign: "middle", width: 180, formatter: $this.tableFormatterValidity.bind($this) },
                { title: "Tipo", field: "RequestDetTypeDisplay", align: "center", valign: "middle", width: 120, formatter: $this.tableFormatterType, cellStyle: $this.tableCellStyleType },
            ],
            onRefresh: () => {},
        });
    }

    show(entity) {
        this.filters = {};
        this.$entity = entity;
        this.$$.tableDetail.bootstrapTable('load', entity.AccessRequestDetails);
        //this.render();
        this.$$.formModal.modal({ backdrop: 'static' });
        this.$$.tableDetail.bootstrapTable('refreshOptions', {
            height: window.innerHeight - 245
        });
    }

    __onConfirm() {
        this.$$.formModal.modal('hide');
        this.onConfirm(true);
    }

    onConfirm(value) {}

    onClose() {}

    __onClose() {
        this.onClose();
    }

    tableCellStyleCurrentResource(value, row, index, field) {
        let classes = '';
        if (!!value) {
            if (row.RequestDetType == null)
                classes = 'current-resource';
            else if (row.RequestDetType == Request.TYPE_MODIFICACION)
                classes = 'current-resource-modify'
            else if (row.RequestDetType == Request.TYPE_BAJA)
                classes = 'current-resource-delete';
        } else {
            if (row.RequestDetType == Request.TYPE_ALTA)
                classes = 'current-resource-add';
        }
        return {
            classes: classes,
            css: {}
        };
    }

    tableFormatterRecurso(value, row, index) {
        return `<strong>${row.ResourceName}</strong><br/><small>${row.ResourceFullName}</small>`;
    }

    tableFormatterValidity(value, row, index) {
        if(row.ReqDetTemporal > 0) {
            return moment(row.ReqDetValidityFrom).format('DD/MM/YYYY') + " - " + moment(row.ReqDetValidityUntil).format('DD/MM/YYYY');
        }
    }

    tableFormatterType(value, row, index) {
        let label = "";
        switch (row.RequestDetType) {
            case Request.TYPE_ALTA:
                label = `<span>Alta</span>`;
                break;
                case Request.TYPE_MODIFICACION:
                    label = `<span>Modificación</span>`;
                    break;
            case Request.TYPE_BAJA:
                label = `<span>Baja</span>`;
                break;
        }
        return label;
    }

    tableCellStyle(value, row, index, field) {
        let classes = '';
        switch (row.RequestDetType) {
            case Request.TYPE_ALTA:
                classes = 'color-resource-add';
                break;
                case Request.TYPE_MODIFICACION:
                    classes = 'color-resource-modify';
                    break;
            case Request.TYPE_BAJA:
                classes = 'color-resource-delete';
                break;
        }
        return {
            classes: classes,
            css: {}
        };
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

    tableFormatterCurrentResource(value, row, index) {
        if (!!value && row.RequestDetType == null)
            return '<i class="fas fa-equals" title="Acceso actual"></i>';
        else if (row.RequestDetType == Request.TYPE_ALTA)
            return '<i class="fas fa-plus" title="Acceso nuevo"></i>';
        else if (row.RequestDetType == Request.TYPE_MODIFICACION)
            return '<i class="fas fa-edit" title="Acceso modificado"></i>';
        else if (row.RequestDetType == Request.TYPE_BAJA)
            return '<i class="fas fa-minus" title="Acceso a dar de baja"></i>';
    }

    tableFormatterValue(value, row, index) {
        return row.RequestDetDisplayValue;
    }
}