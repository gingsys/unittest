import { ComponentView, html } from "../../../core/core.js";
// import { UIWindowComponentView } from "../common/ui-window.js";

export class RequestTemplateEditView extends ComponentView {
    render() {
        const $component = this.$component;
        return html `
        <style>
            request-template-edit .row-select {
                display: block;
                width: 350px !important;
            }
            request-template-edit .valid-datepicker {
                width: calc(50% - 18px);
            }
        </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Solicitud Base</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="cboEmployeeType" class="col-sm-2 col-form-label">Tipo Empleado</label>
                                <div class="col-sm-4">
                                    <div class="input-group">
                                        <control-select id="cboEmployeeType"
                                            data-none-selected-text="Seleccione tipo empleado"
                                            data-live-search="true"
                                            data-size="8"
                                            required>
                                        </control-select>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="form-check">
                                    <input class="form-check-input" type="checkbox" id="chkActive">
                                    <label class="form-check-label" for="chkActive">
                                        Activo
                                    </label>
                                </div>
                            </div>
                            
                            <h5 class="mt-4">Detalle</h5>
                            <div class="table-responsive">
                                <div class="col">
                                    <div class="form-group row">
                                        <select id="cboCategoryResource" class="custom-select col-sm-2" @change=${$component.selectCategory}></select>
                                        <div id="searchResource" class="col-sm-6" style="padding-left: 4px;padding-right: 0px;"></div>
                                        <button type="button" class="btn btn-warning mx-2" @click=${$component.addDetail}><i class="fas fa-plus"></i> Agregar</button>
                                    </div>
                                </div>
                            </div>
                            <table
                                data-table-detail
                                data-locale="es-ES"
                                >
                                <thead>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            ${$component.$entity.ReqTemplateID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        
        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton" id="contextmenuTextInput">
            <button class="dropdown-item" data-value="[[solicitud_numero]]" @click=${$component.addTextInputValue}>Número de Solicitud</button>
            <button class="dropdown-item" data-value="[[solicitud_solicitante]]" @click=${$component.addTextInputValue}>Solicitante</button>
            <button class="dropdown-item" data-value="[[solicitud_para]]" @click=${$component.addTextInputValue}>Solicitado para</button>
            <button class="dropdown-item" data-value="[[solicitud_ticket]]" @click=${$component.addTextInputValue}>Ticket Atención</button>
        </div>`;
    }
}




// export class RequestTemplateEditView extends UIWindowComponentView {
//     get header() {
//         return html`<h4 class="modal-title">Solicitud Base</h4>`;
//     }

//     get body() {
//         const $component = this.$component;
//         return html`
//         <form class="was-validated form-horizontal" novalidate>
//             <div class="form-group row">
//                 <label for="cboEmployeeType" class="col-sm-2 col-form-label">Tipo Empleado</label>
//                 <div class="col-sm-4">
//                     <div class="input-group">
//                         <control-select id="cboEmployeeType"
//                             data-none-selected-text="Seleccione tipo empleado"
//                             data-live-search="true"
//                             data-size="8"
//                             required>
//                         </control-select>
//                     </div>
//                 </div>
//             </div>
//             <div class="form-group">
//                 <div class="form-check">
//                     <input class="form-check-input" type="checkbox" id="chkActive">
//                     <label class="form-check-label" for="chkActive">
//                         Activo
//                     </label>
//                 </div>
//             </div>
            
//             <h5 class="mt-4">Detalle</h5>
//             <div class="table-responsive">
//                 <div class="col">
//                     <div class="form-group row">
//                         <select id="cboCategoryResource" class="custom-select col-sm-2" @change=${$component.selectCategory}></select>
//                         <div id="searchResource" class="col-sm-6" style="padding-left: 4px;padding-right: 0px;"></div>
//                         <button type="button" class="btn btn-warning mx-2" @click=${$component.addDetail}><i class="fas fa-plus"></i> Agregar</button>
//                     </div>
//                 </div>
//             </div>
//             <table
//                 data-table-detail
//                 data-locale="es-ES"
//                 >
//                 <thead>
//                 </thead>
//                 <tbody></tbody>
//             </table>
//         </form>
//         `;
//     }

//     get footer() {
//         const $component = this.$component;
//         return html`
//         <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
//         ${$component.$entity.ReqTemplateID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
//         <button type="button" class="btn btn-success" id="btnSave" @click=${$component.save}><i class="fas fa-save"></i> Guardar</button>
//         `;
//     }

//     render() {
//         const styles = html`
//         <style>
//             request-template-edit .row-select {
//                 display: block;
//                 width: 350px !important;
//             }
//             request-template-edit .valid-datepicker {
//                 width: calc(50% - 18px);
//             }
//         </style>`;

//         return html`${styles}${super.render()}`;
//     }
// }