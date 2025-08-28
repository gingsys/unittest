import { Entity } from '../../core/core.js'
import { ResourceBase } from './resource-base.js'

export class AccessType extends Entity {
    static get TYPE_SIMPLE() { return 1; }
    static get TYPE_TEXTO() { return 2; }
    static get TYPE_SELECT_SIMPLE() { return 3; }
    static get TYPE_SELECT_MULTIPLE() { return 4; }
    static get TYPE_ENTERPRISE_EMAIL() { return 5; }

    constructor() {
        super();
        this.AccessTypeID = 0;
        this.AccessTypeName = '';
        this.AccessTypeType = 1;
        this.AccessTypeValues = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.AccessTypeValues = AccessTypeValue.fromList(this.AccessTypeValues || []);
    }
}
export class AccessTypeValue extends Entity {
    constructor() {
        super();
        this.AccessTypeID = 0;
        this.TypeValueAdditional = 0;
        this.TypeValueCharVal = '';
        this.TypeValueDefault = 0;
        this.TypeValueDisplay = '';
        this.TypeValueID = 0;
        this.TypeValueIntVal = null;
        this.TypeValueName = '';
    }

    get TypeValue() {
        return this.TypeValueCharVal;
    }
    set TypeValue(value) {
        return this.TypeValueCharVal = value;
    }
}
export class AccessTypeResource extends ResourceBase {
    get api() {
        return '/api/AccessTypes';
    }
}