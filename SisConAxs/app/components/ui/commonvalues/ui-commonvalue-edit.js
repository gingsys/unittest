import { CommonValueEditView } from './ui-commonvalue-edit.view.js';
import { CommonValueResource, CommonValueSet, CommonValue } from '../../resources/resource-commonvalue.js';
import { UIEditComponent } from '../common/ui-edit.js';

export class CommonValueEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
        this.$resource = new CommonValueResource();
        this.$entity = new CommonValueSet();
        this.$entityDetail = null;
    }

    get $template() {
        return new CommonValueEditView(this);
    }

    get Resource() {
        return CommonValueResource;
    }
    get Entity() {
        return CommonValueSet;
    }
    get entityID() {
        return 'CommonValueSetID';
    }

    connectedCallback() {
        super.connectedCallback();

        const $this = this;
        this.$$.tableDetail.bootstrapTable({
            columns: [
                { title:"Nombre", field:"CommonValueName", align:"left", valign:"middle", class:"col-nombre" },
                { title:"Etiqueta", field:"CommonValueDisplay", align:"left", valign:"middle", class:"col-etiq" },
                { title:"Descripción", field:"CommonValueDesc", align:"left", valign:"middle", class:"col-desc" },
                // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
                { title:"", field:"", align:"center", valign:"middle", class:"col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onDblClickRow: (row, element, field) => {
                this.editDetail(row);
            }
        });
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                    <button type="button" class="btn btn-danger btn-delete" title="Eliminar"><i class="fas fa-times"></i></button>
                </div>`;
    }


    btnAddDetailClick(e) {
        e.preventDefault();
        let entity = new CommonValue();
        this.editDetail(entity);
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        // let entity = new CommonValue();
        // entity.fromDTO(row);
        this.editDetail(row);
    }
    btnColumnDeleteClick(e, value, row, index) {
        this.$presenter.question(
            'Borrar',
            'Desea borrar este detalle?',
            (instance, toast) => {
                let index = this.$entity.CommonValues.indexOf(row);
                this.$entity.CommonValues.splice(index, 1);
                this.loadDetail();
            }
        );
    }


    loadDetail() {
        this.$$.tableDetail.bootstrapTable('load', this.$entity.CommonValues || []);
    }
    editDetail(entity) {
        this.$entityDetail = entity;
        this.$.txtDetailName.value = entity.CommonValueName;
        this.$.txtDetailDisplay.value = entity.CommonValueDisplay;
        this.$.txtDetailDescription.value = entity.CommonValueDesc;
        this.$$.formModalDetail.modal();

        this.$.txtDetailName.disabled = this.$entityDetail.CommonValueSetID > 0 && this.$entityDetail.CommonValueID > 0;
        if(!this.$.txtDetailName.disabled) {
            this.$.txtDetailName.select();
        } else {
            this.$.txtDetailDisplay.select();
        }
    }
    formDetailSubmit(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({title:'No ha ingresado los datos necesarios'});
            return;
        };

        this.$entityDetail.CommonValueName = this.$.txtDetailName.value;
        this.$entityDetail.CommonValueDisplay = this.$.txtDetailDisplay.value;
        this.$entityDetail.CommonValueDesc = this.$.txtDetailDescription.value;

        let index = this.$entity.CommonValues.indexOf(this.$entityDetail);
        if(index == -1) {
            this.$entity.CommonValues.push(this.$entityDetail);
        }
        this.loadDetail();
        this.$$.formModalDetail.modal('hide');
    }

    // Actions ------------------------------------------------------------------------- //
    add() {
        this.edit(new CommonValueSet());
    }
    edit(entity) {
        this.$entity = entity;
        this.EntityToForm();
        
        this.$$.formModal.modal({ backdrop: 'static' });
        this.$$.tableDetail.bootstrapTable('refreshOptions', {
            height: window.innerHeight - 340
        });
        this.$.txtName.select();
    }

    EntityToForm() {
        this.$.txtName.value        = this.$entity.CommonValueSetName;
        this.$.txtDescription.value = this.$entity.CommonValueSetDesc;
        this.loadDetail();
    }
    FormToEntity() {
        this.$entity.CommonValueSetName = this.$.txtName.value.trim();
        this.$entity.CommonValueSetDesc = this.$.txtDescription.value.trim();
    }

    validate(e) {
        return true;
    }
}
customElements.define('commonvalue-edit', CommonValueEditComponent);