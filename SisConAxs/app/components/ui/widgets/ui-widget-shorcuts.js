import { Router, Component, html } from '../../../core/core.js';
import { SessionResource } from '../../resources/resource-session.js';
import { User } from '../../resources/resource-user.js';

export class WidgetShorcuts extends Component {

    constructor() {
      super(false);
    }

    get $template() {
      return html`
      <style>
        .icon-shorcut {
          display: block;
          font-size: 1.5em;
        }
      </style>
      <div class="card text-white bg-dark my-4">
        <div class="card-header "><i class="fas fa-external-link-alt"></i> Accesos Directos</div>
        <div class="card-body pb-2">
          <!-- <p class="card-text">Some quick example text to build on the card title and make up the bulk of the card's content.</p>
          <a href="#" class="btn btn-primary">Go somewhere</a> -->
          <div class="row">
            ${SessionResource.session.havePermission(User.ROLE_SOLICITANTE)?
              html`
              <div class="col-md-4 mb-2">
                <button class="btn btn-outline-warning btn-lg btn-block" @click=${this.newRequest}>
                  Crear Nueva Solicitud
                  <i class="fas fa-plus-square icon-shorcut"></i>
                </button>
              </div>
              <div class="col-md-4 mb-2">
                <button class="btn btn-outline-warning btn-lg btn-block" @click=${e => Router.redirect('/request/send')}>
                  Mis Solicitudes Emitidas
                  <i class="fas fa-paper-plane icon-shorcut"></i>
                </button>
              </div>` : ''}
            <!-- ${SessionResource.session.havePermission(User.ROLE_APROBADOR)?
              html`<div class="col-md-4">
                <button class="btn btn-outline-success btn-lg btn-block" @click=${e => Router.redirect('/request/forapprove')}>
                  Aprobar Solicitudes
                  <i class="fas fa-check-square icon-shorcut"></i>
                </button>
              </div>` : ''} -->
            ${SessionResource.session.havePermission(User.ROLE_ADMINISTRADOR | User.ROLE_VER_SOLICITUDES_PROYECTO) ?
              html`<div class="col-md-4 mb-2">
                <button class="btn btn-outline-info btn-lg btn-block" @click=${e => Router.redirect('/request/search')}>
                  Consultar Todas las Solicitudes
                  <i class="fas fa-search icon-shorcut"></i>
                </button>
              </div>` : ''}
          </div>
        </div>
      </div>`;
    }

    connectedCallback() {
      super.connectedCallback();
    }

    newRequest() {
      Router.redirect('/request/send');
      const $presenter = this.$presenter;
      setTimeout(() => $presenter.$component.request.add(), 1500);
    }
}
customElements.define('widget-shorcuts', WidgetShorcuts);