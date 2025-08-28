import { LocaleUtils } from "../../common/locale-utils.js";

export const BootstrapTableUtils = {
    getFilters: (table) => {
        return $(table).bootstrapTable('getOptions').valuesFilterControl;
    },
    prepareFilters: (table) => {
        const filters = {};
        const options = $(table).bootstrapTable('getOptions');
        options.valuesFilterControl
            .filter(v => v.value.trim() != '')
            .forEach(v => {
                filters[v.field] = v.value;
            });
        filters.CurrentPage = options.pageNumber;
        filters.PageSize = options.pageSize;
        return filters;
    },
    setFilters: (table, filters) => {
        for (let fieldName in filters) {
            const field = table.querySelector(`[data-field='${fieldName}']`);
            if(field != null) {
                const control = field.querySelector('input');
                if (control != null) {
                    control.value = filters[fieldName];
                    control.addEventListener('click', function(e) {
                        e.target.focus();
                        e.target.select();
                    });
                }
            }
        }
    },

    formatterNumber: (value, row, index) => {
        if(value != null) {
            return LocaleUtils.formatNumber(value);
        }
        return value;
    },
    formatterDate: (value, row, index) => {
        if(value != null) {
            return moment(value).format('DD/MM/YYYY');
        }
        return value;
    },
    formatterActive: (value, row, index) => {
        const className = value? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    },
    // formatterOperate: (value, row, index) => {
    //     return `<div class="btn-group btn-group-sm" role="group">
    //                 <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
    //             </div>`;
    // }

    formatterPadZero(num) {
        return (value, row, index) => {
            if(value != null) {
                return String(value).padStart(num, '0');
            }
            return value;
        }
    }
}