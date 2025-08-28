import { Component, html } from '../core.js';
import { SessionResource } from "./resources/resource-session.js";
import { User } from "./resources/resource-user.js";
import { Request } from "./resource-request.js";
import { People } from './resources/resource-people.js';
import { RequestDetailTableView } from './ui-request-edit.js';

export class RequestEditView {
    static render(scope) {
        return (function() {
            let roleAdministrator = SessionResource.session.havePermission(User.ROLE_ADMINISTRADOR);
            let roleCrearPersonas = SessionResource.session.havePermission(User.ROLE_CREA_PERSONAS);
            let roleSysAdmin = SessionResource.session.havePermission(User.ROLE_SYSADMIN);
            let userCompany = SessionResource.session.CompanyID;
            let isUserCreator = SessionResource.session.sessionUser == this.$entity.RequestBy;
            let showAnnul = this.$entity.RequestID > 0 && this.$entity.RequestCompletedDate == null && (roleAdministrator || isUserCreator);
            let showlows = SessionResource.session.UserRole5 == 1;
    
            return html`
            <style>
                #searchResource {
                    height: 32px;
                }
    
                .request-title {
                    width: 100%
                }
                .request-date {
                    text-align: right;
                }
                .request-status {
                    width: 50%;
                }
                .request-status .input-group-text,
                .request-status .form-control {
                    background: none;
                    border: 0;
                }
    
                .request-alta {
                    background-color: palegreen !important;
                }
                .request-modificacion {
                    background-color: yellow !important;
                }
                .request-baja {
                    background-color: #eaa0a0 !important;
                }
    
                .form-group {
                    margin-bottom: .5rem;
                }
    
                .current-resource {
                    background-color: #555 !important;
                    color: #FFF;
                }
                .current-resource-pending {
                    background-color: #007bff !important;
                    color: #FFF;
                }
                .current-resource-modify {
                    background-color: orange !important;
                    color: #FFF;
                }
                .current-resource-delete {
                    background-color: #F55 !important;
                    color: #FFF;
                }
                .current-resource-add {
                    background-color: #04B404 !important;
                    color: #FFF;
                }
    
                .check-approve input[type="radio"] {
                    opacity: 0;
                    display: none;
                }
    
                .table-responsive {
                    width: 100%;
                    overflow-y: visible;
                    overflow-x: scroll; 
                    -ms-overflow-style: -ms-autohiding-scrollbar;
                    -webkit-overflow-scrolling: touch;
                }
    
                .truncate {
                    /* width: 380px; */
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    vertical-align: -webkit-baseline-middle;
                } 
                .btn-approve {
                    padding-right: 15px;
                    padding-left: 15px;
                }
                /* .modal-applicant{
                    opacity: 0;
                } */
            </style>
            <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-xl" role="document">
                    <div class="modal-content">
                        <form class="was-validated form-horizontal" novalidate id="form" @submit=${this.mode == RequestDetailTableView.MODE_APPROVE ? this.approve : this.callConfirm}>
                            <div class="modal-header">
                                <h4 class="modal-title request-title" id="exampleModalLabel">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            Solicitud ${this.$entity.RequestNumber == 0 ? '' : this.formatNumber(this.$entity.RequestNumber, 8)}
                                        </div>
                                        <div class="col-sm-6 request-date">
                                            <small>Fecha: ${this.$entity.RequestDateDisplay}</small>
                                        </div>
                                    </div>
                                </h4>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="row">
                                    <div class="col">
                                        <div class="form-group row">
                                            <label for="cboType" class="col-sm-2 col-form-label">Tipo Solicitud</label>
                                            <div class="col-sm-4">
                                                <select id="cboType" class="custom-select" required @change=${this.cboTypeChange} ?disabled=${this.$entity.RequestID > 0}></select>
                                            </div>
                                            <label for="cboPriority" class="col-sm-2 col-form-label">Prioridad</label>
                                            <div class="col-sm-4">
                                                <select id="cboPriority" class="custom-select" required ?disabled=${this.$entity.RequestID > 0}></select>
                                            </div>
                                        </div>
                                        <div class="form-group row">
                                            <label for="txtByName" class="col-sm-2 col-form-label"><strong>Solicitado por</strong></label>
                                            <div class="col-sm-4">
                                                <input type="text" class="form-control" id="txtByName" disabled>
                                            </div>
                                            <label for="txtToName" class="col-sm-2 col-form-label"><strong>Solicitado para</strong></label>
                                            <div class="col-sm-4">
                                                <div class="input-group mb-3" style="margin: 0px !important;">
                                                    <input type="text" class="form-control" id="txtToName" disabled>
                                                    <div class="input-group-append">
                                                        <button class="btn btn-outline-secondary" type="button" id="btnPeopleSelf" title="Mi usuario" @click=${this.btnPeopleSelfClick} ?disabled=${this.$entity.RequestID > 0}>
                                                            <i class="fas fa-user-tag"></i>
                                                        </button>
                                                        <button class="btn btn-secondary" type="button" id="btnPeopleSearch" title="Buscar destinatario" @click=${this.btnPeopleSearchClick} ?disabled=${this.$entity.RequestID > 0}>
                                                            <i class="fas fa-search"></i>
                                                        </button>
                                                        <!-- <button class="btn btn-secondary" type="button" id="btnPeopleAdd" title="Agregar destinatario" @click=${this.btnPeopleAddClick} ?disabled=${this.$entity.RequestID > 0} ?data-hidden=${!roleCrearPersonas}>
                                                            <i class="fas fa-user-plus"></i>
                                                        </button> -->
                                                        <div class="btn-group" title="Agregar destinatario">
                                                            <button type="button" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-expanded="false" ?disabled=${this.$entity.RequestID > 0} ?data-hidden=${!roleCrearPersonas}>
                                                                <i class="fas fa-user-plus"></i>
                                                            </button>
                                                            <div class="dropdown-menu">
                                                                ${roleSysAdmin || userCompany == 10 || userCompany == 7 ? html`<a class="dropdown-item" href="javascript:void(0)" @click=${e => this.$.peopleAdd.add(People.CLASIFICACION_COLABORADOR_EN_PROCESO)}>Colaborador (Manual)</a>` : ''}
                                                                <a class="dropdown-item" href="javascript:void(0)" @click=${e => this.$.peopleAdd.add(People.CLASIFICACION_TERCERO)}>Tercero (Trabajador Externo)</a>
                                                                <a class="dropdown-item" href="javascript:void(0)" @click=${e => this.$.peopleAdd.add(People.CLASIFICACION_CLIENTE)}>Cliente</a>
                                                                <a class="dropdown-item" href="javascript:void(0)" @click=${e => this.$.peopleAdd.add(People.CLASIFICACION_PROVEEDOR)}>Proveedor</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <section ?data-hidden=${this.detailExpanded}>
                                            <div class="form-group row">
                                                <label for="txtByProject" class="col-sm-2 col-form-label">Proyecto</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtByProject" disabled>
                                                </div>
                                                <label for="txtToProject" class="col-sm-2 col-form-label">Doc./Empresa/Proyecto/Cargo</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtToProject" disabled>
                                                </div>
                                            </div>
                                            <div class="form-group row" ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT}>
                                                <label for="txtByPosition" class="col-sm-2 col-form-label">Cargo</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtByPosition" disabled>
                                                </div>
                                                <label for="txtToPosition" class="col-sm-2 col-form-label">Cargo</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtToPosition" disabled>
                                                </div>
                                            </div>
                                            <div class="form-group row" ?data-hidden=${this.mode == RequestDetailTableView.MODE_EDIT}>
                                                <label for="txtTicket" class="col-sm-2 col-form-label">Ticket Atención</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtTicket" maxlength="150" .disabled=${this.$entity.AttentionTicket != undefined || this.mode == RequestDetailTableView.MODE_VIEW}> 
                                                </div>
                                                <label for="txtOracleUser" class="col-sm-2 col-form-label">Usuario Oracle</label>
                                                <div class="col-sm-4">
                                                    <input type="text" class="form-control" id="txtOracleUser" maxlength="60" .disabled=${this.$entity.AttentionTicket != undefined || this.mode == RequestDetailTableView.MODE_VIEW}>
                                                </div>
                                            </div>
                                            <div class="form-group row">
                                                    <label for="txtAttached" class="col-sm-2 col-form-label">Adjunto/Anexo</label>
                                                    <div class="col-sm-4">
                                                        <div class="custom-file">
                                                            <input type="file" .disabled=${(this.mode == RequestDetailTableView.MODE_VIEW || this.conflictoDetectado)} class="custom-file-input" id="fileAttached" onchange="$(this).next().after().text($(this).val().split('\\\\').slice(-1)[0])" @change=${this.onChangeFile} lang="es">
                                                            <label class="custom-file-label truncate" for="fileAttached" data-browse="Elegir">Elija un archivo</label>
                                                        </div>
                                                    </div>
                                                    <!-- <label ?data-hidden=${(!(!this.$entity.FileAttachedString && this.$entity.RequestID > 0))} class="col-sm-4" style="vertical-align: sub;">-</label> -->
                                                    <label for="txtToEmail" class="col-sm-2 col-form-label" ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT}>Email</label>
                                                    <div class="col-sm-4" ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT}>
                                                        <input type="text" class="form-control" id="txtToEmail" disabled>
                                                    </div>
                                                    <label for="txtOracleMenu" class="col-sm-2 col-form-label" ?data-hidden=${this.mode == RequestDetailTableView.MODE_EDIT}>Menu Oracle</label>
                                                    <div class="col-sm-4" ?data-hidden=${this.mode == RequestDetailTableView.MODE_EDIT}>
                                                        <input type="text" class="form-control" id="txtOracleMenu" maxlength="150" .disabled=${this.$entity.AttentionTicket != undefined || this.mode == RequestDetailTableView.MODE_VIEW}>
                                                    </div>
                                            </div>
                                            <div class="form-group row">
                                            <ul class="list-group offset-2" style="list-style-type: none;">
                                            ${this.$entity.FileAttachedString ? (this.$entity.FileAttachedString).split(';').map(x => {
                                                            return html ` 
                                                            <li><a ?data-hidden=${(!this.$entity.FileAttachedString)} class="col-sm-4" target="_blank" @click=${this.downloadFile} data-filename=${x.split('/').slice(-1)[0]} style="color: #07C;cursor: pointer;">
                                                                    <i class="fas fa-file-download mr-1" ?data-hidden=${(!this.$entity.FileAttachedString)} style="vertical-align: sub;"></i> 
                                                                    ${x.split('/').slice(-1)[0]}
                                                            </a></li>`
                                                        }) : ""
                                            }
                                            </ul>
                                            </div>
                                        </section>
                                        <div class="form-group row" ?data-hidden=${(this.$entity.RequestID > 0 && this.$entity.RequestNote == '' && this.mode != RequestDetailTableView.MODE_APPROVE) || this.detailExpanded}>
                                            <label for="txtNote" class="col-sm-2 col-form-label">Observación</label>
                                            <div class="col-sm-10">
                                                <div class="input-group">
                                                    <textarea class="form-control" id="txtNote" rows="2" placeholder="Tener en consideración que no se atenderá algún tipo de requerimiento descrito en este campo." ?readonly=${(this.$entity.RequestID > 0 || this.conflictoDetectado)}></textarea>
                                                    ${this.mode == RequestDetailTableView.MODE_APPROVE?
                                                        html`<div class="input-group-append">
                                                                <button class="btn btn-outline-secondary" type="button" @click=${this.btnShowToggleNoteAdd} title="Agregar Observación">
                                                                    ${this.showNoteAdd? html`<i class="fas fa-angle-up"></i>` : html`<i class="fas fa-plus"></i>`}
                                                                </button>
                                                            </div>` : ''
                                                    }
                                                </div>
                                            </div>
                                            <div class="col-sm-10 offset-sm-2 mt-2" ?data-hidden=${!this.showNoteAdd}>
                                                <textarea class="form-control" id="txtNoteAdd" rows="2" placeholder="Tener en consideración que no se atenderá algún tipo de requerimiento descrito en este campo."></textarea>
                                            </div>
                                        </div>
                                        <br>
                                        <div class="table-responsive">
                                            <div class="col">
                                                <div class="form-group row">
                                                        <!-- <div ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT}> -->
                                                                <select id="cboCategoryResource" class="custom-select col-sm-2" required @change=${this.cboCategoryResourceChange} ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT} required></select>
                                                                <div id="searchResource" class="col-sm-6" style="padding-left: 4px;padding-right: 0px;" ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT}></div>
                                                                <div class="btn-group ml-4 mr-5" ?data-hidden=${this.mode != RequestDetailTableView.MODE_EDIT || this.$.cboType.value == Request.TYPE_BAJA}>
                                                                    <button id="btnAddDetail" type="button" class="btn btn-success" @click=${this.btnAddDetailClick}>
                                                                        <i class="fas fa-plus"></i> Pedir Alta
                                                                    </button>
                                                                    <button id="btnDeleteDetail" type="button" class="btn btn-danger" @click=${this.btnDeleteDetailClick} ?data-hidden=${!showlows}>
                                                                        <i class="fas fa-minus"></i> Pedir Baja
                                                                    </button>
                                                                </div>
                                                        <!-- </div> -->
                                                        <div ?data-hidden=${this.mode != RequestDetailTableView.MODE_APPROVE}>
                                                            <div class="btn-group">
                                                                <button id="btnApproveAll" type="button" class="btn btn-outline-success approve-button" @click=${this.btnApproveAllClick}>
                                                                    <i class="fas fa-check"></i> Validar Todo
                                                                </button>
                                                                <button id="btnRejectAll" type="button" class="btn btn-outline-danger approve-button" @click=${this.btnRejectAllClick}>
                                                                    <i class="fas fa-times"></i> Denegar Todo
                                                                </button>
                                                                <button class="btn btn-outline-primary" type="button" id="btnShowAccess" @click=${this.showDetailApplicant}>
                                                                    <i class="fas fa-eye"></i> Ver Accesos
                                                                </button>
                                                            </div>
                                                        </div>
                                                        <label style="color:white" ?data-hidden=${this.mode != RequestDetailTableView.MODE_VIEW}>Detalle de los Recursos</label>
                                                        <button id="btnExpandDetail" type="button" class="btn btn-outline-secondary" style="position: absolute; right: 0;" @click=${this.btnExpandDetailClick}>
                                                            ${this.detailExpanded? 
                                                                html`<i class="fas fa-compress-arrows-alt"></i> Contraer` :
                                                                html`<i class="fas fa-expand-arrows-alt"></i> Expandir`
                                                            }
                                                        </button>
                                                    </div>
                                                </div>
                                            <table
                                                id="tableDetail"
                                                data-locale="es-ES"
                                                >
                                                <thead>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <div class="input-group request-status mr-auto">
                                    <div class="input-group-prepend">
                                        <div class="input-group-text">${this.iconStatus(this.$entity)}</div>
                                    </div>
                                    <input type="text" class="form-control" id="txtStatus" disabled .value=${'Estado: ' + (this.$entity.RequestID == 0? 'Nueva Solicitud' : this.$entity.RequestStatusName)}>
                                </div>
                                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                                <button type="button" class="btn btn-danger" id="btnDelete" @click=${this.Annul} ?data-hidden=${!showAnnul}>
                                    <i class="fas fa-ban"></i> Anular Pendientes
                                </button>
                                <button type="submit" class="btn btn-warning" id="btnSave" ?data-hidden=${!(this.$entity.RequestID == 0 || this.mode == RequestDetailTableView.MODE_APPROVE)}>
                                    <i class="fas fa-save"></i> Guardar
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
            <people-edit id="peopleAdd" @save=${this.onPeopleSave}></people-edit>
            <people-search id="peopleSearch" @selectitem=${this.onPeopleSelect}></people-search>
            <div data-component="requestDetailHistory"></div>
            <div data-component="requestConfirm"></div>
            <div data-component="alert"></div>
            <div data-component="detailApplicant"></div>`
        }).bind(scope)();
    }    
}