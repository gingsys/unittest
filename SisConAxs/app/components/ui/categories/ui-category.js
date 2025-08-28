import { html } from "../../../core/core.js";
import { UIMainComponent } from '../common/ui-main.js';
import { CategoryResource, Category } from '../../resources/resource-category.js';
import './ui-category-edit.js';

export class CategoryComponent extends UIMainComponent {
    constructor() {
        super({ pagination: false });
        this.$resource = new CategoryResource();
        this.$entity = new Category();
    }

    get $template() {
        return html`
        <style>
            /* cols */
            /* .col-check {
                box-sizing: border-box;
                width: 25px;
            } */
            category-main .col-name {
                box-sizing: border-box;
                width: 120px;
            }
            category-main .col-desc {
                box-sizing: border-box;
                /* width: 120px; */
            }
            category-main .col-estado {
                box-sizing: border-box;
                width: 50px;
            }
            category-main .col-operate {
                box-sizing: border-box;
                width: 70px;
            }
            /* cols */

            category-main .status-not {
                width: 18px; height: 18px; border-radius: 3px; background-color: gray;
                position: relative;
                margin: auto;
            }
            category-main .status-ok {
                width: 18px; height: 18px; border-radius: 3px; background-color: limegreen;
                position: relative;
                margin: auto;
            }
        </style>
        <h3>Categorías</h3>
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
            data-search="true"
            data-show-refresh="true"
            >
            <thead>
            </thead>
            <tbody></tbody>
        </table>
        <category-edit id="editor" @save=${this.loadData} @delete=${this.loadData}></category-edit>`;
    }

    get Resource() {
        return CategoryResource;
    }
    get Entity() {
        return Category;
    }
    get entityID() {
        return 'CategoryID';
    }

    connectedCallback() {
        super.connectedCallback();
        this.loadData();
    }

    get tableColumns() {
        const $this = this;
        return [
            { title: "Nombre", field: "CategoryName", align: "left", valign: "middle", class: "col-nombre" },
            { title: "Descripción", field: "CategoryDescription", align: "left", valign: "middle", class: "col-desc" },
            // { title:"Estado", field:"Estado", align:"center", valign:"middle", class:"col-estado", formatter: $this.tableFormatterStatus },
            { title: "", field: "", align: "center", valign: "middle", class: "col-operate", formatter: $this.tableFormatterOperate, events: $this.tableEventColumnOperate }
        ];
    }
    
    tableFormatterStatus(value, row, index) {
        let className = value > 0 ? 'status-ok' : 'status-not';
        return `<div class="${className}"></div>`;
    }
}
customElements.define('category-main', CategoryComponent);





import { ControlSelect } from '../../ui-control-select.js';
export class SelectCategory extends ControlSelect {
    constructor(select) {
        super(select, {
            noneSelectedText: 'Seleccione categoría'
        });
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
    }

    refresh() {
        return CategoryResource.loadSelect(this.__select).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
    refreshFromData(data) {
        // let select = this.__select;
        // select.innerHTML = '';
        // data.forEach(item => {
        //     let opt = document.createElement('option');
        //     opt.innerHTML = item.CategoryName;
        //     opt.value = item.CategoryID;
        //     opt.$data = item;
        //     select.appendChild(opt);
        // });
        // select.value = null;
    }
}