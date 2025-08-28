import { Component, html } from '../../../core/core.js';
import { UIEditComponent } from '../common/ui-edit.js';
import { Company, CompanyParameter, CompanyResource } from '../../resources/resource-company.js';
import { CommonValueSet, CommonValueResource } from '../../resources/resource-commonvalue.js';
import { SelectCommonValue } from '../commonvalues/ui-commonvalue.js';
import { CompanyEditView } from './ui-company-edit.view.js';

export class CompanyEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
        this.$resource = new CompanyResource();
        this.$entity = new Company();
    }

    get $template() {
        return new CompanyEditView(this);
    }

    get Resource() {
        return CompanyResource;
    }
    get Entity() {
        return Company;
    }
    get entityID() {
        return 'CompanyID'
    }

    connectedCallback() {
        super.connectedCallback();

        this.selectCountry = new SelectCommonValue(this.$.cboCountry, CommonValueSet.SET_PAISES);
        // this.$$.tabData.on('shown.bs.tab', (e) => {
        //     // 
        // });
        // this.$$.tabParams.on('shown.bs.tab', (e) => {
        //     this.$.companyParams.onShow();
        // });
    }

    async edit(entity) {
        this.$entity = entity;
        await this.selectCountry.refresh();
        this.EntityToForm();
        this.render();

        // this.$$.tabData.tab('show');
        this.$$.formModal.modal({ backdrop: 'static' });
        this.$.txtTaxpayerID.select();
    }

    EntityToForm() {
        this.$.txtTaxpayerID.value     = this.$entity.CompanyTaxpayerID;
        this.$.txtName.value           = this.$entity.CompanyName;
        this.$.txtDisplay.value        = this.$entity.CompanyDisplay;
        this.$.txtAddress.value        = this.$entity.CompanyAddress;
        this.selectCountry.value       = this.$entity.CompanyCountry;
        this.$.chkActive.checked       = this.$entity.CompanyActive > 0;
    }
    FormToEntity() {
        this.$entity.CompanyTaxpayerID = this.$.txtTaxpayerID.value.trim();
        this.$entity.CompanyName       = this.$.txtName.value.trim();
        this.$entity.CompanyDisplay    = this.$.txtDisplay.value.trim();
        this.$entity.CompanyAddress    = this.$.txtAddress.value.trim();
        this.$entity.CompanyCountry    = this.selectCountry.value;
        this.$entity.CompanyActive     = this.$.chkActive.checked? 1 : 0;
    }

    validate(e) {
        return true;
    }
}
customElements.define('company-edit', CompanyEditComponent);







class CompanyParamsComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.$data = [];
        // this.$entity = null;
        this.$resourceCommonValue = new CommonValueResource();
    }

    get $template() {
        return html`
            <div id="toolbarTableParams">
                <div class="row">
                    <div class="col-md-12">
                        <div class="btn-toolbar mb-3" role="toolbar" aria-label="Toolbar with button groups">
                            <div class="input-group mr-2">
                                <select class="custom-select" id="cboParam"></select>
                                <input id="txtParam" type="text" class="form-control" placeholder="Valor parámetro">
                                <div class="input-group-append">
                                    <button class="btn btn-outline-secondary" type="button" title="Borrar" @click=${this.clearInputParam}>
                                        <i class="fas fa-eraser"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="btn-group" role="group">
                                <button id="btnAddParam" class="btn btn-warning" @click=${this.addParam}>
                                    <i class="fas fa-plus"></i> Agregar
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <table
                id="tableParams"
                data-toolbar="#toolbarTableParams"
                data-locale="es-ES"
                >
                <thead>
                </thead>
                <tbody></tbody>
            </table>`;
    }

    async connectedCallback() {
        super.connectedCallback();

        const $this = this;
        await CommonValueResource.loadSelect(this.$.cboParam, "PARAMETROS_EMPRESAS");
        this.$$.tableParams.bootstrapTable({
            data: [],
            height: window.innerHeight - 295,
            columns: [
                { title:"Parámetro", field:"CompanyParameterDisplay", align:"left", valign:"middle", width: 250 },
                { title:"Valor", field:"Value", align:"left", valign:"middle", width: 250 },
                // { title:"Recurso", field:"ResourceFullName", align:"left", valign:"middle", formatter: $this.tableFormatterRecurso },
                // { title:"Valor", field:"RequestDetDisplayValue", align:"left", valign:"middle" },
                // { title:"Tipo", field:"RequestDetTypeDisplay", align:"center", valign:"middle", cellStyle: $this.tableCellStyleType },
                // { title:"Vigencia", field:"RequestDetValidityDisplay", align:"left", valign:"middle" },
                // { title:"Estado", field:"RequestDetStatusName", align:"right", valign:"middle" },
                // { title:"", field:"RequestDetStatus", align:"center", valign:"middle", class:"", formatter: $this.tableFormatterStatus },
                { title:"", field:"", align:"center", valign:"middle", width: 90, formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ]
        });
    }

    addParam(e) {
        e.preventDefault();        
        if(this.$.cboParam.selectedIndex == -1) {
            iziToast.error({ title: 'No ha seleccionado el parámetro.' });
            return;
        }
        if(this.$.txtParam.value.trim() == '') {
            iziToast.error({ title: 'El parámetro no tiene valor.' });
            return;
        }

        const selectedParam = this.$.cboParam.selectedOptions[0].$data;
        if(this.$parent.$entity.CompanyParameters.some(x => x.CompanyParameterID == selectedParam.CommonValueID)) {
            iziToast.error({ title: 'Ya tiene este parámetro.' });
            return;
        }

        const param = new CompanyParameter();
        param.CompanyParameterID = selectedParam.CommonValueID;
        param.CompanyParameterDisplay = selectedParam.CommonValueDisplay;
        param.Value = this.$.txtParam.value.trim();
        this.$parent.$entity.CompanyParameters.push(param);
        this.loadTable();
        this.clearInputParam();
    }

    deleteParam(row) {
        let index = this.$parent.$entity.CompanyParameters.indexOf(row);
        this.$parent.$entity.CompanyParameters.splice(index, 1);
        this.loadTable();
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-danger btn-delete" title="Eliminar">
                        <i class="fas fa-times"></i> Eliminar
                    </button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let $this = this;
        return {
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }

    btnColumnDeleteClick(e, value, row, index) {
        this.deleteParam(row);
    }

    clearInputParam() {
        this.$.cboParam.selectedIndex = -1;
        this.$.txtParam.value = '';
    }

    onShow() {
        this.clearInputParam();
        this.loadTable();
    }

    loadTable() {
        this.$$.tableParams.bootstrapTable('load', this.$parent.$entity.CompanyParameters);
    }
}
customElements.define('company-edit-params', CompanyParamsComponent);