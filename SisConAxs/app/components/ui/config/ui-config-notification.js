import { Component, html } from '../../../core/core.js';
import { NotificationResource, Notification } from '../../resources/resource-notification.js';

export class NotificationComponent extends Component {
    constructor() {
        super(false);
        this.$resource = new NotificationResource();
        this.$entity = new Notification();
    }

    get $template() {
        return html `
        <style>
        </style>
        <h3 align="center">Configuración de Notificaciones</h3><br>
        <div class="row justify-content-center align-items-center">
        <form class="was-validated form-horizontal col-sm-5" novalidate id="form" @submit=${this.save}>
            <div class="form-group row">
                <label for="txtNotifConfHost" class="col-sm-4 col-form-label">Servidor de Correo</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="txtNotifConfHost" .value=${this.$entity.NotifConfHost} minlength="3" maxlength="400" pattern="^(http\:\/\/|https\:\/\/)?([a-z0-9][a-z0-9\-]*\.)+[a-z0-9][a-z0-9\-]*$" required>
                    <div class="invalid-feedback">
                        El campo debe tener un formato similar: <strong><em>mail.dominio.com</em></strong>
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <label for="txtNotifConfPort" class="col-sm-4 col-form-label">Puerto</label>
                <div class="col-sm-8">
                    <input type="number" min="1" class="form-control" id="txtNotifConfPort" .value=${this.$entity.NotifConfPort} required>
                    <div class="invalid-feedback">
                        Por favor, ingrese Puerto
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <div class="form-check offset-md-4" style="padding-left: 32px;">
                    <input class="form-check-input" type="checkbox" id="chkNotifConfSSL" ?checked=${this.$entity.NotifConfSSL == '1'}>
                    <label class="form-check-label" for="chkNotifConfSSL">
                        SSL
                    </label>
                </div>
            </div>
            <div class="form-group row">
                <label for="txtNotifConfUser" class="col-sm-4 col-form-label">Usuario de envío</label>
                <div class="col-sm-8">
                    <input type="email" class="form-control" id="txtNotifConfUser" .value=${this.$entity.NotifConfUser} minlength="2" maxlength="200" required>
                    <div class="invalid-feedback">
                        El campo debe tener un formato similar: <strong><em>correo@domain.com</em></strong>.
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <label for="txtNotifConfLock" class="col-sm-4 col-form-label">Contraseña de envío</label>
                <div class="col-sm-8">
                    <input type="password" class="form-control" id="txtNotifConfLock" .value=${this.$entity.NotifConfLock} pattern=".{3,}" pattern=".{,20}" maxlength="20" required>
                    <div class="invalid-feedback">
                        Por favor, ingrese Contraseña de envío
                    </div>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-sm-6 px-1">
                    <button type="button" class="btn btn-secondary btn-block" data-toggle="modal" data-target="#formModalTest" >
                        <i class="fas fa-mail-bulk"></i> Verificar
                    </button>
                </div>
                <div class="col-sm-6 px-1">
                    <button type="submit" class="btn btn-warning btn-block">
                        <i class="fas fa-save"></i> Guardar
                    </button>
                </div>
            </div>
        </form>
        <div>
            <div class="modal" id="formModalTest" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <form class="was-validated form-horizontal" novalidate id="formTest">
                            <div class="modal-header">
                                <h4 class="modal-title" id="exampleModalLabel">Prueba de Envío de Correo</h4>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">                                
                                <div class="form-group row">
                                    <label for="txtSendTo" class="col-sm-3 col-form-label">Destino:</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" id="txtSendTo"  minlength="3" maxlength="500" required />
                                        <div class="invalid-feedback">
                                            Por favor, ingrese destino.
                                        </div> 
                                    </div>                                    
                                </div>                                
                                <div class="form-group row">
                                    <label for="txtSubject" class="col-sm-3 col-form-label">Asunto:</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" id="txtSubject"  minlength="3" maxlength="500" value="Correo de Prueba" required />                                        
                                        <div class="invalid-feedback">
                                            Por favor, ingrese asunto.
                                        </div>
                                    </div>                                    
                                </div>
                                <div class="form-group row">
                                    <label for="txtBody" class="col-sm-3 col-form-label">Mensaje:</label>
                                    <div class="col-sm-9">
                                        <textarea class="form-control" id="txtBody"  minlength="3" maxlength="500" rows="5" required>Este es un correo de prueba</textarea>    
                                        <div class="invalid-feedback">
                                            Por favor, ingrese mensaje.
                                        </div>                                    
                                    </div>                                    
                                </div>
                                
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                                <button type="button" class="btn btn-primary" @click="${this.testconfig}">
                                    <i class="fas fa-paper-plane"></i> Enviar Correo
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        </div>`;
    }

    connectedCallback() {
        super.connectedCallback();

        this.$presenter.showLoading();
        this.$resource.get().then(
            r => {
                this.$presenter.hideLoading();
                this.fromEntityToForm(r);
                iziToast.success({ title: 'Cargado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: 'Error al cargar' });
            }
        );
    }

    fromEntityToForm(entity) {
        this.$entity = Notification.fromObject(entity);
        this.render();
    }

    fromFormToEntity() {
        this.$entity.NotifConfHost = this.$.txtNotifConfHost.value.trim();
        this.$entity.NotifConfPort = this.$.txtNotifConfPort.value.trim();
        this.$entity.NotifConfSSL = this.$.chkNotifConfSSL.checked ? '1' : '0';
        this.$entity.NotifConfUser = this.$.txtNotifConfUser.value.trim();
        this.$entity.NotifConfLock = this.$.txtNotifConfLock.value.trim();
    }

    save(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        this.fromFormToEntity();
        this.$presenter.showLoading();
        this.$resource.post(this.$entity).then(
            r => {
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: 'Error al grabar' });
            }
        );
    }

    testconfig(e){
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        this.fromFormToEntity();
        this.$presenter.showLoading();
        const prms = {
           sendTo:  this.$.txtSendTo.value.trim(),
           subject:  this.$.txtSubject.value.trim(),
           body:  this.$.txtBody.value.trim()
        };
        this.$resource.testconf(this.$entity, prms).then(
            r => {
                this.$presenter.hideLoading();
                console.log('Success: Correo enviado');
                iziToast.success({ title: 'Correo de prueba enviado - Favor de verificar.' });
            },
            err => {
                this.$presenter.hideLoading();
                console.error('Error: no se envio el correo', err);
                iziToast.error({ title: 'Error al enviar el correo de prueba.' });
            }
        );
    }
}
customElements.define('config-notification', NotificationComponent);