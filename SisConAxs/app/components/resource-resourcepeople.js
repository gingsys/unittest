import { Entity } from '../core.js'
import { ResourceBase } from './resource-base.js'

export class ResourcePeople extends Entity {
    constructor() {
        super();
        this.AddedRequestDetID = null;
        this.AddedRequestID = null;
        this.AddedRequestNum = null;
        this.PeopleDepartment = null;
        this.PeopleID = 0;
        this.PresActive = 0;
        this.PresAdditional = 0;
        this.PresAdditionalIntValue = 0;
        this.PresAdditionalStrValue = null;
        this.PresCompany = 0;
        this.PresCompanyName = "";
        this.PresDateEnd = null;
        this.PresDateStart = null;
        this.PresDisplayValue = null;
        this.PresID = 0;
        this.PresTemporal = 0;
        this.PresValidityFrom = null;
        this.PresValidityUntil = null;
        this.RemovedRequestDetID = null;
        this.RemovedRequestID = null;
        this.RemovedRequestNum = null;
        this.CategoryName = "";
        this.ResourceFullName = "";
        this.ResourceID = 0;
        this.ResourcePeopleLog = [];
    }

    get ResourceActiveDesc() {
        return this.ResourceActive == 0? 'Inactivo' : 'Activo';
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.ResourcePeopleLog = ResourcePeopleLog.fromList(this.ResourcePeopleLog || []);
    }
}
export class ResourcePeopleLog extends Entity {
    constructor() {
        super();
        this.Action = 0;
        this.ActionDisplay = "";
        this.Source = "";
        this.Description = "";
        this.CreateDate = null;
        this.CreateUser = null;
    }
}
export class ResourcePeopleResource extends ResourceBase {
    get api() {
        return '/api/resource/people';
    }

    getByPeopleID(id, params = {}) {
        return ResourcePeopleResource.$get(`/api/resource/people/${id}`, params, {
            headers: {
                'X-Auth-Token': this.constructor.session.sessionToken || {}
            }
        });
    }
}