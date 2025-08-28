import { Router, Component, html } from '../../../core/core.js';
import { SessionResource } from '../../resources/resource-session.js';
import { User } from '../../resources/resource-user.js';
import { Request, RequestResource } from '../../resource-request.js';

export class WidgetRequestForApproval extends Component {
    constructor() {
        super(false);
        this.$resource = new RequestResource();
        this.data = null;
    }

    get $template() {
        if(!SessionResource.session.havePermission(User.ROLE_APROBADOR)) {
            return '';
        }

        return html`
        <style>
        </style>
        <div class="card">
            <!-- <div class="card-header ">Solicitudes por Revisar</div> -->
            <div class="card-body">
                <h4 class="card-title font-weight-bold">Solicitudes por Revisar</h4>
                ${this.data == null?
                html`
                <div class="text-center">
                    <i class="fa-3x fas fa-circle-notch fa-spin"></i>
                    <div class="font-weight-bold">Cargando...</div>
                </div>` :
                html`
                <div class="d-flex justify-content-end">
                    <p class="display-4 align-self-end mb-2">${this.data.count}</p>
                </div>
                <button class="btn btn-warning btn-block" ?disabled=${this.data.count == 0} @click=${() => Router.redirect('/request/forapprove')}>
                    <i class="fas fa-search"></i> Revisar Solicitudes
                </button>
                <h5 class="mt-4 mb-2">Solicitudes más antiguas</h5>
                <table class="table table-sm table-hover">
                    <thead class="thead-dark">
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Empresa</th>
                            <th scope="col">Para</th>
                            <th scope="col">Fecha</th>
                            <th scope="col"></th>
                        </tr>
                    </thead>
                    <tbody>
                    ${this.data.lastRequests.map(req => {
                        return html`<tr>
                            <th scope="row">${req.RequestNumber}</th>
                            <td>${req.RequestToCompanyName}</td>
                            <td>${req.RequestToName}</td>
                            <td>${req.RequestDateDisplay}</td>
                            <td>
                                <button class="btn btn-warning btn-sm" title="Revisar" @click=${e => Router.redirect(`/request/forapprove/${req.RequestID}`)}>
                                    <i class="fas fa-search"></i> 
                                </button>
                            </td>
                        </tr>`;
                    })}
                    </tbody>
                </table>`}
            </div>
        </div>`;
    }

    connectedCallback() {
        super.connectedCallback();
        if(SessionResource.session.havePermission(User.ROLE_APROBADOR)) {
            this.getData();
        }
    }

    getData() {
        this.$resource.getRequestPendings(SessionResource.session.sessionUser).then(
            r => {
                console.log('> Widget.RequestForApproval >>', r);
                this.data = {
                    count: r.count,
                    lastRequests: Request.fromList(r.lastRequests)
                };
                this.render();
            });
    }
}
customElements.define('widget-request-forapproval', WidgetRequestForApproval);