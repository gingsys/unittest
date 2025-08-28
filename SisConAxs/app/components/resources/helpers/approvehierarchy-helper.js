import { WorkflowApproveHierarchyResource } from "../resource-approvehierarchy.js";

export const ApproveHierarchyHelper = new class {
    constructor() {
        this.$rc = new WorkflowApproveHierarchyResource();
    }

    /**
     * Llena un HTMLSelect desde WorkflowApproveHierarchyResource
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {Object} options - Opciones
     * @param {Boolean} options.includeAll - Agrega el item - TODOS -
    //  * @param {Boolean} options.localdb - Usa el localdb (indexedDB) en lugar de hacer un request al servidor
     * @param {Function} options.filter - Funcion de filtro
     */
    async fillSelect(select, withDetails = false, options = {}) {
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
            await this.$rc.get({ WithDetails: withDetails })
                .then(r => this.$fillSelect(select, r, includeAll, filter));
        }
    }

    /**
     * Llena un HTMLSelect desde un array
     * @param {HTMLSelectElement} select - Elemento HTMLSelect
     * @param {Array} rdata - Array de ApproveHierarchy
     * @param {Boolean} includeAll - Agrega el item - TODOS -
     * @param {Function} filter - Funcion de filtro
     */
    $fillSelect(select, rdata, includeAll = false, filter = f => true) {
        const data = rdata
        .filter(filter)
        .sort((a, b) => a.WfApproveHierarchyName.localeCompare(b.WfApproveHierarchyName))
        .map(item => {
            return {
                value: item.WfApproveHierarchyID,
                display: item.WfApproveHierarchyName,
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