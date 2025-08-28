import { Component, html } from '../../../core/core.js';
import { UIMainComponent } from '../common/ui-main.js';
import { UserResource, User } from '../../resources/resource-user.js';
import { SessionResource } from '../../resources/resource-session.js';
import './ui-user-edit.js';

export class UserComponent extends UIMainComponent {
    constructor() {
        super()
        this.$resource = new UserResource();
        this.$entity = new User();
    }

    get $template() {
        return html `
        <style>
            /* cols */
            user-main .col-name {
                box-sizing: border-box;
                width: 120px;
            }
            user-main .col-desc {
                box-sizing: border-box;
                /* width: 120px; */
            }
            user-main .col-estado {
                box-sizing: border-box;
                width: 50px;
            }
            user-main .col-operate {
                box-sizing: border-box;
                width: 70px;
            }
            /* cols */

            user-main .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            user-main .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }
        </style>
        <h3>Usuarios</h3>
        <div data-table-toolbar>
            <div class="btn-group">
                <button id="btnAdd" class="btn btn-warning" @click=${this.add}>
                    <i class="fas fa-plus"></i> Agregar
                </button>
            </div>
        </div>
        <table
            data-table-main
            data-locale="es-ES"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <user-edit id="editor" @save=${this.loadData}></user-edit>`;
    }

    get Resource() {
        return UserResource;
    }
    get Entity() {
        return User;
    }
    get entityID() {
        return 'UserID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title: "Login", field: "UserInternalID", filterControl: "input", filterControlPlaceholder: "Login", align: "left", valign: "middle" },
            { title: "Apellido", field: "UserLastName", filterControl: "input", filterControlPlaceholder: "Apellido", align: "left", valign: "middle" },
            { title: "Nombre", field: "UserFirstName", filterControl: "input", filterControlPlaceholder: "Nombre", align: "left", valign: "middle" },
            // { title: "Solicitante", field: "UserRole1", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            // { title: "Aprobador", field: "UserRole2", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            // { title: "Administrador", field: "UserRole3", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            // { title: "Crear personas", field: "UserRole4", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            // { title: "Dar de baja", field: "UserRole5", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            // { title: "Ver solicitudes proyecto", field: "UserRole6", align: "center", valign: "middle", formatter: $this.tableFormatterCheck },
            { title: "Activo", field: "UserStatusDesc", filterControl: "input", filterControlPlaceholder: "Activo", align: "center", valign: "middle" },
            // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
            { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }

    prepareFilters(filters) {
        super.prepareFilters(filters);
    }

    tableFormatterOperate(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group">
                    <button type="button" class="btn btn-secondary btn-edit" title="Editar"><i class="fas fa-edit"></i></button>
                    ${SessionResource.session.UserRoleSysAdmin > 0?
                        `<button type="button" class="btn ${row.UserRoleSysAdmin > 0? `btn-outline-success` : `btn-outline-secondary`} btn-setsysadmin" title="Rol SysAdmin"><i class="fas fa-key"></i></button>` :
                        ''}
                </div>`;
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        const $this = this;
        return {
            'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .btn-setsysadmin': $this.btnColumnSetSysAdminClick.bind($this)
        }
    }
    btnColumnEditClick(e, value, row, index) {
        this.edit(row[this.entityID]);
    }
    btnColumnSetSysAdminClick(e, value, row, index) {
        this.$presenter.question(
            'Establecer como SysAdmin',
            `Desea ${row.UserRoleSysAdmin > 0? 'quitarle' : 'otorgarle'} el rol de SysAdmin a <br><strong>${row.UserLastName}, ${row.UserFirstName}<br>${row.UserInternalID}</strong> ?`,
            (instance, toast) => {
                this.$presenter.showLoading();
                this.$resource.setSysAdmin(row.UserInternalID, 1 - row.UserRoleSysAdmin)
                .then(
                    r => {
                        this.loadData();
                        this.$presenter.hideLoading();
                        iziToast.success({ title: 'Grabado correctamente' });
                        console.log('grabado!');
                    },
                    err => {
                        this.$presenter.hideLoading();
                        iziToast.error({ title: err });
                    }
                );
            }
        );
    }
}
customElements.define('user-main', UserComponent);