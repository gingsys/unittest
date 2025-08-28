import { Resource } from '../../core/core.js';
import { Session } from './entity-session.js';

export class ResourceBase extends Resource {
    static get session() {
        const entity = new Session();
        const dto = JSON.parse(window.localStorage.getItem('auth')) || {};
        entity.fromDTO(dto);
        return entity;
    }

    constructor(params = {}) {
        super(
            Object.assign(params, {
                request: {
                    headers: {
                        get 'X-Auth-Token'() { return ResourceBase.session.sessionToken || {}; }
                    }
                }
            })
        );
    }
}