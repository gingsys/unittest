export class ControlSelect {
    constructor(select, options = {}) {
        this.__select = select;
        $(select).selectpicker(Object.assign({
            noneResultsText: 'No se encontaron resultados para {0}',
            noneSelectedText: 'Nada seleccionado',
            selectAllText: 'Seleccionar todos',
            deselectAllText: 'Quitar todos'
        }, options));
        $(select).selectpicker('setStyle', 'form-control');
        $(select).on('changed.bs.select', (e, clickedIndex, isSelected, previousValue) => {
            if(select.required) {
                if(this.value != null) {
                    $(select).selectpicker('setStyle', 'is-invalid', 'remove');
                    $(select).selectpicker('setStyle', 'is-valid', 'add');
                } else {
                    $(select).selectpicker('setStyle', 'is-valid', 'remove');
                    $(select).selectpicker('setStyle', 'is-invalid', 'add');
                }
            } else {
                $(select).selectpicker('setStyle', 'is-invalid', 'remove');
                $(select).selectpicker('setStyle', 'is-valid', 'add');
            }
            let $newValueData = clickedIndex != null? select.options[clickedIndex].$data : null;
            this.onChange(e, { clickedIndex, isSelected, previousValue, newValue: this.value, $newValueData });
        });
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
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

    clear() {
        this.__select.innerHTML = '';
        $(this.__select).selectpicker('refresh');
    }
    refresh() {
        console.error('Not implemented');
    }
    setData(data) {
        console.error('Not implemented');
    }

    // events --------------------------------------------
    onChange(e, data) {
        // console.log('select change.');
    }
}