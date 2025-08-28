import { AccessType, AccessTypeResource, AccessTypeValue } from "../../resources/resource-accesstype.js";
import { UIEditComponent } from "../common/ui-edit.js";
import { AccessTypeEditView } from "./ui-accesstype-edit.view.js";

class AccessTypeEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
        this.$entityDetail = null;
    }

    get $template() {
        return new AccessTypeEditView(this);
    }

    get Resource() {
        return AccessTypeResource;
    }
    get Entity() {
        return AccessType;
    }
    get entityID() {
        return 'AccessTypeID';
    }

    connectedCallback() {
        super.connectedCallback();

        const $this = this;
        this.$$.tableDetail.bootstrapTable({
            columns: [
                { title:"Nombre", field:"TypeValueName", align:"left", valign:"middle", class:"col-nombre" },
                { title:"Etiqueta", field:"TypeValueDisplay", align:"left", valign:"middle", class:"col-etiq" },
                { title:"Valor", field:"TypeValue", align:"left", valign:"middle", class:"col-desc" },
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

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        // let entity = new AccessType();
        // entity.fromDTO(row);
        this.editDetail(row);
    }
    btnColumnDeleteClick(e, value, row, index) {
        this.$presenter.question(
            'Borrar',
            'Desea borrar este detalle?',
            (instance, toast) => {
                this.$entity.AccessTypeValues.splice(index, 1);
                this.loadDetail();
            }
        );
    }

    addDetail(e) {
        e.preventDefault();
        const entity = new AccessTypeValue();
        this.editDetail(entity);
    }
    showDetail() {
        this.$$.tableDetailWrap.hide();

        const accessTypeType = parseInt(this.$.cboAccessTypeType.value);
        if([AccessType.TYPE_SELECT_SIMPLE, AccessType.TYPE_SELECT_MULTIPLE].includes(accessTypeType)) {
            this.$$.tableDetailWrap.show();
            setTimeout(() => {
                this.$$.tableDetail.bootstrapTable('refreshOptions', {
                    height: window.innerHeight - this.$.tableDetail.getBoundingClientRect().top - 45
                });
            }, 50);
            
        }
    }
    loadDetail() {
        this.$$.tableDetail.bootstrapTable('load', this.$entity.AccessTypeValues || []);
    }
    editDetail(entity) {
        this.$entityDetail            = entity;
        this.$.txtDetailName.value    = entity.TypeValueName;
        this.$.txtDetailDisplay.value = entity.TypeValueDisplay;
        this.$.txtDetailValue.value   = entity.TypeValue;
        this.$.cboDetailValueAdditional.value = entity.TypeValueAdditional;
        this.$$.formModalDetail.modal();
        this.$.txtDetailName.select();
    }
    formDetailSubmit(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({title:'No ha ingresado los datos necesarios'});
            return;
        };

        this.$entityDetail.TypeValueName       = this.$.txtDetailName.value.trim();
        this.$entityDetail.TypeValueDisplay    = this.$.txtDetailDisplay.value.trim();
        this.$entityDetail.TypeValue           = this.$.txtDetailValue.value.trim();
        this.$entityDetail.TypeValueAdditional = this.$.cboDetailValueAdditional.value;

        const index = this.$entity.AccessTypeValues.indexOf(this.$entityDetail);
        if(index == -1) {
            this.$entity.AccessTypeValues.push(this.$entityDetail);
        }
        this.loadDetail();
        this.$$.formModalDetail.modal('hide');
    }

    // Actions ------------------------------------------------------------------------- //

    
    EntityToForm() {
        this.$.txtName.value           = this.$entity.AccessTypeName;
        this.$.cboAccessTypeType.value = this.$entity.AccessTypeType;
        this.showDetail();
        this.loadDetail();
    }
    FormToEntity() {
        this.$entity.AccessTypeName = this.$.txtName.value.trim();
        this.$entity.AccessTypeType = this.$.cboAccessTypeType.value.trim();
    }

    validate(e) {
        if (!e.target.checkValidity()) {
            iziToast.error({title:'No ha ingresado los datos necesarios'});
            return;
        };

        const accessTypeType = parseInt(this.$.cboAccessTypeType.value.trim());
        if([AccessType.TYPE_SELECT_SIMPLE, AccessType.TYPE_SELECT_MULTIPLE].includes(accessTypeType)) {
            if(this.$entity.AccessTypeValues.length == 0) {
                iziToast.error({title:'No ha ingresado el detalle'});
                return;
            }
        } else {
            this.$entity.AccessTypeValues = [];
        }

        return true;
    }
}
customElements.define("accesstype-edit", AccessTypeEditComponent);