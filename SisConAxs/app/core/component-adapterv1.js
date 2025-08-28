// Clase creada como adaptador para q sea compatible con la versión anterior
// NOTA: ELIMIAR ESTE ARCHIVO y ACTUALIZAR "app/core/core.js" cuando se termine de adaptar todos los módulos a la versión nueva

export { html, classMap, styleMap } from './component.js'
import { Component as ComponentV2 } from "./component.js";

export class Component extends ComponentV2 {
    constructor(shadow, options = {}) {
        super(shadow, options);
        this.$component = {};
    }

    connectedCallback() {
        super.connectedCallback();
        this.__readyComponents();
    }

    addComponent(name, component) {
        // component.$presenter = this.$presenter;
        this.$component[name] = component;
    }
    deleteComponent(name) {
        this.$component[name].remove();
        delete this.$component[name];
    }
    __readyComponents() {
        for(let name in this.$component) {
            this.querySelectorAll(`[data-component="${name}"]`).forEach(el => {
                this.$component[name].attach(el);
            });
        };
    }
}

export class Presenter extends ComponentV2 {
    constructor(shadow, options = {}) {
        super(shadow, options);
        this.dataset.ispresenter = "";
        this.$component = {};
    }

    connectedCallback() {
        super.connectedCallback();
        this.__readyComponents();
    }

    addComponent(name, component) {
        // component.$presenter = this;
        this.$component[name] = component;
    }
    deleteComponent(name) {
        this.$component[name].remove();
        delete this.$component[name];
    }
    __readyComponents() {
        for(let name in this.$component) {
            this.querySelectorAll(`[data-component="${name}"]`).forEach(el => {
                this.$component[name].attach(el);
            });
        };
    }
}