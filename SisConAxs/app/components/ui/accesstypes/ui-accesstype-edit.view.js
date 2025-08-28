import { ComponentView, html } from "../../../core/core.js";

 export class AccessTypeEditView extends ComponentView {
    render() {
        const $component = this.$component;        
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Tipo de Acceso</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col">
                                    <div class="form-group row">
                                        <label for="txtName" class="col-sm-2 col-form-label">Nombre</label>
                                        <div class="col-sm-10">
                                            <input type="text" class="form-control" id="txtName" minlength="3" maxlength="100" required>
                                            <div class="invalid-feedback">
                                                Por favor, ingrese un nombre correcto.
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="cboAccessTypeType" class="col-sm-2 col-form-label">Tipo</label>
                                        <div class="col-sm-10">
                                            <select id="cboAccessTypeType" class="custom-select" @change=${$component.showDetail}>
                                                <option value="1">Simple</option>
                                                <option value="2">Texto</option>
                                                <option value="3">Selección Simple</option>
                                                <option value="4">Selección Múltiple</option>
                                                <option value="5">Correo Corporativo</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div id="toolbarDetail">
                                        <div class="btn-group">
                                            <button id="btnAddDetail" class="btn btn-warning" @click=${$component.addDetail}>
                                                <i class="fas fa-plus"></i> Agregar
                                            </button>
                                        </div>
                                    </div>
                                    <div id="tableDetailWrap">
                                        <table
                                            id="tableDetail"
                                            data-locale="es-ES"
                                            data-toolbar="#toolbarDetail"
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
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            ${$component.$entity.AccessTypeID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        
        
        <div class="modal" id="formModalDetail" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-nested" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="formDetail" @submit=${$component.formDetailSubmit}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Valor para Tipo de Acceso</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="txtDetailName" class="col-sm-2 col-form-label">Nombre</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailName" minlength="3" maxlength="100" required>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un nombre correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtDetailDisplay" class="col-sm-2 col-form-label">Etiqueta</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailDisplay" minlength="3" maxlength="100" required>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese una etiqueta correcta.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtDetailValue" class="col-sm-2 col-form-label">Valor</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailValue" minlength="3" maxlength="200" required>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un valor correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="cboDetailValueAdditional" class="col-sm-2 col-form-label">Dato Adicional</label>
                                <div class="col-sm-10">
                                    <select id="cboDetailValueAdditional" class="custom-select">
                                        <option value="0">- Ninguno -</option>
                                        <!--<option value="1">Simple</option>-->
                                        <option value="2">Texto</option>
                                        <!--<option value="3">Selección Simple</option>
                                        <option value="4">Selección Múltiple</option>
                                        <option value="5">Correo Corporativo</option>-->
                                        <!--<option value="5">Correo Corporativo</option>-->
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- <button type="button" class="btn btn-danger" id="btnDetailDelete"><i class="fas fa-eraser"></i> Borrar</button> -->
                            <button type="submit" class="btn btn-success"><i class="fas fa-check"></i> Aceptar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        `;
    }
 }