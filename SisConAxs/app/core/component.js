import { render, TemplateResult } from '../../node_modules/lit-html/lit-html.js';
import { html } from '../../node_modules/lit-html/lit-html.js';
export { html } from '../../node_modules/lit-html/lit-html.js';
export { classMap } from '../../node_modules/lit-html/directives/class-map.js';
export { styleMap } from '../../node_modules/lit-html/directives/style-map.js';
import { ComponentView } from './component-view.js';

// class ComponentBase extends HTMLElement {
    // __getElementById(id) {
    //     let el = Array.from(this.querySelectorAll(`#${id}`));
    //     let out = Array.from(this.querySelectorAll(`[data-iscomponent="${this.dataset.iscomponent}"] [data-iscomponent] #${id}`));
    //     return el.find(i => !out.includes(i));
    // }

    // __handleSelectors() {
    //     Object.defineProperty(this, '__$', {
    //         writable: false,
    //         value: new Proxy({}, {
    //             get: (obj, prop) => this.__getElementById(prop)
    //         })
    //     });
    //     Object.defineProperty(this, '__$$', {
    //         writable: false,
    //         value: new Proxy({}, {
    //             get: (obj, prop) => {
    //                 if (!!window.jQuery) {
    //                     return window.jQuery(this.__getElementById(prop));
    //                 }
    //                 throw "jQuery not loaded";
    //             }
    //         })
    //     });
    // }
// }

export class Component extends HTMLElement {
    static get observedAttributes() { return []; }
    static get attrsToProperties() { return {}; }

    constructor(shadow, options = {}) {
        super();

        if(typeof shadow != 'boolean') {
            throw new Error(`> Component.constructor: [Error] '${this.tagName}' >> shadow isn't a boolean.`);
        }        
        if(shadow) {
            this.attachShadow({ mode: 'open' });
        }

        this.__$options = Object.assign({
            rescan: false
        }, options);

        this.__$ = {};
        this.__$$ = {};
        this.__$attr = {};
        this.__$presenter = null;
        this.__$parent = null;
        this.__handleAttributes();
        // this.__handleSelectors();
        this.__handleLinkedProperties();
    }

    __generateID() {
        let time = (new Date()).getTime();
        return Math.floor(Math.random() * time).toString(16);
    }

    __handleAttributes() {
        Object.defineProperty(this, '__$attr', {
            writable: false,
            value: new Proxy({}, {
                get: (target, prop) => this.getAttribute(prop),
                set: (target, prop, value) => {
                    this.setAttribute(prop, value);
                    return true;
                },
                deleteProperty: (target, prop) => {
                    this.removeAttribute(prop);
                    return true;
                }
            })
        });
    }

    // New feature
    // status: beta
    __handleLinkedProperties() {
        const defineProperty = (prop, options) => {
            let type;
            let val;
            if(typeof options === 'object') {
                type = options.type
                val = options.value || undefined;
            } else {
                type = options;
            }
            if(!(type == Boolean || type == Number || type == BigInt || type == String || type == Array)) {
                throw new Error(`> Component.attrsToProperties: [Error] '${this.tagName}' >> type param only allowed Boolean, Number, BigInt, String or Array types.`);
            }

            Object.defineProperty(this, prop, {
                configurable: false,
                enumerable: true,
                get: () => {
                    if(type === Boolean) {
                        return this.hasAttribute(prop);
                    }
                    if(type === Array) {
                        return(this.getAttribute(prop) || '').split(',');
                    }
                    return type(this.getAttribute(prop) || '');
                },
                set: (value) => {
                    if(value === undefined || value === null || (type === Boolean && !value) || (type === String && !value)) {
                        this.removeAttribute(prop);
                    } else {
                        value = type(value);
                        if(type === Boolean) {
                            this.setAttribute(prop, '');
                        } else {
                            this.setAttribute(prop, value);
                        }
                    }
                    return true;
                }
            });

            if(this.getAttribute(prop) === undefined || this.getAttribute(prop) === null)
                this[prop] = val;
        }
        for(let prop in this.constructor.attrsToProperties) {
            defineProperty(prop, this.constructor.attrsToProperties[prop]);
        }
    }

    __getParent() {
        this.__$presenter = this.closest(`[data-ispresenter]`);
        this.__$parent = this.closest(`[data-iscomponent]:not([data-iscomponent="${this.dataset.iscomponent}"])`);
    }

    __getElements() {
        this.__$ = {};
        this.__$$ = {};

        let out = Array.from(this.$all(`[data-iscomponent="${this.dataset.iscomponent}"] [data-iscomponent] [id]:not([id=""])`))
                            .map(el => el);
        Array.from(this.$all(`[id]:not([id=""])`))
        .filter(el => out.length == 0 || !out.includes(el))
        .forEach(el => {
            this.__$[el.id] = el;
            if (!!window.jQuery)
                this.__$$[el.id] = $(el);
        });
    }

    __setStyles() {
        this.adoptedStyleSheets = this.$styles;
    }

    get $() {
        return this.__$;
    }
    get $$() {
        return this.__$$;
    }
    get $attr() {
        return this.__$attr;
    }
    get $presenter() {
        return this.__$presenter;
    }
    get $parent() {
        return this.__$parent;
    }

    get $styles() {
        return [];
    }
    get $template() {
        return html``;
    }

    $one(selector) {
        return (this.shadowRoot || this).querySelector(selector);
    }
    $all(selector) {
        return (this.shadowRoot || this).querySelectorAll(selector);
    }

    // New feature, not compatible with Firefox
    // Update: Pollyfill for firefox implemented!
    // https://developers.google.com/web/updates/2019/02/constructable-stylesheets
    set adoptedStyleSheets(styles) {
        if(!Array.isArray(styles)) {
            throw new Error(`> Component.adoptedStyleSheets: [Error] '${this.tagName}' >> styles isn't an array.`);
        }
        if(!!this.shadowRoot && __features__.ConstructableStylesheets) {
            this.shadowRoot.adoptedStyleSheets = styles.map(s => {
                if(typeof s === 'string' || s instanceof String) {
                    const sheet = new CSSStyleSheet();
                    if(s.indexOf('@import') > -1)
                        sheet.replace(s);
                    else
                        sheet.replaceSync(s);
                    return sheet;
                } else if(s instanceof HTMLStyleElement) {
                    const sheet = new CSSStyleSheet();
                    sheet.replaceSync(s.textContent);
                    return sheet;
                } else if (s instanceof CSSStyleSheet) {
                    return s;
                } else {
                    throw new Error(`> Component.adoptedStyleSheets: [Error] '${this.tagName}' >> style isn't a String, HTMLStyleElement or CSSStyleSheet.`);
                }
            });
        } else {
            styles = styles.map(s => {
                if(typeof s === 'string' || s instanceof String) {
                    const element = document.createElement('style');
                    element.textContent = s;
                    return element;
                } else if (s instanceof CSSStyleSheet) {
                    const rules = Array.from(s.cssRules).map(rule => {
                        return rule.cssText;
                    });
                    const element = document.createElement('style');
                    element.textContent = rules.join('\n');
                    return element;
                } else if (s instanceof HTMLStyleElement) {
                    return s;
                } else {
                    throw new Error(`> Component.adoptedStyleSheets: [Error] '${this.tagName}' >> style isn't a String, HTMLStyleElement or CSSStyleSheet.`);
                }
            });
            this.__legacyAdoptedStyleSheets = styles;
        }
        this.render();
    }
    get adoptedStyleSheets() {
        if(!!this.shadowRoot && __features__.ConstructableStylesheets) {
            return this.shadowRoot.adoptedStyleSheets;
        } else {
            return this.__legacyAdoptedStyleSheets;
        }
    }

    render(rescan = false) {
        if(this.isConnected) {
            let template = this.$template;
            if(template instanceof ComponentView) {
                template = template.render();
            } else {
                if(typeof template === 'string') {
                    template = html`${template}`;
                }
                if(typeof template === typeof(Function)) {
                    template = template.bind(this)();
                }
            }
            if(!(template instanceof TemplateResult)) {
                throw new Error(`> Component.render(): [Error] '${this.tagName}' >> Doesn't have a valid template.`);
            }
            render([this.__legacyAdoptedStyleSheets, template], this.shadowRoot || this, { eventContext: this });

            if(rescan || !!this.__$options.rescan) {
                this.__getElements();
            }
        }
    }

    connectedCallback() {
        this.dataset.iscomponent = !!this.dataset.iscomponent? this.dataset.iscomponent : this.__generateID();
        this.__getParent();
        this.render(true);
        this.__setStyles();
    }
    disconnectedCallback() {
        this.__$presenter = null;
        this.__$parent = null;
    }
    adoptedCallback() {
        this.__getParent();
        this.render(true);
    }
    attributeChangedCallback(attr, oldValue, newValue) {
        this.render();
    }

    $input(prop, render) {
        return (e) => {
            let $this = this;
            if(Array.isArray(prop)) {
                $this = prop[0];
                prop = prop[1];
            }
            if(e.currentTarget instanceof HTMLSelectElement) {
                if(e.currentTarget.multiple) {
                    $this[prop] = Array.from(e.currentTarget.selectedOptions).map(o => o.value);
                } else {
                    $this[prop] = e.currentTarget.value;
                }
            } else {
                $this[prop] = e.currentTarget.value;
            }
            if(!!render) {
                this.render();
            }
        };
    }
}

export class Presenter extends Component {
    constructor(shadow, options = {}) {
        super(shadow, options);
        this.dataset.ispresenter = "";
    }
}


// check compatibility features ----------------------------------------------------
const __features__ = {
    ConstructableStylesheets: true
};
function checkCompatibilityFeatures() {
    try {
        let tmp = new CSSStyleSheet();
    } catch(e) {
        __features__.ConstructableStylesheets = false;
    }
};
checkCompatibilityFeatures();