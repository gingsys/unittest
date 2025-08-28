import { ComponentView, html } from "../../../core/core.js";

export class CommonValueEditView extends ComponentView {
    render() {
        const $component = this.$component;
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Valor Comúm</h4>
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
                                            <input type="text" class="form-control" id="txtName" required disabled>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtDescription" class="col-sm-2 col-form-label">Descripción</label>
                                        <div class="col-sm-10">
                                            <input type="text" class="form-control" id="txtDescription" disabled>
                                        </div>
                                    </div>
                                    <div id="toolbarDetail">
                                        <div class="btn-group">
                                            <button id="btnAddDetail" class="btn btn-warning" @click=${$component.btnAddDetailClick}>
                                                <i class="fas fa-plus"></i> Agregar
                                            </button>
                                        </div>
                                    </div>
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
                            <!-- <div class="row">
                                <div class="col-sm-7">
                                    
                                </div>
                                <div class="col-sm-5">
                                    <h5>Detalle</h5>
                                    <div class="form-group row">
                                        <label for="txtDetailName" class="col-sm-2 col-form-label">Nombre</label>
                                        <div class="col-sm-10">
                                            <input type="text" class="form-control" id="txtDetailName" required>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtDetailDisplay" class="col-sm-2 col-form-label">Etiqueta</label>
                                        <div class="col-sm-10">
                                            <input type="text" class="form-control" id="txtDetailDisplay" required>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtDetailDescription" class="col-form-label">Descripción:</label>
                                        <textarea class="form-control" id="txtDetailDescription" rows="4"></textarea>
                                    </div>
                                </div>
                            </div> -->
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- <button type="button" class="btn btn-danger" id="btnDelete"><i class="fas fa-eraser"></i> Borrar</button> -->
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
                            <h4 class="modal-title" id="exampleModalLabel">Valor</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group row">
                                <label for="txtDetailName" class="col-sm-2 col-form-label">Nombre</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailName" minlength="3" maxlength="150" required>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese un nombre correcto.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group row">
                                <label for="txtDetailDisplay" class="col-sm-2 col-form-label">Etiqueta</label>
                                <div class="col-sm-10">
                                    <input type="text" class="form-control" id="txtDetailDisplay" minlength="3" maxlength="150" required>
                                    <div class="invalid-feedback">
                                        Por favor, ingrese una etiqueta correcta.
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="txtDetailDescription" class="col-form-label">Descripción:</label>
                                <textarea class="form-control" id="txtDetailDescription" rows="4"></textarea>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            <!-- <button type="button" class="btn btn-danger" id="btnDetailDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button> -->
                            <button type="submit" class="btn btn-success" id="btnDetailAceptar"><i class="fas fa-check"></i> Aceptar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        `;
    }
}