import { CommonValueResource } from "../resource-commonvalue.js";

export const CommonValueHelper = new class {
    constructor() {
        this.$rc = new CommonValueResource();
    }

    /**
     * Llena un HTMLSelect desde CommonValueResource por Conjunto de Valores
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {String} name - Nombre Conjunto de Valores
     * @param {Object} options - Opciones
     * @param {Boolean} options.includeAll - Agrega el item - TODOS -
    //  * @param {Boolean} options.localdb - Usa el localdb (indexedDB) en lugar de hacer un request al servidor
     * @param {Function} options.filter - Funcion de filtro
     */
    async fillSelectBySetName(select, name, options = {}) {
        await this.fillSelectBySetNameCompany(select, name, null, options);
    }

    /**
     * Llena un HTMLSelect desde CommonValueResource por Conjunto de Valores
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {String} name - Nombre Conjunto de Valores
     * @param {Number} name - ID empresa
     * @param {Object} options - Opciones
     * @param {Boolean} options.includeAll - Agrega el item - TODOS -
    //  * @param {Boolean} options.localdb - Usa el localdb (indexedDB) en lugar de hacer un request al servidor
     * @param {Function} options.filter - Funcion de filtro
     */
    async fillSelectBySetNameCompany(select, name, companyID, options = {}) {
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
            const fdata = data.filter(p => p.GrupoParametroID == name)
            this.$fillSelect(select, fdata, includeAll, filter);
        }
        // else if(!!localdb) {
        //     await this.$rc.localStorage.query(p => p.GrupoParametroID == name)
        //         .then(_data => this.$fillSelect(select, _data, includeAll, filter));
        // }
        else {
            await this.$rc.getValues(name, companyID)
                .then(r => this.$fillSelect(select, r, includeAll, filter));
        }
    }

    /**
     * Llena un HTMLSelect desde un array
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {Array} rdata - Array de CommonValue
     * @param {Boolean} includeAll - Agrega el item - TODOS -
     * @param {Function} filter - Funcion de filtro
     */
    $fillSelect(select, rdata, includeAll = false, filter = f => true) {
        const data = rdata
        .filter(filter)
        .sort((a, b) => a.CommonValueName.localeCompare(b.CommonValueName))
        .map(item => {
            return {
                value: item.CommonValueID,
                display: item.CommonValueName,
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