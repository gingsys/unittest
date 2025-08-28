import { Resource } from '../core.js';
import { Session } from './resources/resource-session.js';

export class ResourceBase extends Resource {
    static get session() {
        let entity = new Session();
        let dto = JSON.parse(window.localStorage.getItem('auth')) || {};
        entity.fromDTO(dto);
        return entity;
    }

    getById(id) {
        return Resource.$get.apply(this, [`${this.api}/${id}`, undefined, {
            headers: {
                'X-Auth-Token': ResourceBase.session.sessionToken || {}
            }
        }]);
    }

    get(params) {
        return Resource.$get.apply(this, [this.api, params, {
            headers: {
                'X-Auth-Token': ResourceBase.session.sessionToken || {}
            }
        }]);
    }

    post(data, params) {
        return Resource.$post.apply(this, [this.api, data, params, {
            headers: {
                'X-Auth-Token': ResourceBase.session.sessionToken || {}
            }
        }]);
    }

    delete(params) {
        return Resource.$delete.apply(this, [this.api, params, {
            headers: {
                'X-Auth-Token': ResourceBase.session.sessionToken || {}
            }
        }]);
    }
}