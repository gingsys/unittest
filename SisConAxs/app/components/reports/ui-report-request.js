import { Component, html } from '../../core/core.js';
import { SessionResource } from '../resources/resource-session.js';

export class ReportRequestComponent extends Component {
    constructor() {
        super(false);
    }

    get $template() {
        return html `
            <style>
                report-request {
                    width: 100%;
                    height: 100%;
                    display: block;
                }
                .card {
                    box-shadow: 2px 1px 3px 0px #999;
                }
            </style>
            <!-- <h4><i class="fas fa-chart-bar"></i> Reporte de Requerimiento</h4> -->
            <iframe id="iframeReport" src='/Reports/frm_rpt_people.aspx?token=${encodeURIComponent(SessionResource.session.sessionToken)}' style="border:0px;width:100%;height:100%"></iframe>`;
    }

    connectedCallback() {
        super.connectedCallback();
    }
}
customElements.define('report-request', ReportRequestComponent);