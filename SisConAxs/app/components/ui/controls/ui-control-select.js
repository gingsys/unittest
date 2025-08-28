import { Component, html } from '../../../core/core.js';

export class ControlSelect extends Component {
    static get observedAttributes() { return ['required', 'disabled', 'multiple']; }

    constructor() {
        super(false);
        this.__options = {};
        this.__select = null;
    }

    get $template() {
        return html`
            <style>
                control-select {
                    flex-basis: auto;
                    flex-grow: 1;
                    flex-shrink: 1;
                }
                control-select .bootstrap-select {
                    width: 100% !important;
                }
                control-select .btn.dropdown-toggle.disabled {
                    border: 1px solid #AAA;
                    background-color: #e9ecef;
                }
                control-select .btn.dropdown-toggle {
                    border: 1px solid #AAA;
                    background-color: white;
                }
                control-select .bs-searchbox input {
                    background-repeat: no-repeat;
                    background-position: right 5px center;
                    background-size: 15px;
                }
            </style>
            <select class="custom-bootstrapselect" @change=${e => e.stopPropagation()}></select>`;
    }

    connectedCallback() {
        super.connectedCallback();
        const attributes = this.getAttributeNames().filter(attr => attr.indexOf('data-') > -1);
        // const select = this.__select = this.shadowRoot.querySelector('slot').assignedElements()[0];
        const select = this.__select = this.$one('select');
        attributes.forEach(attr => {
            select.setAttribute(attr, this.getAttribute(attr));
        });
        select.required = this.hasAttribute('required');
        select.multiple = this.hasAttribute('multiple');
        select.disabled = this.hasAttribute('disabled');
        
        $(select).selectpicker(Object.assign({
            noneResultsText: 'No se encontaron resultados para {0}',
            noneSelectedText: 'Nada seleccionado',
            selectAllText: 'Seleccionar todos',
            deselectAllText: 'Quitar todos'
        }, this.__options));
        $(select).selectpicker('setStyle', 'form-control');
        $(select).on('changed.bs.select', (e, clickedIndex, isSelected, previousValue) => {
            this.__setComponetStyle();

            // const newValueData = clickedIndex != null? select.options[clickedIndex].$data : null;
            const detail = { 
                clickedIndex, isSelected,
                previousValue,
                newValue: this.value,
                newDisplay: this.display,
                newData: this.data
                // newValueData
            };
            this.dispatchEvent(new CustomEvent('change', { detail, bubbles: false }));
            if(this.value != null) {
                this.dispatchEvent(new CustomEvent('select', { detail }));
            }
        });
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });

        if(select.required) {
            $(select).selectpicker('setStyle', 'is-invalid', 'add');
        }
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        $(this.__select).selectpicker('destroy');
    }

    attributeChangedCallback(name, oldValue, newValue) {
        if(!this.isConnected) return;

        const select = $(this.__select);
        if(name == 'required') {
            select.required = this.hasAttribute('required');
            $(select).prop('required', select.required);
            this.refresh();
            this.__setComponetStyle();
        } else if(name == 'disabled') {
            select.disabled = this.hasAttribute('disabled');
            $(select).prop('disabled', select.disabled);
            this.refresh();
            this.__setComponetStyle();
        } else if(name == 'multiple') {
            select.multiple = this.hasAttribute('multiple');
            select.selectpicker('render');
        }
    }

    get value() {
        return $(this.__select).selectpicker('val');
    }
    set value(val) {
        $(this.__select).selectpicker('val', val);
    }
    get display() {
        if(this.__select.selectedIndex > -1) {
            return this.__select.options[this.__select.selectedIndex].text.trim();
        }
        return null;
    }
    get data() {
        if(this.__select.selectedIndex > -1) {
            return this.__select.options[this.__select.selectedIndex].$data;
        }
        return null;
    }

    get required() {
        return this.hasAttribute('required');
    }
    set required(val) {
        if(val === true) {
            this.setAttribute('required', '');
        } else {
            this.removeAttribute('required');
        }
    }

    get disabled() {
        return this.hasAttribute('disabled');
    }
    set disabled(val) {
        if(val === true) {
            this.setAttribute('disabled', '');
        } else {
            this.removeAttribute('disabled');
        }
    }

    get multiple() {
        return this.hasAttribute('multiple');
    }
    set multiple(val) {
        if(val === true) {
            this.setAttribute('multiple', '');
        } else {
            this.removeAttribute('multiple');
        }
    }

    clear() {
        this.__select.innerHTML = '';
        $(this.__select).selectpicker('refresh');
    }
    refresh() {
        $(this.__select).selectpicker('refresh');
    }
    setData(data, defaultValue = null) {
        this.__select.innerHTML = '';
        data.forEach(item => {
            const option = document.createElement('option');
            option.$data = item.data;
            option.value = item.value;
            option.innerHTML = item.display;
            if(!!item.content) {
                option.setAttribute('data-content', item.content);
            }
            if(!!item.subtext) {
                option.setAttribute('data-subtext', item.subtext);
            }
            if(!!item.icon) {
                option.setAttribute('data-icon', item.icon);
            }
            this.__select.appendChild(option);
        });
        this.__select.value = defaultValue;
        this.refresh();
    }

    fn(name) {
        return $(this.__select).selectpicker(name);
    }

    __setComponetStyle() {
        const select = this.__select;
        if(select.disabled) {
            $(select).selectpicker('setStyle', 'is-invalid', 'remove');
            $(select).selectpicker('setStyle', 'is-valid', 'remove');
            return;
        }
        if(select.required) {
            if(this.value == null || (this.hasAttribute('multiple') && this.value.length == 0)) {
                $(select).selectpicker('setStyle', 'is-valid', 'remove');
                $(select).selectpicker('setStyle', 'is-invalid', 'add');
            } else {
                $(select).selectpicker('setStyle', 'is-invalid', 'remove');
                $(select).selectpicker('setStyle', 'is-valid', 'add');
            }
        } else {
            $(select).selectpicker('setStyle', 'is-invalid', 'remove');
            $(select).selectpicker('setStyle', 'is-valid', 'remove');
        }
    }
}
customElements.define('control-select', ControlSelect);