import { Component, html } from '../core.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';

export class RequestDetailHistoryComponent extends Component {
    constructor(presenter) {
        super(presenter);

        this.$entity = {};
        this.$detail = {};
        this.$resource = new RequestResource();
    }

    __render() {
        return html`
        <style>
            .resource-fullname {
                width: 100%;
                display: block;
                background-color: aquamarine;
                padding: .35em;
                margin: 0;
            }
        </style>
        <div class="modal" id="formRequestDetailHistoryModal" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form">
                        <div class="modal-header">
                            <h4 class="modal-title"><i class="fas fa-history"></i> Historial Detalle de Solicitud : ${this.formatNumber(this.$entity.RequestNumber || 0, 8)}</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div id="toolbarRequestDetailHistory">
                                <h5 class="resource-fullname">${this.$detail.ResourceFullName}</h5>
                            </div>
                            <table
                                id="table"
                                data-locale="es-ES"
                                data-search="true"
                                data-show-refresh="true"
                                >
                                <thead>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <div class="mr-auto">
                                <strong>Para: ${this.$entity.RequestToName}</strong>
                            </div>
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cerrar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }

    __ready() {
        let $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - 250,
            toolbar: this.$.toolbarRequestDetailHistory,
            columns: [
                // { title:"Fecha y Hora", field:"HistoryDate", align:"left", valign:"middle", formatter: $this.tableFormatterTimestamp },
                { title:"Evento", field:"HistoryMessage", align:"left", valign:"middle" },
                // { title:"", field:"", align:"center", valign:"middle", width:"120", formatter: $this.tableFormatterOperate, events:$this.tableEventColumnOperate }
            ],
            onRefresh: () => {
                this.loadData();
            }
        });
    }

    show(entity, detail) {
        this.$entity = entity;
        this.$detail = detail;
        this.$$.table.bootstrapTable('load', []);
        this.$$.table.bootstrapTable('resetSearch');
        this.render();
        this.loadData();
        this.$$.formRequestDetailHistoryModal.modal({ backdrop: 'static' });
    }

    loadData() {
        this.$presenter.showLoading();
        this.$resource.getDetailHistory(this.$detail.RequestDetID).then(r => {
            this.$$.table.bootstrapTable('load', r);
            this.$presenter.hideLoading();
            iziToast.success({title: 'Cargado correctamente'});
        },
        err => {
            this.$presenter.hideLoading();
            iziToast.error({title:'Error al cargar'});
        });
    }

    formatNumber(number, width = 8) {
        return new Array(width - number.toString().length + 1).join('0') + number;
    }

    tableFormatterTimestamp(value, row, index) {
        return moment(value).format('DD/MM/YYYY HH:mm:ss');
    }
}