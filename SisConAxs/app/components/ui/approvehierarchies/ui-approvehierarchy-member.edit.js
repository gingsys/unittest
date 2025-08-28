import { Component, html } from "../../../core/core.js";
import { WorkflowApproveHierarchyResource, WorkflowHierarchyMember } from "../../resources/resource-approvehierarchy.js";
import { SessionResource } from "../../resources/resource-session.js";
import { CommonValueHelper } from "../../resources/helpers/commonvalue-helper.js";
import { CompanyHelper } from "../../resources/helpers/company-helper.js";
import { Constants } from "../../resources/constants.js";

class ApproveHierarchyMemberEditComponent extends Component {

    constructor() {
        super(false);
        this.$resource = new WorkflowApproveHierarchyResource();
    }

    get $template() {
        return html `
        <div data-modal class="modal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${this.setMember}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Miembro para Jerarquía</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <label for="cboCompany" class="col-form-label">Empresa:</label>
                                <div class="input-group mb-3" style="margin: 0px !important;">
                                    <control-select id="cboCompany" data-none-selected-text="Seleccione empresa" data-live-search="true" data-size="8" @select=${this.selectCompany}></control-select>
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="cboArea" class="col-form-label">Área:</label>
                                <div class="input-group" style="margin: 0px !important;">
                                    <control-select id="cboArea" data-none-selected-text="Seleccione área" data-live-search="true" data-size="8" required></control-select>
                                    <div class="invalid-feedback">
                                        Por favor, seleccionar un área.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="cboPosition" class="col-form-label">Cargo:</label>
                                <div class="input-group" style="margin: 0px !important;">
                                    <control-select id="cboPosition" data-none-selected-text="Seleccione cargo" data-live-search="true" data-size="8" required></control-select>
                                    <div class="invalid-feedback">
                                        Por favor, seleccionar un cargo.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="txtDescription" class="col-form-label">Descripción:</label>
                                <input type="text" class="form-control" id="txtDescription">
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <button type="submit" class="btn btn-success"><i class="fas fa-check"></i> Aceptar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        CompanyHelper.fillSelect(this.$.cboCompany);
        CommonValueHelper.fillSelectBySetName(this.$.cboArea, Constants.CV_SET_NOMBRE_AREAS);
        CommonValueHelper.fillSelectBySetName(this.$.cboPosition, Constants.CV_SET_NOMBRE_CARGOS);
    }

    add() {
        this.edit(new WorkflowHierarchyMember());
    }
    async edit(entity) {
        this.$entity = entity;

        this.__editing = false;
        this.$.cboCompany.value     = entity.WfHierarchyMemberCompany == null ? SessionResource.session.CompanyID : entity.WfHierarchyMemberCompany;
        this.$presenter.showLoading();
        await Promise.all([
            CommonValueHelper.fillSelectBySetNameCompany(this.$.cboArea, Constants.CV_SET_NOMBRE_AREAS, this.$.cboCompany.value),
            CommonValueHelper.fillSelectBySetNameCompany(this.$.cboPosition, Constants.CV_SET_NOMBRE_CARGOS, this.$.cboCompany.value)
        ]);
        this.$presenter.hideLoading();
        this.$.cboArea.value        = entity.WfHierarchyMemberDepartment;
        this.$.cboPosition.value    = entity.WfHierarchyMemberPosition;
        this.$.txtDescription.value = entity.WfHierarchyMemberDescription;
        this.__editing = true;

        $(this.querySelector("[data-modal]")).modal({ backdrop: "static" });
    }

    async selectCompany() {
        const companyID = this.$.cboCompany.value;
        if (this.__editing) {
            this.$presenter.showLoading();
            await Promise.all([
                CommonValueHelper.fillSelectBySetNameCompany(this.$.cboArea, Constants.CV_SET_NOMBRE_AREAS, companyID),
                CommonValueHelper.fillSelectBySetNameCompany(this.$.cboPosition, Constants.CV_SET_NOMBRE_CARGOS, companyID)
            ]);
            this.$presenter.hideLoading();
        }
    }

    setMember(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: "No ha ingresado los datos necesarios." });
            return;
        };

        if(this.$parent.$entity.WorkflowHierarchyMembers.some(m =>
                m != this.$entity &&
                m.WfHierarchyMemberDepartment == this.$.cboArea.value &&
                m.WfHierarchyMemberPosition == this.$.cboPosition.value
        ))
        {
            iziToast.error({ title: "Ya existe un registro con la misma área y cargo." });
            return;
        }

        this.$entity.WfHierarchyMemberCompany        = this.$.cboCompany.value;
        this.$entity.WfHierarchyMemberCompanyDisplay = this.$.cboCompany.display;
        this.$entity.WfHierarchyMemberDepartment     = this.$.cboArea.value;
        this.$entity.WfHierarchyMemberDepartmentName = this.$.cboArea.display;
        this.$entity.WfHierarchyMemberPosition       = this.$.cboPosition.value;
        this.$entity.WfHierarchyMemberPositionName   = this.$.cboPosition.display;
        this.$entity.WfHierarchyMemberDescription    = this.$.txtDescription.value.trim();

        this.$presenter.showLoading();
        this.$resource.getPeopleMemberWorkflow(this.$entity)
        .then(
            r => {
                this.$entity.WfHierarchyMemberPerson = r;                
                this.dispatchEvent(new CustomEvent('setmember', {
                    detail: this.$entity
                }));
                $(this.querySelector("[data-modal]")).modal('hide');
                this.$presenter.hideLoading();
            },
            err => {
                console.error('error!', err);
                this.$presenter.hideLoading();
            }
        );
    }
}
customElements.define("approvehierarchy-member", ApproveHierarchyMemberEditComponent);