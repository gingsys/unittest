import { Component, html } from '../../core/core.js';
import { SessionResource } from '../resources/resource-session.js';

export class RptBitacoraComponent extends Component {
    constructor() {
        super(false);
        this.height = document.documentElement.clientHeight - 110;
    }

    get $template() {
        return html`
        <style>
            report-bitacora {
                width: 100%;
                height: 100%;
                display: block;
            }
        </style>
        <!-- <h4><i class="fas fa-chart-bar"></i> Reporte de Accesos del Personal</h4> -->
        <iframe src='/Reports/frm_rpt_bitacora.aspx?token=${encodeURIComponent(SessionResource.session.sessionToken)}' style="border:0px;width:100%;height:100%"></iframe>`;
    }

    connectedCallback() {
        super.connectedCallback();
    }
}
customElements.define('report-bitacora', RptBitacoraComponent);