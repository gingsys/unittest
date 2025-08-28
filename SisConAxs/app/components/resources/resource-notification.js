import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class Notification extends Entity {
    constructor() {
        super();
        this.NotifConfDesc = '';
        this.NotifConfHost = '';
        this.NotifConfID = 0;
        this.NotifConfLock = '';
        this.NotifConfName = '';
        this.NotifConfPort = '';
        this.NotifConfSSL = '';
        this.NotifConfUser = '';
    }
}

export class NotificationResource extends ResourceBase {
    get api() {
        return '/api/NotifConfig';
    }

    testconf(data, prms = undefined) {
        return ResourceBase.$post(`${this.api}/test-config`, data, prms, this.__requestOptions);
    }
}