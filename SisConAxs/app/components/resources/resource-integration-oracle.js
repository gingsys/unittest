import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class IntegrationOracleLog extends Entity {
    static get TYPE_UNMATCH_PEOPLE() { return "UNMATCH_PEOPLE"; }
    static get TYPE_UNMATCH_RESPONSABILITY() { return "UNMATCH_RESPONSABILITY"; }

    constructor() {
        super();
        this.LogID = 0;
        this.LogType = '';
        this.LogData1 = '';
        this.LogData2 = '';
        this.LogMessage = '';
        this.LogActive = 1;
        this.CreateDate = null;
    }

    fromDTO(dto) {
        super.fromDTO(dto);
    }
}
export class IntegrationOracleResource extends ResourceBase {
    get api() {
        return '/api/integration/oracle';
    }

    getLogByType(type, params) {
        return IntegrationOracleResource.$get(`${this.api}/log/${type}`, params, this.__requestOptions);
    }

    checkResponsabilityConflicts(user, responsabilites) {
        return IntegrationOracleResource.$post(`${this.api}/check-responsability-conflicts/${user}`, responsabilites, undefined, this.__requestOptions);
    }

    syncMetadata() {
        return IntegrationOracleResource.$post(`${this.api}/sync-metadata`, null, undefined, this.__requestOptions);
    }
    syncResources() {
        return IntegrationOracleResource.$post(`${this.api}/sync-resources`, null, undefined, this.__requestOptions);
    }
    syncAccess() {
        return IntegrationOracleResource.$post(`${this.api}/sync-access`, null, undefined, this.__requestOptions);
    }
}