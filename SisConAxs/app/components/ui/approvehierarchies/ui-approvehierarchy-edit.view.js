import { ComponentView, html } from "../../../core/core.js";

export class ApproveHierarchyEditView extends ComponentView {
    render() {
        const $component = this.$component;
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Jerarquía de Aprobación</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col">
                                    <div class="form-group">
                                        <label for="txtwfName" class="col-form-label">Nombre:</label>
                                        <input id="txtwfName" name="wfName" type="text" class="form-control" minlength="3" maxlength="200" autofocus tabindex="1" required />
                                        <div class="invalid-feedback">
                                            Por favor, ingresar un Nombre.
                                        </div>
                                    </div>
                                </div>
                                <div class="col">
                                    <div class="form-group">
                                            <label for="cboDepartment" class="col-form-label">Área por defecto</label>
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <control-select id="cboDepartment" data-none-selected-text="Seleccione área" data-live-search="true" data-size="8"></control-select>
                                                <div class="input-group-append">
                                                    <button id="btnRemoveArea" type="button" class="btn btn-outline-secondary" title="Quitar Área" autofocus tabindex="1003" @click=${$component.removeDepartment}>
                                                        <i class="fas fa-minus"></i>
                                                    </button>
                                                </div>
                                            </div>
                                    </div>
                                </div>
                            </div><br>
                            <h4>Miembros</h4>
                            <div data-toolbar-member>
                                <div class="btn-group">
                                    <button id="btnAddMember" class="btn btn-warning" @click=${$component.addMember}>
                                        <i class="fas fa-plus"></i> Agregar
                                    </button>
                                </div>
                            </div>
                            <table
                                data-table-member
                                data-locale="es-ES"
                                data-reorderable-rows="true"
                                >
                                <thead>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            ${$component.$entity.WfApproveHierarchyID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <approvehierarchy-member id="editMember" @setmember=${$component.setMember}></approvehierarchy-member>`;
    }
}