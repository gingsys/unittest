import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class CommonValueSet extends Entity {
    static get SET_TIPO_DOCUMENTO() { return 'TIPO_DOCUMENTO'; }
    static get SET_TIPO_SOLICITUD() { return 'TIPO_SOLICITUD'; }
    static get SET_PRIORIDAD_SOLICITUD() { return 'PRIORIDAD_SOLICITUD'; }
    static get SET_AREAS() { return 'AREAS'; }
    static get SET_PROYECTOS() { return 'PROYECTOS%20COLABORADORES'; }
    static get SET_CARGOS() { return 'POSICIONES'; }
    static get SET_PAISES() { return 'PAISES'; }
    static get SET_TIPO_CLASIFICACION() { return 'TIPO_CLASIFICACIONES'; }
    static get SET_TIPO_EMPLEADO() { return 'TIPO_EMPLEADO'; }

    constructor() {
        super();
        this.CommonValueSetID = 0;
        this.CommonValueSetName = '';
        this.CommonValueSetDesc = '';
        this.CommonValues = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.CommonValues = CommonValue.fromList(this.CommonValues || []);
    }
}
export class CommonValue extends Entity {
    constructor() {
        super();
        this.CommonValueID = 0;
        this.CommonValueSetID = '';
        this.CommonValueName = '';
        this.CommonValueDesc = '';
        this.CommonValueDisplay = '';
    }
}


export class CommonValueResource extends ResourceBase {
    get api() {
        return '/api/CommonValueSets';
    }

    getValues(name, companyID = null) {
        return CommonValueResource.$get(`/api/CommonValueSets/${name}/values`, { CommonValueCompany: companyID }, this.__requestOptions);
    }

    // 
    static loadSelect(select, name, companyID = null) {
        select.innerHTML = '';
        return this.$get(`/api/CommonValueSets/${name}/values`, { CommonValueCompany: companyID }, {
                headers: {
                    'X-Auth-Token': this.session.sessionToken || {}
                }
            })
            .then(r => {
                r.sort((a, b) => a.CommonValueDisplay.localeCompare(b.CommonValueDisplay))
                .forEach(item => {
                    let opt = document.createElement('option');
                    opt.innerHTML = item.CommonValueDisplay;
                    opt.value = item.CommonValueID;
                    opt.$data = item;
                    select.appendChild(opt);
                });
                select.value = null;
            });
    }

    // método para llenar <control-select></control-select>
    static async fillControlSelectData(selectElement, name, companyID = null) {
        await this.$get(`/api/CommonValueSets/${name}/values`, { CommonValueCompany: companyID }, {
            headers: {
                'X-Auth-Token': this.session.sessionToken || {}
            }
        })
        .then(r => {
            const data = r
            .sort((a, b) => a.CommonValueDisplay.localeCompare(b.CommonValueDisplay))
            .map(cv => {
                return {
                    value: cv.CommonValueID,
                    display: cv.CommonValueDisplay,
                    data: cv
                }
            });
            selectElement.setData(data, '');
        });
    }
}