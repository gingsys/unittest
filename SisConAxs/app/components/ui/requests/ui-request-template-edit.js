import { UIEditComponent } from '../common/ui-edit.js';
import { RequestTemplate, RequestTemplateDetail, RequestTemplateResource } from '../../resources/resource-request-template.js';
import { CommonValueResource, CommonValueSet } from '../../resources/resource-commonvalue.js';
import { CategoryResource } from '../../resources/resource-category.js';
import { Resource, ResourceResource } from '../../resource-resource.js';
import { AccessType, AccessTypeResource } from '../../resources/resource-accesstype.js';
import { TreeSelectResource } from '../../ui-resource.js';
import { RequestTemplateEditView } from './ui-request-template-edit.view.js';
import '../controls/ui-control-date.js';
import '../controls/ui-control-select.js';

export class RequestTemplateEditComponent extends UIEditComponent {
    constructor() {
        super();
        this.$resourceResource = new ResourceResource();
        this.$accessTypeResource = new AccessTypeResource();
        this.$resources = [];
        this.$accessTypes = [];
    }

    get $template() {
        return new RequestTemplateEditView(this);
    }

    get Resource() {
        return RequestTemplateResource;
    }
    get Entity() {
        return RequestTemplate;
    }
    get entityID() {
        return 'ReqTemplateID';
    }

    get tableDetails() {
        return this.$one('[data-table-detail]');
    }

    async connectedCallback() {
        super.connectedCallback();
        
        this.setControls();
        // await Promise.all([
        //     this.$resourceResource.get({ ResourceActive: 1 }).then(r => this.$resources = r),
        //     this.$accessTypeResource.get({ WithDetails: true }).then(r => this.$accessTypes = r)
        // ]);
    }

    setControls() {
        const $this = this;
        CommonValueResource.fillControlSelectData(this.$.cboEmployeeType, CommonValueSet.SET_TIPO_EMPLEADO);
        CategoryResource.loadSelect(this.$.cboCategoryResource);
        this.treeSelectResource = ReactDOM.render(React.createElement(TreeSelectResource), this.$.searchResource);

        $(this.tableDetails).bootstrapTable({
            data: [],
            height: window.innerHeight - 405,
            filterControl: true,
            columns: [
                { title: "Categoría", field: "ReqTemplateDetCategoryName", align: "left", valign: "middle", width: 200 },
                { title: "Recurso", field: "ReqTemplateDetResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterResource },
                { title: "Valor", field: "ReqTemplateDetDisplayValue", align: "left", valign: "middle", width: 275, formatter: $this.tableFormatterValue.bind($this), events: $this.tableEventColumnValue },
                { title: "Vigencia", field: "ReqTemplateDetTemporal", align: "center", valign: "middle", width: 50, formatter: $this.tableFormatterValidity.bind($this) },
                { title: "", field: "", align: "center", valign: "middle", width: 90, formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            // rowStyle: null,
            onPostBody: (data) => {
                data.forEach((row, index) => {
                    const resource = this.$resources.find(a => a.ResourceID == row.ReqTemplateDetResourceID);
                    const accesstype = this.$accessTypes.find(a => a.AccessTypeID == row.ReqTemplateDetAccessTypeID);

                    // value -----------------------------------------------------------------
                    const controlValue = this.$one(`[data-row-value='${index}']`);
                    if(accesstype.AccessTypeType == AccessType.TYPE_SIMPLE) {
                        row.ReqTemplateDetStrValue = '';
                        row.ReqTemplateDetIntValue = null;
                    }
                    // text
                    else if ([AccessType.TYPE_TEXTO, AccessType.TYPE_ENTERPRISE_EMAIL].includes(accesstype.AccessTypeType)) {
                        controlValue.addEventListener('input', e => row.ReqTemplateDetStrValue = e.target.value.trim());
                        controlValue.value = (row.ReqTemplateDetStrValue || '');
                        row.ReqTemplateDetIntValue = null;
                    }
                    // select control
                    else if ([AccessType.TYPE_SELECT_SIMPLE, AccessType.TYPE_SELECT_MULTIPLE].includes(accesstype.AccessTypeType)) {
                        const data = accesstype.AccessTypeValues.map(v => {
                            return {
                                value: v.TypeValueID,
                                display: v.TypeValueDisplay,
                                data: v
                            };
                        });
                        controlValue.setData(data, '');

                        if (AccessType.TYPE_SELECT_SIMPLE == accesstype.AccessTypeType) {
                            controlValue.addEventListener('select', e => row.ReqTemplateDetIntValue = e.target.value);
                            controlValue.value = row.ReqTemplateDetIntValue;
                            row.ReqTemplateDetStrValue = '';
                        } else if (AccessType.TYPE_SELECT_MULTIPLE == accesstype.AccessTypeType) {
                            controlValue.addEventListener('select', e => row.ReqTemplateDetStrValue = e.target.value.toString());
                            controlValue.value = (row.ReqTemplateDetStrValue || '').split(',');
                            row.ReqTemplateDetIntValue = null;
                        }
                    }
                    // value -----------------------------------------------------------------

                    // temporal --------------------------------------------------------------
                    // if(resource.ResourceTemporal > 0) { // TEMPORAL_OPTIONAL, TEMPORAL_REQUIRED
                    //     const controlValidCheck = this.$one(`[data-row-valid='${index}'][data-valid-check]`);
                    //     const controlValidFrom  = this.$one(`[data-row-valid='${index}'][data-valid-from]`);
                    //     const controlValidUntil = this.$one(`[data-row-valid='${index}'][data-valid-until]`);
                        
                    //     // events
                    //     controlValidCheck.addEventListener('click', e => {
                    //         controlValidFrom.disabled = !e.target.checked;
                    //         controlValidUntil.disabled = !e.target.checked;
                    //         if(!e.target.checked) {
                    //             row.ReqTemplateDetValidityFrom  = controlValidFrom.value = null;
                    //             row.ReqTemplateDetValidityUntil = controlValidUntil.value = null;
                    //         }
                    //         row.ReqTemplateDetTemporal = e.target.checked? 1 : 0;
                    //     });
                    //     controlValidFrom.on('dp.change', (e) => {
                    //         // console.log(e.date._d);
                    //         if (e.date._d == null) {
                    //             controlValidUntil.fn.minDate(null);
                    //         } else {
                    //             controlValidUntil.fn.minDate(new Date(e.date._d));
                    //         }
                    //         row.ReqTemplateDetValidityFrom = e.date._d;
                    //     });
                    //     controlValidUntil.on('dp.change', (e) => {
                    //         // console.log(e.date._d);
                    //         if (e.date._d == null) {
                    //             controlValidFrom.fn.maxDate(null);
                    //         } else {
                    //             controlValidFrom.fn.maxDate(new Date(e.date._d));
                    //         }
                    //         row.ReqTemplateDetValidityUntil = e.date._d;
                    //     });
    
                    //     // set values
                    //     if (resource.ResourceTemporal == Resource.TEMPORAL_REQUIRED) {
                    //         controlValidCheck.disabled = true;
                    //         controlValidCheck.checked  = true;
                    //         controlValidFrom.disabled  = false;
                    //         controlValidUntil.disabled = false;
                    //         row.ReqTemplateDetTemporal  = 1;
                    //     } else if (resource.ResourceTemporal == Resource.TEMPORAL_OPTIONAL) {
                    //         controlValidCheck.disabled = false;
                    //         controlValidCheck.checked  = row.ReqTemplateDetTemporal > 0;
                    //         controlValidFrom.disabled  = !controlValidCheck.checked;
                    //         controlValidUntil.disabled = !controlValidCheck.checked;
                    //     }                        
                    //     controlValidFrom.value  = moment(row.ReqTemplateDetValidityFrom);
                    //     controlValidUntil.value = moment(row.ReqTemplateDetValidityUntil);
                    // } else {
                    //     row.ReqTemplateDetTemporal      = 0;
                    //     row.ReqTemplateDetValidityFrom  = null;
                    //     row.ReqTemplateDetValidityUntil = null;
                    // }

                    if(resource.ResourceTemporal > 0) { // TEMPORAL_OPTIONAL, TEMPORAL_REQUIRED
                        const controlValidCheck = this.$one(`[data-row-valid='${index}'][data-valid-check]`);
                        
                        // events
                        controlValidCheck.addEventListener('click', e => {
                            if(!e.target.checked) {
                                row.ReqTemplateDetValidityFrom  = null;
                                row.ReqTemplateDetValidityUntil = null;
                            }
                            row.ReqTemplateDetTemporal = e.target.checked? 1 : 0;
                        });                        
    
                        // set values
                        if(resource.ResourceTemporal == Resource.TEMPORAL_REQUIRED) {
                            controlValidCheck.disabled = true;
                            controlValidCheck.checked  = true;
                            row.ReqTemplateDetTemporal  = 1;
                        } else if (resource.ResourceTemporal == Resource.TEMPORAL_OPTIONAL) {
                            controlValidCheck.disabled = false;
                            controlValidCheck.checked  = row.ReqTemplateDetTemporal > 0;
                        }
                    } else {
                        row.ReqTemplateDetTemporal  = 0;
                    }
                    row.ReqTemplateDetValidityFrom  = null;
                    row.ReqTemplateDetValidityUntil = null;
                    // temporal --------------------------------------------------------------
                });
            },
            // onDblClickRow: (row, element, field) => {
            // }
        });
    }

    selectCategory(e) {
        const data = this.$resources.filter(r => r.ResourceCategory == this.$.cboCategoryResource.value);
        this.treeSelectResource.setData(data);
        this.render();
    }
    addDetail(e) {
        const selected = this.treeSelectResource.getSelectedData();
        if (selected.length == 0) {
            iziToast.info({ title: 'No ha seleccionado un recurso' });
            return;
        }
        
        const detail = this.$entity.ReqTemplateDetails;
        const resourcesID = detail.map(d => d.ReqTemplateDetResourceID);

        //mostramos una alerta si los items seleccionados ya se encuentran o estan pendientes
        const detailExists = selected.filter(r => resourcesID.includes(r.ResourceID))
                                     .map(r => {
                                        return `<li><b>${r.ResourceCategoryName}</b>/${r.ResourceFullName}</li>`;
                                     })
                                     .sort(r => r.ResourceCategoryName);
        if (detailExists.length > 0) {
            iziToast.info({ title: `Los siguientes recursos se encuentran asignados:<br><br><ul>${detailExists.join('')}</ul>` });
            return
        }

        const toAdd = RequestTemplateDetail.fromResourceList(selected.filter(x => !resourcesID.includes(x.ResourceID)));
        this.$entity.ReqTemplateDetails = detail.concat(toAdd)
                                               .sort((a, b) => `${a.ReqTemplateDetCategoryName} ${a.ReqTemplateDetResourceFullName}`.localeCompare(`${b.ReqTemplateDetCategoryName} ${b.ReqTemplateDetResourceFullName}`));
        $(this.tableDetails).bootstrapTable('load', this.$entity.ReqTemplateDetails);
        console.warn(toAdd);

        // clean
        this.$.cboCategoryResource.value = null;
        this.treeSelectResource.clearSelected();
        this.treeSelectResource.clearFilter();
        this.treeSelectResource.setData([]);
        this.render();
    }


    showContextMenuTextInput(e) {
        e.preventDefault();
        const top = e.pageY - 60;
        const left = e.pageX;
        const contextMenu = this.$.contextmenuTextInput;
        $(contextMenu)
            .css({
                display: "block",
                top: top,
                left: left,
                'z-index': 9999999
            })
            .addClass('show');
        contextMenu.$target = e.target;
        e.target.focus();
        return false;
    }
    addTextInputValue(e) {
        const contextMenu = this.$.contextmenuTextInput;
        const target = contextMenu.$target;
        const value = e.target.dataset.value;

        $(contextMenu).removeClass("show").hide();
        target.focus();        
        const start  = target.selectionStart;
        const end    = target.selectionEnd;
        const before = target.value.slice(0, start);
        const after  = target.value.slice(end);
        target.value = before + value + after;        
        target.focus();
        contextMenu.$target = null;
    }


    tableFormatterResource(value, row, index) {
        return `<strong>${row.ReqTemplateDetResourceName}</strong><br/><small>${row.ReqTemplateDetResourceFullName}</small>`;
    }
    tableFormatterValue(value, row, index) {
        const accesstype = this.$accessTypes.find(a => a.AccessTypeID == row.ReqTemplateDetAccessTypeID);
        if (accesstype.AccessTypeType == AccessType.TYPE_TEXTO) {
            return `<input type="text" data-row-value="${index}" class="form-control input-text" min="3" placeholder="Ingrese un texto" required />`;
        } else if (accesstype.AccessTypeType == AccessType.TYPE_SELECT_SIMPLE) {
            return `<control-select
                        class="row-select"
                        data-row-value="${index}"
                        data-none-selected-text="Nada seleccionado"
                        data-live-search="true"
                        data-size="8"
                        required>
                    </control-select>`;
        } else if (accesstype.AccessTypeType == AccessType.TYPE_SELECT_MULTIPLE) {
            return `<control-select
                        class="row-select"
                        data-row-value="${index}"
                        data-none-selected-text="Nada seleccionado"
                        data-live-search="true"
                        data-size="8"
                        multiple
                        required>
                    </control-select>`;
        } else if (accesstype.AccessTypeType == AccessType.TYPE_ENTERPRISE_EMAIL) {
            return `<input type="email" data-row-value="${index}" class="form-control input-email" placeholder="ejemplo: admin@gym.com" required />`;
        }
    }
    tableFormatterValidity(value, row, index) {
        const resource = this.$resources.find(r => r.ResourceID == row.ReqTemplateDetResourceID);        
        if (resource.ResourceTemporal > 0 || row.ReqTemplateDetTemporal > 0) {
            const isOptional = resource.ResourceTemporal == Resource.TEMPORAL_OPTIONAL;
            const isRequired = resource.ResourceTemporal == Resource.TEMPORAL_REQUIRED;
            const checkTemporal = row.ReqTemplateDetTemporal > 0;
            const title = "Se usará la fecha de ingreso como fecha inicial, para recursos Oracle la fecha final será de 2 años y para otros recursos la fecha final será el último día del siglo.";

            // return `
            // <div class="input-group mb-2 mr-sm-2">
            //     <div class="input-group-prepend">
            //         <div class="input-group-text">
            //             ${isOptional? `<input type="checkbox" data-row-valid="${index}" data-valid-check>` : ''}
            //             ${isRequired? `<input type="checkbox" data-row-valid="${index}" data-valid-check checked disabled>` : ''}
            //         </div>
            //     </div>
            //     <control-date class="valid-datepicker" data-row-valid="${index}" data-valid-from required ${isOptional && checkTemporal? 'disabled' : ''}></control-date>
            //     <control-date class="valid-datepicker" data-row-valid="${index}" data-valid-until required ${isOptional && checkTemporal? 'disabled' : ''}></control-date>
            // </div>
            // `;

            return `
            <div class="input-group mb-2 mx-4">
                <div class="input-group-prepend">
                    <div class="input-group-text">
                        ${isOptional? `<input type="checkbox" title="${title}" data-row-valid="${index}" data-valid-check>` : ''}
                        ${isRequired? `<input type="checkbox" title="${title}" data-row-valid="${index}" data-valid-check checked disabled>` : ''}
                    </div>
                </div>
            </div>
            `;
        }
    }
    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group">
                    <button type="button" class="btn btn-danger btn-delete" title="Eliminar"><i class="fas fa-times"></i> Eliminar</button>
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnValue() {
        const $this = this;
        return {
            // 'contextmenu .input-text': $this.showContextMenuTextInput.bind($this)
        }
    }

    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }
    btnColumnDeleteClick(e, value, row, index) {
        const data = this.$entity.ReqTemplateDetails;
        const dataIndex = data.findIndex(d => d.ReqTemplateDetResourceID == row.ReqTemplateDetResourceID);
        data.splice(dataIndex, 1);
        $(this.tableDetails).bootstrapTable('load', data);
    }
    // Table Events ------------------------------------------------------------------ //

    async edit(entity) {
        // load data
        this.$presenter.showLoading();
        await Promise.all([
            this.$resourceResource.get({ ResourceActive: 1 }).then(r => this.$resources = r),
            this.$accessTypeResource.get({ WithDetails: true }).then(r => this.$accessTypes = r)
        ]);
        this.$presenter.hideLoading();

        // controls
        super.edit(entity);
        this.$.cboCategoryResource.value = null;
        this.treeSelectResource.clearSelected();
        this.treeSelectResource.clearFilter();
        this.treeSelectResource.setData([]);
    }

    EntityToForm() {
        this.$.cboEmployeeType.value = this.$entity.ReqTemplateEmployeeType;
        this.$.chkActive.checked     = this.$entity.ReqTemplateActive;
        $(this.tableDetails).bootstrapTable('load', this.$entity.ReqTemplateDetails);
    }
    FormToEntity() {
        this.$entity.ReqTemplateEmployeeType = this.$.cboEmployeeType.value;
        this.$entity.ReqTemplateActive       = this.$.chkActive.checked? 1 : 0;
    }

    validate(e) {
        const details = this.$entity.ReqTemplateDetails;
        if(details.length == 0) {
            iziToast.error({ title: 'La solicitud no tiene detalles' });
            return false;
        }
        for(const index in details) {
            const item = details[index];
            const resource         = this.$resources.find(r => r.ResourceID == item.ReqTemplateDetResourceID);
            const resourceRequired = this.$resources.find(r => r.ResourceID == resource.ResourceRequired);
            const detailRequired   = details.find(d => d.ReqTemplateDetResourceID == resource.ResourceRequired);
            if(resource.ResourceRequired != null && detailRequired == null) {
                iziToast.error({ title: `El recurso '${resource.ResourceCategoryName} / ${resource.ResourceFullName}' requiere que se asigne también '${resourceRequired.ResourceCategoryName} / ${resourceRequired.ResourceFullName}'.` });
                return false;
            }
        };
        return true;
    }
}
customElements.define('request-template-edit', RequestTemplateEditComponent);