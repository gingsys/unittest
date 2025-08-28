import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class Category extends Entity {
    constructor() {
        super();
        this.CategoryID = 0;
        this.CategoryName = '';
        this.CategoryDescription = '';
    }
}
export class CategoryResource extends ResourceBase {
    get api() {
        return '/api/ResourceCategories';
    }

    static loadSelect(select, name) {
        select.innerHTML = '';
        return this.$get(`/api/ResourceCategories`, undefined, {
            headers: {
                'X-Auth-Token': this.session.sessionToken || {}
            }
        }).then(r => {
            r.forEach(item => {
                let opt = document.createElement('option');
                opt.innerHTML = item.CategoryName;
                opt.value = item.CategoryID;
                opt.$data = item;
                select.appendChild(opt);
            });
            select.value = null;
        });
    }
}