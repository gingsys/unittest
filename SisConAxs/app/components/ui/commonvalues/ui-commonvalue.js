import { Component, html } from '../../../core/core.js';
import { UIMainComponent } from '../common/ui-main.js';
import { CommonValueResource, CommonValueSet, CommonValue } from '../../resources/resource-commonvalue.js';
import { ControlSelect } from '../../ui-control-select.js';
import './ui-commonvalue-edit.js';
import { SessionResource } from '../../resources/resource-session.js';

export class CommonValueComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
        this.$resource = new CommonValueResource();
        this.$entity = new CommonValueSet();
    }

    get $template() {
        return html`
        <style>
            /* cols */
            /* .col-check {
                box-sizing: border-box;
                width: 25px;
            } */
            .col-name {
                box-sizing: border-box;
                width: 120px;
            }
            .col-desc {
                box-sizing: border-box;
                /* width: 120px; */
            }
            .col-estado {
                box-sizing: border-box;
                width: 50px;
            }
            .col-operate {
                box-sizing: border-box;
                width: 70px;
            }
            /* cols */

            .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }
        </style>
        <h3>Valores Comunes</h3>
        <div data-table-toolbar>
            <div class="btn-group">
                <!-- <button id="btnAdd" class="btn btn-warning">
                    <i class="fas fa-plus"></i> Agregar
                </button> -->
            </div>
        </div>
        <table
            data-table-main
            data-locale="es-ES"
            data-search="true"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <commonvalue-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></commonvalue-edit>`;
    }

    get Resource() {
        return CommonValueResource;
    }
    get Entity() {
        return CommonValueSet;
    }
    get entityID() {
        return 'CommonValueSetID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title:"Nombre", field:"CommonValueSetName", align:"left", valign:"middle", class:"col-nombre" },
            { title:"Descripción", field:"CommonValueSetDesc", align:"left", valign:"middle", class:"col-desc" },
            // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
            { title:"", field:"", align:"center", valign:"middle", class:"col-operate", formatter: $this.tableFormatterOperate, events:$this.tableEventColumnOperate }
        ];
    }

    prepareFilters(filters) {
        if(SessionResource.session.UserRoleSysAdmin == 0) {
            filters.OnlyRestrictedByCompany = true;
        }
        filters.SystemCompanySets = false;
    }

    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
}
customElements.define('commonvalue-main', CommonValueComponent);




// export class CommonValueSearchComponent extends Component {
//     constructor(presenter, parent) {
//         super(presenter);
//         this.$parent = parent;

//         this.$resource = new CommonValueResource();
//         this.commonValueSetName = null;
//         this.company = null;
//     }

//     __render() {
//         return html`
//         <div class="modal" id="formPeopleSearchModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
//             <div class="modal-dialog modal-xl" role="document">
//                 <div class="modal-content">
//                     <form class="was-validated form-horizontal" novalidate id="form">
//                         <div class="modal-header">
//                             <h4 class="modal-title" id="exampleModalLabel">Seleccionar <span id="lblCommonValueSetName">{{name}}</span></h4>
//                             <button type="button" class="close" data-dismiss="modal" aria-label="Close">
//                                 <span aria-hidden="true">&times;</span>
//                             </button>
//                         </div>
//                         <div class="modal-body">
//                             <div id="toolbarCommonValueSearch">
//                                 <div class="btn-group">
//                                     <!-- <button id="btnAdd" class="btn btn-warning">
//                                         <i class="fas fa-plus"></i> Agregar
//                                     </button> -->
//                                 </div>
//                             </div>
//                             <table
//                                 id="table"
//                                 data-locale="es-ES"
//                                 data-toolbar="#toolbarCommonValueSearch"
//                                 data-search="true"
//                                 data-show-refresh="true"
//                                 >
//                                 <thead>
//                                 </thead>
//                                 <tbody></tbody>
//                             </table>
//                         </div>
//                         <div class="modal-footer">
//                             <button type="button" class="btn btn-default" data-dismiss="modal"><i class="fas fa-window-close"></i> Cerrar</button>
//                             <!-- <button type="submit" class="btn btn-success" id="btnSave"><i class="fas fa-save"></i> Guardar</button> -->
//                         </div>
//                     </form>
//                 </div>
//             </div>
//         </div>`;
//     }

//     __ready(params) {
//         let $this = this;
//         this.$$.table.bootstrapTable({
//             height: window.innerHeight - 255,
//             columns: [
//                 { title:"Nombre", field:"CommonValueName", align:"left", valign:"middle" },
//                 { title:"Etiqueta", field:"CommonValueDisplay", align:"left", valign:"middle" },
//                 { title:"Descripción", field:"CommonValueDesc", align:"left", valign:"middle" },
//                 { title:"", field:"", align:"center", valign:"middle", width:"120", formatter: $this.tableFormatterOperate, events:$this.tableEventColumnOperate }
//             ],
//             onRefresh: () => {
//                 this.loadData();
//             }
//         });
//     }

//     show(commonValueSetName, company = null) {
//         this.commonValueSetName = commonValueSetName;
//         this.company = company;

//         if(commonValueSetName == CommonValueSet.SET_PROYECTOS)
//             this.$.lblCommonValueSetName.innerHTML = 'Proyecto';
//         if(commonValueSetName == CommonValueSet.SET_AREAS)
//             this.$.lblCommonValueSetName.innerHTML = 'Área';
//         if(commonValueSetName == CommonValueSet.SET_CARGOS)
//             this.$.lblCommonValueSetName.innerHTML = 'Cargo';

//         this.$$.table.bootstrapTable('load', []);
//         this.$$.table.bootstrapTable('resetSearch');
//         this.loadData();
//         this.$$.formPeopleSearchModal.modal({ backdrop: 'static' });
//     }

//     loadData() {
//         this.$presenter.showLoading();
//         this.$resource.getValues(this.commonValueSetName, this.company).then(r => {
//             this.$$.table.bootstrapTable('load', CommonValue.fromList(r));
//             this.$presenter.hideLoading();
//             iziToast.success({title: 'Cargado correctamente'});
//         },
//         err => {
//             iziToast.error({title:'Error al cargar'});
//         });
//     }


//     tableFormatterOperate(value, row, index) {
//         return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
//                     <button type="button" class="btn btn-outline-primary btn-select" title="Seleccionar">
//                         <i class="fas fa-check"></i> Seleccionar
//                     </button>
//                 </div>`;
//     }

//     // Table Events ------------------------------------------------------------------ //
//     get tableEventColumnOperate() {
//         let _self = this;
//         return {
//             'click .btn-select': _self.btnColumnSelectClick.bind(_self)
//         }
//     }
//     btnColumnSelectClick(e, value, row, index) {
//         this.__onSelect(row);
//     }

//     __onSelect(entity) {
//         console.log('Selected', entity);
//         this.$$.formPeopleSearchModal.modal('hide');
//         this.onSelect(entity);
//     }
//     onSelect(entity) {}
// }




export class SelectCommonValue extends ControlSelect {
    constructor(select, commonValueSetID) {
        super(select);
        this.__commonValueSetID = commonValueSetID;
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
    }

    refresh(companyID = null) {
        return CommonValueResource.loadSelect(this.__select, this.__commonValueSetID, companyID).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
}