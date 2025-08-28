export class EntityForm {
    static toForm(entity, formElement, mapFunction = {}) {
        formElement.reset();
        for (let prop in entity) {
            if(typeof entity[prop] !== 'function' && !!formElement.querySelector(`[name=${prop}]`)) {
                let value = entity[prop];

                if(Array.isArray(value)) {
                    let inputs = Array.from(formElement.querySelectorAll(`[name=${prop}]`));
                    if(!!mapFunction[prop]) {
                        mapFunction[prop](inputs, value, entity);
                    } else {
                        if(inputs.length == 1 && inputs[0] instanceof HTMLSelectElement && inputs[0].multiple) {
                            let select = inputs[0];
                            select.value = null;
                            for(let i = 0; i < select.options.length; i++) {
                                select.options[i].selected = value[input.options[i].value] !== undefined;
                            }
                        } else {
                            for(let input in inputs) {
                                if(input.type == "checkbox" || input.type == "radio") {
                                    input.checked = value[input.value] !== undefined;
                                }
                            }
                        }
                    }
                } else {
                    let input = formElement.querySelector(`[name=${prop}]`);
                    if(!!mapFunction[prop]) {
                        mapFunction[prop](input, value, entity);
                    } else {
                        if(input instanceof HTMLSelectElement) {
                            input.value = null;
                            if(input.multiple) {
                                let values = value.split(",");
                                for(let i = 0; i < input.options.length; i++) {
                                    input.options[i].selected = values.some(v => v == input.options[i].value);
                                }
                            } else {
                                input.value = value;
                            }
                        } else if(input.type == "checkbox" || input.type == "radio") {
                            input.checked = !!value;
                        } else {
                            input.value = value;
                        }
                    }
                }
            }
        }
    }
    static fromForm(entity, formElement, mapFunction = {}) {
        let formData = new FormData(formElement);
        let data = Array.from(formData.entries());
        
        for (let prop in entity) {
            if(typeof entity[prop] !== 'function' && !!formElement.querySelector(`[name=${prop}]`)) {
                if(Array.isArray(entity[prop])) {
                    entity[prop] = [];
                    entity[prop] = data.filter(entry => entry[0] == prop)
                                       .map(entry => mapFunction[prop]? mapFunction[prop](entry[1], entity) : entry[1]);
                } else {
                    let value = data.find(d => d[0] == prop)[1];
                    entity[prop] = mapFunction[prop]? mapFunction[prop](value, entity) : (value || null);
                }
            }
        }
    }
}