import { Component, html } from '../../../core/core.js';
import { BootstrapTableUtils } from './bootstraptable-utils.js';
import { LocaleUtils } from '../../common/locale-utils.js';

export class UIMainComponent extends Component {
    constructor(options) {
        super(false);
        this.__options = Object.assign({
            pagination: true
        }, options);

        this.$resource = new this.Resource();
        this.$entity = new this.Entity();
    }

    get Resource() {
        return null;
    }
    get Entity() {
        return null;
    }
    get entityID() {
        return '';
    }
    get table() {
        return this.querySelector('[data-table-main]');
    }
    get tableColumns() {
        return [];
    }

    connectedCallback() {
        super.connectedCallback();
        this.createTable();
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        $(this.$all('.modal')).modal('hide');
    }

    createTable() {
        $(this.table).bootstrapTable({
            toolbar: this.querySelector('[data-table-toolbar]'),
            height: window.innerHeight - 130,
            filterControl: true,
            searchOnEnterKey: true,
            // showRefresh: "true",
            sidePagination: !!this.__options.pagination? "server" : "client",
            pagination: !!this.__options.pagination,
            pageSize: 12,
            pageList: "[12, 25, 50, 100]",
            columns: this.tableColumns,
            onDblClickRow: (row, element, field) => {
                this.edit(row[this.entityID]);
            },
            onRefresh: () => {
                this.loadData();
            },
            // onColumnSearch: (filterColumn, textColumn) => {
                // Nota: no es necesario porque se ejecuta onPageChange
            // },
            onPageChange: () => {
                this.loadData();
            }
        });
    }

    prepareFilters(filters) {
        const controls = Array.from(this.$all('[data-filter]'));
        controls.forEach(c => {
            if(c.value !== '') {
                filters[c.dataset.filter] = c.value;
            }
        });
        return filters;
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
                if(this.__options.pagination) {
                    $(this.table).bootstrapTable('load', {
                        rows: data.Rows, //this.Entity.fromList(data.Rows || []),
                        total: data.TotalRows
                    });
                } else {
                    $(this.table).bootstrapTable('load', data);
                }
                BootstrapTableUtils.setFilters(this.table, filters);
                this.$presenter.hideLoading();
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: 'Error al cargar' });
            }
        );
    }

    add() {
        this.$.editor.add();
    }
    edit(id) {
        this.$presenter.showLoading();
        this.$resource.getById(id).then(
            //async ({data}) => {
            async data => {
                let dto = data;
                if(Array.isArray(data)) {
                    dto = data[0];
                }
                const entity = this.Entity.fromObject(dto);
                await this.$.editor.edit(entity);
                this.$presenter.hideLoading();
            },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: 'Error al cargar' });
            }
        );
    }
    
    tableFormatterActive(value, row, index) {
        if(!!value) {
            return 'Activo';
        } else {
            return 'Inactivo';
        }
    }
    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        this.edit(row[this.entityID]);
    }
}