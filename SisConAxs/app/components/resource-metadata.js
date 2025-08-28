import { Entity } from '../core.js'
import { ResourceBase } from './resource-base.js'

export class Metadata extends Entity {
    static get ORACLE_COMPANIES() { return 10000; }
    static get ORACLE_PROJECTS() { return 10001; }
    static get ORACLE_PROFILES() { return 10002; }
    static get ORACLE_RESPONSABILITIES() { return 10003; }
    static get ICARUS_ACCESS() { return 25582; }

    constructor() {
        super();
        this.MetadataID = 0;
        this.MetadataParentID = null;
        this.MetadataDisplay = '';
        this.MetadataDescription = '';
        this.MetadataInt1 = 0;
        this.MetadataInt2 = 0;
        this.MetadataInt3 = 0;
        this.MetadataStr1 = '';
        this.MetadataStr2 = '';
        this.MetadataStr3 = '';
        this.MetadataDatetime1 = null;
        this.MetadataDatetime2 = null;
        this.MetadataDatetime3 = null;
        this.MetadataActive = 0;;
    }
}
export class MetadataResource extends ResourceBase {
    get api() {
        return '/api/metadata';
    }

    getMetadataByParentID(parentID) {
        return MetadataResource.$get(`api/metadata/${parentID}/list`, undefined, {
            headers: {
                'X-Auth-Token': this.constructor.session.sessionToken || {}
            }
        })
    }

    static loadSelect(select, parentID, fn = null) {
        select.innerHTML = '';
        return (new MetadataResource()).getMetadataByParentID(parentID)
            .then(r => {
                r.sort((a, b) => a.MetadataDisplay.localeCompare(b.MetadataDisplay))
                .forEach(item => {
                    let opt = document.createElement('option');
                    if(fn != null)
                        fn(opt, item);
                    else
                        fnSetData(opt, item);
                    opt.$data = item;
                    select.appendChild(opt);
                });
                select.value = null;
            });

        function fnSetData(opt, item) {
            opt.innerHTML = item.MetadataDisplay;
            opt.value = item.MetadataID;
        }
    }
}