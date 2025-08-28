import { Component, html } from '../../../core/core.js';
import { IntegrationOracleLog, IntegrationOracleResource } from '../../resources/resource-integration-oracle.js';
import { SessionResource } from '../../resources/resource-session.js';

export class IntegrationOracleComponent extends Component {
    constructor() {
        super(false);
        this.$resource = new IntegrationOracleResource();
        this.filters = {
            LogActive: 1
        }
    }

    get $template() {
        return html`
        <style>
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

            .detail-view td, .subtable {
                margin: 0;
                padding: 0 !important;
            }
            .subtable {
                font-size: 1.2em;
                margin-left: 28px;
            }
            .btn-show {
                width: 90px;
            }
        </style>
        <h3>Integración Oracle</h3>
        ${SessionResource.session.UserRoleSysAdmin > 0? html`
        <div class="form-group">
            <div class="input-group">
                <div class="input-group-prepend">
                    <span class="input-group-text"><i class="fas fa-sync-alt mr-2"></i> Sincronización</span>
                </div>
                <div class="input-group-append">
                    <button class="btn btn-outline-info" @click="${this.syncMetadata}">
                        <i class="fas fa-table"></i> Datos
                    </button>
                    <button class="btn btn-outline-info" @click="${this.syncResources}">
                        <i class="fas fa-laptop"></i> Recursos
                    </button>
                    <button class="btn btn-outline-info" @click="${this.syncAccess}">
                        <i class="fas fa-hand-holding"></i> Accesos
                    </button>
                </div>
            </div>
        </div>` : ''}
        <div id="toolbar" class="btn-toolbar">
            <div class="input-group mr-2" data-toggle="buttons">
                <div class="input-group-prepend">
                    <label class="input-group-text" for="cboType"><i class="far fa-file-alt mr-2"></i> Log Oracle</label>
                </div>
                <select class="custom-select" id="cboType" @change="${this.loadData}">
                    <option value="UNMATCH_PEOPLE">Personas no coincidentes</option>
                    <option value="UNMATCH_RESPONSABILITY">Responsabilidades no coincidentes</option>
                </select>
            </div>
            <div class="btn-group" data-toggle="buttons">
                <button class="btn btn-outline-success active" id="optFilterStatusActive" @click="${this.optFilterStatusActiveClick}">
                    <i class="fas fa-check"></i> Activos <input type="radio" name="optFilterStatus" value="1" autocomplete="off" checked style="display: none;" />
                </button>
                <button class="btn btn-outline-secondary" id="optFilterStatusDeactive" @click="${this.optFilterStatusDeactiveClick}">
                    <i class="fas fa-minus-circle"></i> Inactivos <input type="radio" name="optFilterStatus" value="1" autocomplete="off" checked style="display: none;" />
                </button>
                <button class="btn btn-outline-dark" id="optFilterStatusAll" @click="${this.optFilterStatusAllClick}">
                    <i class="fas fa-border-all"></i> Todos <input type="radio" name="optFilterStatus" value="0" autocomplete="off" style="display: none;" />
                </button>
            </div>
        </div>
        <table
            id="table"
            data-locale="es-ES"
            data-toolbar="#toolbar"
            data-search="true"
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
        </table>`;
    }

    connectedCallback() {
        super.connectedCallback();

        const $this = this;
        this.$$.table.bootstrapTable({
            height: window.innerHeight - this.$.table.offsetTop - 45,
            columns: [
                { title: "Código", field: "LogData1", filterControl: "input", filterControlPlaceholder: "Código", align: "center", valign: "middle", width: 120, sortable: true },
                { title: "Nombre", field: "LogData2", filterControl: "input", filterControlPlaceholder: "Nombre", align: "left", valign: "middle", sortable: true },
                { title: "Fecha Registro", field: "CreateDate", filterControl: "input", filterControlPlaceholder: "Fecha Registro", align: "center", valign: "middle", formatter: $this.tableFormatterDate },
                { title: "Estado", field:"LogActive", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus }
            ],
            onRefresh: () => {
                this.loadData();
            }
        });
        this.loadData();
    }

    tableFormatterDate(value, row, index) {
        return moment(value).format('DD/MM/YYYY');
    }
    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }

    optFilterStatusActiveClick() {
        this.filters.LogActive = 1;
        this.loadData();
    }
    optFilterStatusDeactiveClick() {
        this.filters.LogActive = 0;
        this.loadData();
    }
    optFilterStatusAllClick() {
        this.filters.LogActive = -1;
        this.loadData();
    }

    loadData() {
        this.$presenter.showLoading();
        this.$resource.getLogByType(this.$.cboType.value, this.filters).then(r => {
                this.$$.table.bootstrapTable('load', IntegrationOracleLog.fromList(r));
                this.$presenter.hideLoading();
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err });
            });
    }

    syncMetadata() {
        this.$presenter.question(
            'Sincronizar Datos',
            'Desea sincronizar la metadata desde Oracle?, esta operación puede tardar varios minutos',
            (instance, toast) => {
                this.$presenter.showLoading();
                this.$resource.syncMetadata().then(r => {
                    this.$presenter.hideLoading();
                    iziToast.success({ title: 'Ejecutado correctamente' });
                },
                err => {
                    this.$presenter.hideLoading();
                    iziToast.error({ title: err });
                });
            }
        );
    }
    syncResources() {
        this.$presenter.question(
            'Sincronizar Recursos',
            'Desea sincronizar los recursos desde Oracle?, esta operación puede tardar varios minutos',
            (instance, toast) => {
                this.$presenter.showLoading();
                this.$resource.syncResources().then(r => {
                    this.$presenter.hideLoading();
                    iziToast.success({ title: 'Ejecutado correctamente' });
                },
                err => {
                    this.$presenter.hideLoading();
                    iziToast.error({ title: err });
                });
            }
        );
    }
    syncAccess() {
        this.$presenter.question(
            'Sincronizar Accesos',
            'Desea sincronizar los accesos desce Oracle?, esta operación puede tardar varios minutos',
            (instance, toast) => {
                this.$presenter.showLoading();
                this.$resource.syncAccess().then(r => {
                    this.$presenter.hideLoading();
                    iziToast.success({ title: 'Ejecutado correctamente' });
                },
                err => {
                    this.$presenter.hideLoading();
                    iziToast.error({ title: err });
                });
            }
        );
    }
}
customElements.define('integration-oracle-main', IntegrationOracleComponent);