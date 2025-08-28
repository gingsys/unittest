import { ComponentView, html } from "../../../core/core.js";

export class CompanyEditView extends ComponentView {
    render() {
        const $component = this.$component;
        return html `
        <div class="modal" id="formModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <form class="was-validated form-horizontal" novalidate id="form" @submit=${$component.save}>
                        <div class="modal-header">
                            <h4 class="modal-title" id="exampleModalLabel">Empresa</h4>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <!-- <ul class="nav nav-tabs" id="myTab" role="tablist">
                                <li class="nav-item">
                                    <a class="nav-link active" id="tabData" data-toggle="tab" href="#home" role="tab" aria-controls="home" aria-selected="true"><i class="fab fa-wpforms"></i> Datos</a>
                                </li>
                                <li class="nav-item">
                                    <a class="nav-link" id="tabParams" data-toggle="tab" href="#profile" role="tab" aria-controls="profile" aria-selected="false"><i class="fas fa-th-list"></i> Parámetros</a>
                                </li>
                            </ul> -->
                            <!-- <div class="tab-content" id="myTabContent">
                                <div class="tab-pane fade show active pt-3" id="home" role="tabpanel" aria-labelledby="tabData"> -->

                                    <div class="form-group row">
                                        <label for="txtTaxpayerID" class="col-form-label col-3">ID Contribuyente:</label>
                                        <div class="col-9">
                                            <input type="text" class="form-control" id="txtTaxpayerID" minlength="3" maxlength="64" required>
                                            <div class="invalid-feedback">
                                                Por favor, ingrese el ID de Contribuyente (RUC, NIT, etc).
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtName" class="col-form-label col-3">Nombre:</label>
                                        <div class="col-9">
                                            <input type="text" class="form-control" id="txtName" minlength="3" maxlength="255" required>
                                            <div class="invalid-feedback">
                                                Por favor, ingrese un nombre correcto.
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtDisplay" class="col-form-label col-3">Etiqueta:</label>
                                        <div class="col-9">
                                            <input type="text" class="form-control" id="txtDisplay" minlength="3" maxlength="255" required>
                                            <div class="invalid-feedback">
                                                Por favor, ingrese una etiqueta correcta.
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group row">
                                        <label for="txtCountry" class="col-form-label col-3">País:</label>
                                        <div class="col-9">
                                            <div class="input-group mb-3" style="margin: 0px !important;">
                                                <select id="cboCountry" class="custom-bootstrapselect" data-none-selected-text="Seleccione país" data-live-search="true" data-size="8" required></select>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtAddress" class="col-form-label">Dirección:</label>
                                        <textarea class="form-control" id="txtAddress" rows="4" maxlength="1024"></textarea>
                                    </div>
                                    <div class="form-group">
                                        <div class="form-check">
                                            <input class="form-check-input" type="checkbox" id="chkActive" ?checked=${$component.$entity.CompanyActive > 0}>
                                            <label class="form-check-label" for="chkActive">
                                                Activo
                                            </label>
                                        </div>
                                    </div>

                                <!-- </div>
                                <div class="tab-pane fade pt-3" id="profile" role="tabpanel" aria-labelledby="tabParams">
                                    <company-edit-params id="companyParams"></company-edit-params>
                                </div>
                            </div> -->
                            
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cancelar</button>
                            ${$component.$entity.CompanyID > 0? html`<button type="button" class="btn btn-danger" id="btnDelete" @click=${$component.delete}><i class="fas fa-eraser"></i> Borrar</button>` : ''}
                            <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>`;
    }
}