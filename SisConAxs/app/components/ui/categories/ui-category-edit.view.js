import { ComponentView, html } from "../../../core/core.js";

export class CategoryEditView extends ComponentView {

    render() {
        const $component = this.$component;
        return html`
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Categoría</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <label for="txtName" class="col-form-label">Nombre:</label>
                                <input type="text" class="form-control" id="txtName" minlength="3" maxlength="50">
                                <div class="invalid-feedback">
                                    Por favor, ingrese un nombre correcto.
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="txtDescription" class="col-form-label">Descripción:</label>
                                <textarea class="form-control" id="txtDescription" rows="4"></textarea>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            ${$component.$entity.CategoryID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }
}