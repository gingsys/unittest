import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class Calendar extends Entity {
    constructor() {
        super();
        this.CalID = 0;
        this.CalDate = '';
        this.CalDescription = '';
        this.CalAnual = false;
        this.CalIdCountry = 0;
    }
}
export class CalendarFilter extends Entity {
    constructor() {
        super();
        this.country = 0;
        this.year = 0;
        this.status = 0;
    }
}

export class CalendarResource extends ResourceBase {
    get api() {
        return '/api/Calendar';
    }

    listCalendar(params) {
        return CalendarResource.$get(`${this.api}/GetCalendar/`, params, this.__requestOptions);
    }
}