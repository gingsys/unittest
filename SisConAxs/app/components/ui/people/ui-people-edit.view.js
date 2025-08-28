import { ComponentView, html } from '../../../core/core.js';
import { SessionResource } from '../../resources/resource-session.js';
import { User } from '../../resources/resource-user.js';
import { People } from '../../resources/resource-people.js';

export class PeopleEditView extends ComponentView {

    render() {
        const $component = this.$component;
        // const roleAdministrator = LoginResource.auth.havePermission(User.ROLE_ADMINISTRADOR);
        return html`
        <style>
            .modal-opened {
                opacity: 0.9;
                filter: Alpha(Opacity=50);
            }
        </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit="${$component.save}">
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Destinatario</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="cboPeopleTypeClasificacion" class="col-sm-2 col-form-label">Clasificación</label>
                                <div class="col-sm-4">
                                <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboPeopleTypeClasificacion" class="custom-select" disabled></select>
                                        <div class="invalid-feedback">
                                            Por favor, ingrese una clasificación correcta.
                                        </div>
                                    </div>
                                </div>
                                <label for="lblCompanyName" class="col-sm-2 col-form-label">Empresa</label>
                                <div class="col-sm-4">
                                    <label id="lblCompanyName" class="col-form-label"></label>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtPeopleInternalID" class="col-sm-2 col-form-label">Número ID</label>
                                <div class="col-sm-4">
                                    <div class="input-group mb-3" id="prueba" style="margin: 0px !important;">
                                        <input type="text" name="numeroid" class="form-control" autocomplete="off" disabled id="txtPeopleInternalID" placeholder="Ingrese el número de documento" disabled />
                                        <div class="input-group-append" data-classification="${People.CLASIFICACION_COLABORADOR},${People.CLASIFICACION_COLABORADOR_EN_PROCESO},${People.CLASIFICACION_TERCERO}">
                                            <button class="btn btn-secondary" type="button" id="btnSearchAD" title="Buscar persona en Active Directory" @click="${$component.searchAD}">
                                                Active Directory <i class="fas fa-search"></i>
                                            </button>
                                        </div>
                                        <div id="errorid" class="invalid-feedback">
                                            Por favor, Digite un Número ID correcto.
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="cboPeopleDocType" class="col-sm-2 col-form-label">Tipo Documento</label>
                                <div class="col-sm-4">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <select id="cboPeopleDocType" class="custom-select" required @change=${$component.changeDocType}></select>
                                        <div class="invalid-feedback">
                                            Por favor, ingrese un Tipo de Documento correcto.
                                        </div>
                                    </div>
                                </div>
                                <label for="txtPeopleDocNum" class="col-sm-2 col-form-label">Número Documento</label>
                                <div class="col-sm-4">
                                    <input type="text" class="form-control" id="txtPeopleDocNum" minlength="5" maxlength="200" required @input=${$component.inputDocNum} @change=${$component.changeDocNum} />
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un Número de Documento correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <div class="col-sm-6" data-people-name="last">
                                    <div class="row">
                                        <label for="txtPeopleLastName" class="col-sm-4 col-form-label">Apellidos</label>
                                        <div class="col-sm-8">
                                            <input type="text" class="form-control" id="txtPeopleLastName" minlength="3" maxlength="255" required />
                                            <div class="invalid-feedback">
                                                Por favor, ingrese los Apellidos.
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6" data-people-name="first">
                                    <div class="row">
                                        <label id="lblPeopleFirstName" for="txtPeopleFirstName" class="col-sm-4 col-form-label">Nombres</label>
                                        <div class="col-sm-8">
                                            <input type="text" class="form-control" id="txtPeopleFirstName" minlength="3" maxlength="255" required />
                                            <div id="messagePeopleFirstName" class="invalid-feedback">
                                                Por favor, ingrese los Nombres.
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- COMENTADO POR AHORA !!! -->
                            <!-- <div class="form-group row">
                                <label for="txtPeopleBirthday" class="col-sm-2 col-form-label">Fecha Nacimiento</label>
                                <div class="col-sm-4">
                                    <div class="input-group date" id="dptPeopleBirthday">
                                        <input id="txtPeopleBirthday" class="form-control" required />
                                        <span class="input-group-addon">
                                            <span><i class="fa fa-calendar btn btn-outline-secondary" aria-hidden="true"></i></span>
                                        </span>
                                    </div>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese la fecha de nacimiento.
                                    </div>
                                </div>
                                <label for="cboPeopleGender" class="col-sm-2 col-form-label">Sexo</label>
                                <div class="col-sm-4">
                                    <select class="custom-select" id="cboPeopleGender"  required >
                                        <option value="F">Femenino</option>
                                        <option value="M">Masculino</option>
                                    </select>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese sexo.
                                    </div>
                                </div>
                            </div> -->
                            <div class="form-group row">
                                <div class="form-check offset-md-2" style="padding-left: 32px;">
                                    <input class="form-check-input" type="checkbox" id="chkPeopleStatus">
                                    <label class="form-check-label" for="chkPeopleStatus">
                                        Activo
                                    </label>
                                </div>
                            </div>

                            <br/>
                            <h5>Datos Adicionales</h5>
                            <div class="form-group row" data-classification="${People.CLASIFICACION_COLABORADOR},${People.CLASIFICACION_COLABORADOR_EN_PROCESO}">
                                <label for="cboPeopleEmployeeType" class="col-sm-2 col-form-label">Tipo Empleado</label>
                                <div class="col-sm-4">
                                    <div class="input-group">
                                        <control-select id="cboPeopleEmployeeType"
                                            data-none-selected-text="Seleccione tipo empleado"
                                            data-live-search="true"
                                            data-size="8"
                                            required>
                                        </control-select>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row" data-classification="${People.CLASIFICACION_COLABORADOR},${People.CLASIFICACION_COLABORADOR_EN_PROCESO},${People.CLASIFICACION_TERCERO}">
                                <label for="cboPeopleAttribute2" class="col-sm-2 col-form-label">Proyecto</label>
                                <div class="col-sm-4">
                                    <div class="input-group mb-3" style="margin: 0px !important;">
                                        <control-select id="cboPeopleAttribute2"
                                            data-none-selected-text="Seleccione proyecto"
                                            data-live-search="true"
                                            data-size="8"
                                            required>
                                        </control-select>
                                        <div class="input-group-append">
                                            <button class="btn btn-outline-secondary" type="button" id="btnProjectDelete" title="Quitar proyecto" @click="${$component.btnProjectDeleteClick}">
                                                <i class="fas fa-minus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                                <label for="txtPeopleAttribute3" class="col-sm-2 col-form-label">Cargo / Tipo</label>
                                <div class="col-sm-4">
                                    <input type="text" class="form-control" id="txtPeopleAttribute3" required />
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un Cargo o Tipo correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtPeopleEmail" class="col-sm-2 col-form-label">Email</label>
                                <div class="col-sm-4">
                                    <input type="email" class="form-control" id="txtPeopleEmail" required />
                                    <div class="invalid-feedback">
                                        No tiene el formato correcto.
                                    </div>
                                </div>
                            </div>

                            <div data-workflow>
                                <br/>
                                <h5>Workflow</h5>
                                <div class="form-group row">
                                    <label for="cboPeopleDepartment" class="col-sm-2 col-form-label">Área (Workflow)</label>
                                    <div class="col-sm-4">
                                        <div class="input-group">
                                            <control-select id="cboPeopleDepartment"
                                                data-none-selected-text="Seleccione área"
                                                data-live-search="true"
                                                data-size="8">
                                            </control-select>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" type="button" id="btnDepartmentDelete" title="Quitar área" @click="${$component.btnDepartmentDeleteClick}">
                                                    <i class="fas fa-minus"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                    <label for="cboPeoplePosition" class="col-sm-2 col-form-label">Cargo (Workflow)</label>
                                    <div class="col-sm-4">
                                        <div class="input-group">
                                            <control-select id="cboPeoplePosition"
                                                data-none-selected-text="Seleccione cargo"
                                                data-live-search="true"
                                                data-size="8">
                                            </control-select>
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" type="button" id="btnPositionDelete" title="Quitar cargo" @click="${$component.btnPositionDeleteClick}">
                                                    <i class="fas fa-minus"></i>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group row" data-classification="${People.CLASIFICACION_COLABORADOR},${People.CLASIFICACION_COLABORADOR_EN_PROCESO},${People.CLASIFICACION_TERCERO}">
                                <label for="txtUserID" class="col-sm-2 col-form-label">Usuario</label>
                                <div class="col-sm-4">
                                    <input type="email" class="form-control" id="txtUserID" disabled />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- ${$component.$entity.PeopleID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click="${$component.delete}"><i class="fas fa-eraser"></i> Borrar</button>` : ''} -->
                            <button type="submit" class="btn btn-success" id="btnSave" ><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <user-search-ad id="searchAD" @selectitem=${$component.setValuesFromAD} @close=${$component.closeModalSearchAD}></user-search-ad>`;
    }
}