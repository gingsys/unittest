import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';
//import { AccessTypeValue } from '../resource-accesstype.js';
import { Resource } from '../resource-resource.js';;

export class RequestTemplate extends Entity {
    static get TYPE_ALTA() { return 46; }
    static get TYPE_BAJA() { return 47; }
    static get TYPE_MODIFICACION() { return 48; }

    constructor() {
        super();
        this.ReqTemplateID = 0;
        this.ReqTemplateCompany = 0;
        this.ReqTemplateCompanyName = '';
        this.ReqTemplateType = RequestTemplate.TYPE_ALTA;
        this.ReqTemplateTypeName = '';
        this.ReqTemplateEmployeeType = 0;
        this.ReqTemplateEmployeeTypeName = '';
        this.ReqTemplateActive = true;
        this.ReqTemplateDetails = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.ReqTemplateDetails = RequestTemplateDetail.fromList(this.ReqTemplateDetails || []);
    }
}
export class RequestTemplateDetail extends Entity {

    constructor() {
        super();
        this.ReqTemplateID = 0;
        this.ReqTemplateDetID = 0;

        this.ReqTemplateDetCategoryID = 0;
        this.ReqTemplateDetCategoryName = '';
        this.ReqTemplateDetResourceID = 0;
        this.ReqTemplateDetResourceName = '';
        this.ReqTemplateDetResourceFullName = '';
        this.ReqTemplateDetAccessTypeID = 0;

        this.ReqTemplateDetStrValue = '';
        this.ReqTemplateDetIntValue = null;
        this.ReqTemplateDetTemporal = 0;
        this.ReqTemplateDetValidityFrom = null;
        this.ReqTemplateDetValidityUntil = null;
        this.ReqTemplateDetAdditional = 0;
        this.ReqTemplateDetAdditionalStrValue = '';
        this.ReqTemplateDetAdditionalIntValue = 0;
    }

    // get RequestDetValidityDisplay() {
    //     if (this.ReqDetValidityFrom != null)
    //         return moment(this.ReqDetValidityFrom).format('DD/MM/YYYY') + " - " + moment(this.ReqDetValidityUntil).format('DD/MM/YYYY');
    // }

    fromDTO(dto) {
        super.fromDTO(dto);
        // this.ResourceAccessTypeValues = AccessTypeValue.fromList(this.ResourceAccessTypeValues || []);
    }


    fromResource(dto) {
        this.ReqTemplateID = 0;
        this.ReqTemplateDetID = 0;

        this.ReqTemplateDetCategoryID       = dto.ResourceCategoryID;
        this.ReqTemplateDetCategoryName     = dto.ResourceCategoryName;
        this.ReqTemplateDetResourceID       = dto.ResourceID;
        this.ReqTemplateDetResourceName     = dto.ResourceName;
        this.ReqTemplateDetResourceFullName = dto.ResourceFullName;
        this.ReqTemplateDetAccessTypeID     = dto.ResourceAccessType;

        this.ReqTemplateDetStrValue           = '';
        this.ReqTemplateDetIntValue           = null;
        this.ReqTemplateDetTemporal           = dto.ResourceTemporal == Resource.TEMPORAL_REQUIRED? 1 : 0;  //dto.ResourceTemporal;
        this.ReqTemplateDetValidityFrom       = null;
        this.ReqTemplateDetValidityUntil      = null;
        this.ReqTemplateDetAdditional         = 0;
        this.ReqTemplateDetAdditionalStrValue = '';
        this.ReqTemplateDetAdditionalIntValue = 0;
    }
    static fromResourceList(dtoList) {
        const list = dtoList || [];
        return list.map(r => {
            const detail = new RequestTemplateDetail();
            detail.fromResource(r);
            return detail;
        });
    }
}

export class RequestTemplateResource extends ResourceBase {
    get api() {
        return '/api/request-template';
    }

    // getPaginate(params = undefined) {
    //     return this.constructor.$get(`/api/request-template/paginate`, params, this.__requestOptions);
    // }
}