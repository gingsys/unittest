import { Component } from "./component.js";

export class ComponentView {
    constructor($component) {
        if(!($component instanceof Component)) {
            throw new Error(`> ComponentView.constructor(): [Error] Not valid Component.`);
        }
        this.__$component = $component;
    }

    get $component() {
        return this.__$component;
    }

    render() {
        return ``;
    }
}