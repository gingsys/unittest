import { Component, html } from '../../core/core.js';
import { SessionResource } from '../resources/resource-session.js';

export class ReportUsersRolesComponent extends Component {
    constructor() {
        super(false);
        this.height = document.documentElement.clientHeight - 110;
    }

    get $template() {
        return html`
        <style>
            report-users-roles {
                width: 100%;
                height: 100%;
                display: block;
            }
        </style>       
        <iframe src='/Reports/frm_rpt_req_auditoria_user_rol_emp.aspx?token=${encodeURIComponent(SessionResource.session.sessionToken)}' style="border:0px;width:100%;height:100%"></iframe>`;
    }

    connectedCallback() {
        super.connectedCallback();
    }
}
customElements.define('report-users-roles', ReportUsersRolesComponent);