import { Entity } from '../core.js'
import { ResourceBase } from './resource-base.js'

export class Resource extends Entity {
    static get TEMPORAL_NO() { return 0; }
    static get TEMPORAL_REQUIRED() { return 1; }
    static get TEMPORAL_OPTIONAL() { return 2; }

    constructor() {
        super();
        this.ResourceAccessType = 0;
        this.ResourceAccessTypeName = "";
        this.ResourceActive = 0;
        this.ResourceCategory = 0;
        this.ResourceCategoryName = "";
        this.ResourceDepartment = null;
        this.ResourceDepartmentName = null;
        this.ResourceDescription = null;
        this.ResourceFlag = 0;
        this.ResourceFullName = "";
        this.ResourceID = 0;
        this.ResourceLevel = 0;
        this.ResourceName = "";
        this.ResourceOnlyAssignable = 0;
        this.ResourceParent = null;
        this.ResourceParentName = "";
        this.ResourceRequired = 0;
        this.ResourceRequiredName = "";
        this.ResourceSendAtEnd = 0;
        this.ResourceTemporal = 0;
        this.ResourceWorkflow = 0;
        this.ResourceWorkflowName = "";

        this.ResourceParameters = [];
    }

    get ResourceActiveDesc() {
        return this.ResourceActive == 0? 'Inactivo' : 'Activo';
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.ResourceParameters = ResourceParameter.fromList(this.ResourceParameters || []);
    }
}
export class ResourceParameter extends Entity {
    static get INTEGRACION_ORACLE_COMPANY_MOUNT() { return 1201; }
    static get INTEGRACION_ORACLE_ID() { return 1202; }
    static get INTEGRATION_ICARUS_ACCESS() { return 2958; }
    // static get INTEGRACION_SRA_COMPANY_MOUNT() { return 0; }
    // static get INTEGRACION_SRA_ID() { return 0; }

    constructor() {
        super();
        this.ResourceID = 0;
        this.ResourceParameterID = 0;
        this.ResourceParameterDisplay = '';
        this.ResourceParameterMetadataID = null;
        this.Value = '';
        this.ValueDisplay = '';
        // this.ValueInt = 0;
        // this.ValueDate = null;
    }
}
export class ResourceResource extends ResourceBase {
    get api() {
        return '/api/AccessResources';
    }

    saveMultiple(data) {
        return ResourceResource.$post(`/api/AccessResources/multiple/`, data, undefined, {
            headers: {
                'X-Auth-Token': this.constructor.session.sessionToken || {}
            }
        });
    }

    static loadSelect(select) {
        select.innerHTML = '';
        return this.$get(`/api/AccessResources`, undefined, {
            headers: {
                'X-Auth-Token': this.session.sessionToken || {}
            }
        }).then(r => {
            r.forEach(item => {
                let opt = document.createElement('option');
                opt.innerHTML = item.ResourceName;
                opt.value = item.ResourceID;
                opt.dataset.content = `<span class='badge badge-success'>${item.ResourceCategoryName}</span> ${item.ResourceName} <i class="fas fa-angle-right"></i><small>${item.ResourceFullName}</small>`;
                opt.$data = item;
                select.appendChild(opt);
            });
            select.value = null;
        });
    }
}