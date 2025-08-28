import { Component, html } from '../core.js';

export class AlertComponent extends Component {
    constructor(presenter) {
        super(presenter);
        this.html = "";
    }

    __render() {
        return html ` 
        <div class="modal fade" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel"><i class="fas fa-exclamation-circle"></i> Alerta</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    ${this.html}
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-warning" data-dismiss="modal"><i class="fas fa-window-close"></i> Cerrar</button>
                </div>
                </div>
            </div>
        </div>
        `;
    }

    __ready() {
    }

    show(request) {
        this.html = request;
        this.render();
        this.$$.formModal.modal({ backdrop: 'static' });
    }
}