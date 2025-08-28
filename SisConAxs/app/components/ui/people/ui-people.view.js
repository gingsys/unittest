import { ComponentView, html } from '../../../core/core.js';
import { People } from '../../resources/resource-people.js';
import { SessionResource } from '../../resources/resource-session.js';
import { User } from '../../resources/resource-user.js';

export class PeopleView extends ComponentView {
    render() {
        const $component = this.$component;
        return html`
        <style>
            people-main .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            people-main .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }
            /* .th-inner {
                white-space: break-spaces !important;
            } */
        </style>
        <h3>Destinatarios</h3>
        <div data-table-toolbar>
            <div class="form-group row mx-0 mt-0 mb-0" style="margin-left:-.5em !important;">
                <div class="col pl-2 pr-0">
                    <div class="btn-group">
                        <button type="button" class="btn btn-warning dropdown-toggle" data-toggle="dropdown" aria-expanded="false">
                            <i class="fas fa-plus"></i> Agregar
                        </button>
                        <div class="dropdown-menu">
                            ${this.colaboradorManual}                            
                            <a class="dropdown-item" href="javascript:void(0)" @click=${e => $component.add(People.CLASIFICACION_TERCERO)}>Tercero (Trabajador Externo)</a>
                            <a class="dropdown-item" href="javascript:void(0)" @click=${e => $component.add(People.CLASIFICACION_CLIENTE)}>Cliente</a>
                            <a class="dropdown-item" href="javascript:void(0)" @click=${e => $component.add(People.CLASIFICACION_PROVEEDOR)}>Proveedor</a>
                        </div>
                        <!-- <button type="button" class="btn btn-warning" @click=${e => $component.add($component.$.cboClasificationType.value)}>
                            <i class="fas fa-plus"></i> Agregar
                        </button> -->
                    </div>
                </div>
                <div class="col-auto pl-2 pr-0 mb-2">

                </div>
                <div class="col-auto pl-2 pr-0 mb-2">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <label class="input-group-text" for="cboClasificationType">Tipo</label>
                        </div>
                        <select id="cboClasificationType" class="custom-select" @change=${$component.applyFilter}>
                        </select>
                    </div>
                </div>
                <div class="col-auto pl-2 pr-0">
                    <button type="button" class="btn btn-outline-info float-right" id="btnViewAll" @click=${$component.btnViewAllClick}>
                        <i class="fas fa-binoculars"></i> Ver Todas las Empresas
                    </button>
                </div>
                <!-- <button id="btnImport" class="btn btn-success" title="Importar desde un archivo excel">
                    <i class="fas fa-file-excel"></i> Importar
                </button> -->
            </div>
        </div>
        <table
            data-table-main
            data-locale="es-ES"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <people-edit id="editor" @save=${$component.loadData} @delete=${$component.loadData}></people-edit>`;
    }

    get colaboradorManual() {
        const session = SessionResource.session
        if (session.CompanyID == 10 || session.CompanyID == 7 || session.havePermission(User.ROLE_SYSADMIN)) {
            return html`
                <a class="dropdown-item" href="javascript:void(0)" @click=${e => this.$component.add(People.CLASIFICACION_COLABORADOR_EN_PROCESO)}>Colaborador (Manual)</a>
            `;
        }
        return '';
    }
}