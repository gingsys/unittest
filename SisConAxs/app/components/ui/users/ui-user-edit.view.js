import { ComponentView, html } from '../../../core/core.js';
import { SessionResource } from '../../resources/resource-session.js';

export class UserEditView extends ComponentView {
    render() {
        const $component = this.$component;

        return html`
        <style>
            .modal-opened{
            opacity: 0.9;
            filter: Alpha(Opacity=50);         
            }
            .txt-disabled{
                background-color: #e9ecef;
                opacity: 1;
            }
            .txt-disabled:focus{
                background-color: #e9ecef;
                opacity: 1;
                outline: none;
            }
        </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Usuario</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="txtUserInternalID" class="col-sm-2 col-form-label">Usuario AD</label>
                                <div class="col-sm-10">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <input type="text" class="form-control txt-disabled" id="txtUserInternalID" maxlength="0" required>
                                        <div class="input-group-append">
                                            <button class="btn btn-secondary" type="button" id="btnSearchAD" @click=${$component.searchAD} title="Buscar usuario en Active Directory">
                                                Active Directory <i class="fas fa-search"></i>
                                            </button>
                                        </div>
                                        <div class="invalid-feedback">
                                            Por favor, ingrese un Usuario AD.
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtUserLastName" class="col-sm-2 col-form-label">Apellidos</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control txt-disabled" id="txtUserLastName"  maxlength="0" onkeypress="return false" />
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtUserFirstName" class="col-sm-2 col-form-label">Nombres</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control txt-disabled" id="txtUserFirstName" maxlength="0" required/>
                                    <div class="invalid-feedback">
                                            Por favor, ingrese Nombres.
                                        </div>
                                </div>
                            </div>

                            <div class="form-group row">
                                <div class="form-check offset-md-2" style="padding-left: 32px;">
                                    <input class="form-check-input" type="checkbox" id="chkUserStatus" checked
                                           .disabled="${SessionResource.session.UserRoleSysAdmin <= 0}"
                                           @change=${$component.checkStatus} />
                                    <label class="form-check-label" for="chkUserStatus">
                                        Activo
                                    </label>
                                </div>
                            </div>
                            <fieldset>
                                <legend>Roles</legend>
                                <div class="row">
                                    <div class="col-sm-3">
                                        <select id="lstCompany" class="custom-select" size="5" @change=${$component.changeListCompany}></select>
                                    </div>
                                    <div class="col-sm-9">
                                        <div class="form-group row">
                                            <div class="form-check" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole1" data-attr="UserRole1" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus==0}">
                                                <label class="form-check-label" for="chkUserRole1">
                                                    Solicitante
                                                </label>
                                            </div>
                                        </div>
                                        <div class="form-group row">
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole2" data-attr="UserRole2" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole2">
                                                    Aprobador
                                                </label>
                                            </div>
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole3" data-attr="UserRole3" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole3">
                                                    Administrador
                                                </label>
                                            </div>
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole4" data-attr="UserRole4" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole4">
                                                    Crear Personas
                                                </label>
                                            </div>
                                        </div>
                                        <div class="form-group row">
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole5" data-attr="UserRole5" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole5">
                                                    Dar de baja
                                                </label>
                                            </div>
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole6" data-attr="UserRole6" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole6">
                                                    Ver solicitudes proyecto
                                                </label>
                                            </div>
                                            <div class="form-check col-4" style="padding-left: 32px;">
                                                <input class="form-check-input" type="checkbox" id="chkUserRole7" data-attr="UserRole7" @change=${$component.checkRole} .disabled="${$component.$entity.UserStatus == 0}">
                                                <label class="form-check-label" for="chkUserRole7">
                                                    Reportes
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- <button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button> -->
                            <button type="submit" class="btn btn-success" id="btnSave" ?disabled=${!$component.__edited}><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <user-search-ad id="searchAD" @selectitem=${$component.setValuesFromAD} @close=${$component.closeModalSearchAD}></user-search-ad>`;
    }
}