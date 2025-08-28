import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class Company extends Entity {
    constructor() {
        super();
        this.CompanyID = 0;
        this.CompanyTaxpayerID = '';
        this.CompanyName = '';
        this.CompanyDisplay = '';
        this.CompanyCountry = 0;
        this.CompanyCountryName = '';
        //this.CompanyAD = '';
        this.CompanyAddress = '';
        this.CompanyActive = true;

        // this.CompanyParameters = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        // this.CompanyParameters = CompanyParameter.fromList(this.CompanyParameters || []);
    }
}
export class CompanyParameter extends Entity {
    constructor() {
        super();
        this.CompanyID = 0;
        this.CompanyParameterID = 0;
        this.CompanyParameterDisplay = '';
        this.Value = '';
        // this.ValueInt = 0;
        // this.ValueDate = null;
    }
}
export class CompanyResource extends ResourceBase {
    get api() {
        return '/api/Company';
    }

    static loadSelect(select) {
        select.innerHTML = '';
        return this.$get(`/api/Company`, undefined, {
            headers: {
                'X-Auth-Token': this.session.sessionToken || {}
            }
        }).then(r => {
            r.filter(x => x.CompanyID > 0).forEach(item => {
                let opt = document.createElement('option');
                opt.innerHTML = item.CompanyDisplay;
                opt.value = item.CompanyID;
                opt.$data = item;
                select.appendChild(opt);
            });
            select.value = null;
        });
    }
}