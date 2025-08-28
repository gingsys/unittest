import { WorkflowApproveHierarchy, WorkflowApproveHierarchyResource } from "../../resources/resource-approvehierarchy.js";
import { CommonValueHelper } from "../../resources/helpers/commonvalue-helper.js";
import { Constants } from "../../resources/constants.js";
import { UIEditComponent } from "../common/ui-edit.js";
import { ApproveHierarchyEditView } from "./ui-approvehierarchy-edit.view.js";
import "./ui-approvehierarchy-member.edit.js";

export class ApproveHierarchyEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
    }

    get $template() {
        return new ApproveHierarchyEditView(this);
    }

    get Resource() {
        return WorkflowApproveHierarchyResource;
    }
    get Entity() {
        return WorkflowApproveHierarchy;
    }
    get entityID() {
        return 'WfApproveHierarchyID';
    }

    get tableMembers() {
        return this.querySelector("[data-table-member]");
    }

    connectedCallback() {
        super.connectedCallback();

        this.buildTableMembers();
        CommonValueHelper.fillSelectBySetName(this.$.cboDepartment, Constants.CV_SET_NOMBRE_AREAS);
    }

    removeDepartment() {
        this.$.cboDepartment.value = null;
    }


    addMember(e) {
        e.preventDefault();
        this.$.editMember.add();
    }
    async editMember(entity) {
        this.$presenter.showLoading();
        await this.$.editMember.edit(entity);
        this.$presenter.hideLoading();
    }
    setMember(e) {
        const member = e.detail;
        const index = this.$entity.WorkflowHierarchyMembers.indexOf(member);
        if (index == -1) {
            member.WfHierarchyMemberOrder = this.$entity.WorkflowHierarchyMembers.length + 1;
            this.$entity.WorkflowHierarchyMembers.push(member);
        }
        this.loadMembers();
    }
    loadMembers() {
        $(this.tableMembers).bootstrapTable('load', this.$entity.WorkflowHierarchyMembers || []);
    }

    buildTableMembers() {
        const $this = this;
        $(this.tableMembers).bootstrapTable({
            toolbar: $this.querySelector("[data-toolbar-member]"),
            columns: [
                { title: "Empresa", field: "WfHierarchyMemberCompanyDisplay", align: "left", valign: "middle", class: "col-nombre" },
                { title: "Área", field: "WfHierarchyMemberDepartmentName", align: "left", valign: "middle", class: "col-nombre" },
                { title: "Cargo", field: "WfHierarchyMemberPositionName", align: "left", valign: "middle", class: "col-desc" },
                { title: "Descripción", field: "WfHierarchyMemberDescription", align: "left", valign: "middle", class: "col-desc" },
                { title: "Persona", field: "WfHierarchyMemberPerson", align: "left", valign: "middle", class: "col-desc" },
                { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
            ],
            onDblClickRow: (row, element, field) => {
                this.editMember(row);
            },
            onRefresh: () => {},
            onReorderRow: (reorderData) => {
                const membersOrder = reorderData.map((item, idx) => {
                    item.WfHierarchyMemberOrder = idx + 1;
                    return item;
                });
                this.$entity.WorkflowHierarchyMembers = membersOrder;
            }
        });
    }
    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
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
    async btnColumnEditClick(e, value, row, index) {
        const members = this.$entity.WorkflowHierarchyMembers;
        this.editMember(members[index]);
    }
    btnColumnDeleteClick(e, value, row, index) {
        this.$presenter.question(
            'Borrar',
            'Desea borrar este detalle?',
            (instance, toast) => {
                const members = this.$entity.WorkflowHierarchyMembers;
                members.splice(index, 1);
                members.forEach((item, idx) => {
                    item.WfHierarchyMemberOrder = idx + 1;
                });
                this.loadMembers();
            }
        );
    }


    EntityToForm() {
        // await CommonValueHelper.fillSelectBySetName(this.$.cboDepartment, Constants.CV_SET_NOMBRE_AREAS);

        this.$.txtwfName.value     = this.$entity.WfApproveHierarchyName;
        this.$.cboDepartment.value = this.$entity.WfApproveHierarchyDepartment;
        $(this.tableMembers).bootstrapTable('load', this.$entity.WorkflowHierarchyMembers);
        $(this.tableMembers).bootstrapTable('refreshOptions', {
            height: window.innerHeight - this.tableMembers.getBoundingClientRect().top - 400
        });
    }
    FormToEntity() {
        this.$entity.WfApproveHierarchyName       = this.$.txtwfName.value.trim();
        this.$entity.WfApproveHierarchyDepartment = this.$.cboDepartment.value;
    }

    validate(e) {
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        if (this.$entity.WorkflowHierarchyMembers.length <= 0) {
            iziToast.warning({ title: 'Por favor, ingresar un miembro como mínimo.' });
            return;
        }

        return true;
    }
}
customElements.define('approvehierarchy-edit', ApproveHierarchyEditComponent);