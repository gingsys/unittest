import { CompanyResource } from "../resource-company.js";

export const CompanyHelper = new class {
    constructor() {
        this.$rc = new CompanyResource();
    }

    /**
     * Llena un HTMLSelect desde CompanyResource
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {Object} options - Opciones
     * @param {Boolean} options.includeAll - Agrega el item - TODOS -
    //  * @param {Boolean} options.localdb - Usa el localdb (indexedDB) en lugar de hacer un request al servidor
     * @param {Function} options.filter - Funcion de filtro
     */
    async fillSelect(select, options = {}) {
        const {
            includeAll,
            localdb,
            data,
            filter
        } = Object.assign({
                includeAll: false,
                // localdb: false,
                data: null,
                filter: p => true
            }, options);

        if (!!data) {
            this.$fillSelect(select, data, includeAll, filter);
        }
        // else if(!!localdb) {
        //     await this.$rc.localStorage.query(p => p.GrupoParametroID == name)
        //         .then(_data => this.$fillSelect(select, _data, includeAll, filter));
        // }
        else {
            await this.$rc.get()
                .then(r => this.$fillSelect(select, r, includeAll, filter));
        }
    }

    /**
     * Llena un HTMLSelect desde un array
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {Array} rdata - Array de Company
     * @param {Boolean} includeAll - Agrega el item - TODOS -
     * @param {Function} filter - Funcion de filtro
     */
    $fillSelect(select, rdata, includeAll = false, filter = f => true) {
        const data = rdata
        .filter(filter)
        .sort((a, b) => a.CompanyDisplay.localeCompare(b.CompanyDisplay))
        .map(item => {
            return {
                value: item.CompanyID,
                display: item.CompanyDisplay,
                data: item
            }
        });
        if(!!includeAll) {
            data.unshift({
                value: '',
                display: '- TODOS -'
            });
        }
        select.setData(data, '');
    }
}