import { Component, html } from '../../../core/core.js';

export class ControlDate extends Component {
    static get observedAttributes() { return ['required', 'disabled', 'showclear']; }

    constructor() {
        super(false);
        this.__datepicker = null;
        this.__input = null;
        this.__btnDate = null;
        this.__btnClear = null;
    }

    get $template() {
        return html`
        <style>
            control-date input {
                border: none;
                width: 100%;
            }
            control-date .btn-clear {
                margin-left: -1px;
            }
        </style>
        <!-- <input type="text" class="form-control date input-date" @keypress=${e => {e.preventDefault(); return false;}} /> -->
        <div class='input-group date'>
            <input type='text' class="form-control input-date" @keypress=${e => {e.preventDefault(); return false;}} />
            <span class="input-group-addon">
                <button type="button" class="btn btn-outline-secondary btn-date"><i class="fa fa-calendar"></i></button>
            </span>
            <button type="button" class="btn btn-outline-secondary btn-clear" @click=${e => this.value = null}><i class="fa fa-minus"></i></button>
        </div>
        `;
    }
    
    connectedCallback() {
        super.connectedCallback();

        this.__datepicker = this.$one('.date');
        this.__input = this.$one('.input-date');
        this.__btnDate = this.$one('.btn-date');
        this.__btnClear = this.$one('.btn-clear');
        
        this.__input.required = this.hasAttribute('required');
        this.__input.disabled = this.hasAttribute('disabled');
        this.__btnDate.disabled = this.hasAttribute('disabled');
        this.__btnClear.disabled = this.hasAttribute('disabled');

        this.__btnClear.hidden = !this.hasAttribute('showclear');

        const $datepicker = $(this.__datepicker);
        $datepicker.datetimepicker({
            ignoreReadonly: true,
            format: 'DD/MM/YYYY',
            locale: 'es',
            widgetPositioning: {
                horizontal: 'left',
                vertical: 'bottom'
            },
            date: null,
            // maxDate: new Date(),
            icons: {
                next: 'fas fa-chevron-right',
                previous: 'fas fa-chevron-left',
                clear: 'fas fa-trash',
            }
        });
    }

    attributeChangedCallback(name, oldValue, newValue) {
        if(!this.isConnected) return;

        if(name == 'required') {
            this.__input.required = this.hasAttribute('required');
        } else if(name == 'disabled') {
            this.__input.disabled = this.hasAttribute('disabled');
            this.__btnDate.disabled = this.hasAttribute('disabled');
            this.__btnClear.disabled = this.hasAttribute('disabled');
        } else if(name == 'showclear') {
            this.__btnClear.hidden = !this.hasAttribute('showclear');
        }
    }

    get value() {
        return $(this.__datepicker).data('DateTimePicker').date();
    }
    set value(val) {
        if(typeof val === 'string' || val instanceof String) {
            val = moment(val);
        }
        $(this.__datepicker).data('DateTimePicker').date(val);
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

    get showclear() {
        return this.hasAttribute('showclear');
    }
    set showclear(val) {
        if(val === true) {
            this.setAttribute('showclear', '');
        } else {
            this.removeAttribute('showclear');
        }
    }

    get fn() {
        return $(this.__datepicker).data('DateTimePicker');
    }
    
    on(event, fn) {
        return $(this.__datepicker).on(event, fn);
    }
}
customElements.define('control-date', ControlDate);