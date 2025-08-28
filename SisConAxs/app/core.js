import { render } from '../node_modules/lit-html/lit-html.js'; // 'https://unpkg.com/lit-html?module'
export { html }
from '../node_modules/lit-html/lit-html.js'; // 'https://unpkg.com/lit-html?module'



export class Router {
    static get ERROR_404() {
        return 404;
    }


    static init(presenter, routes) {
        const router = async() => {
            // Get the parsed URl from the addressbar
            let request = location.hash.slice(1).toLowerCase() || '/';

            class RequestRegExp extends RegExp {
                [Symbol.matchAll](str) {
                    let result = RegExp.prototype[Symbol.matchAll].call(this, str);
                    if (!result) {
                        return null;
                    }
                    return Array.from(result);
                }
            }

            for (let r in routes) {
                let regex = new RequestRegExp('^' + r.replace(/({[^?}]+})/, '([^?]+)') + '$', 'ig');
                let matches = regex.exec(request);
                if (matches !== null) {
                    let args = Array.from(matches);
                    args.push(args.shift());
                    console.log(`> Router: "${request}" >> `, args);
                    routes[r].apply(presenter, args);
                    return;
                }
            }
            if (routes[Router.ERROR_404]) {
                routes[Router.ERROR_404].apply(presenter);
            } else {
                Router.error404.apply(presenter);
            }
        }

        // Listen on hash change:
        window.addEventListener('hashchange', router);
        // Listen on page load:
        window.addEventListener('load', router);
    }

    static error404() {
        console.error('not found');
    }
}


/* PRESENTER ---------------------------------------------------------------- */
class Selector {
    constructor() {
        this.__container = null;
        this.$ = {};
        this.$$ = {};
        this.$component = {};
    }

    $one(selector) {
        return this.__container.querySelector(selector);
    }
    $all(selector) {
        return this.__container.querySelectorAll(selector);
    }

    __readyComponents() {
        for(let name in this.$component) {
            this.__container.querySelectorAll(`[data-component="${name}"]`).forEach(el => {
                this.$component[name].attach(el);
            });
        };
    }
}

export class Presenter extends Selector {
    static get id() {
        return '<id>';
    }

    constructor() {
        super();
        this.$component = {};
        this.__container = document;
        document.addEventListener('DOMContentLoaded', () => {
            document.querySelector(`[data-presenter="${this.constructor.id}"]`).querySelectorAll('[id]:not([id=""])').forEach(x => {
                this.$[x.id] = x;
                if (!!jQuery)
                    this.$$[x.id] = $(x);
            });
            this.__readyComponents();
            this.ready();
        });
    }

    ready() {
        console.log('> Presenter: ready!', this);
    };

    addComponent(name, component) {
        component.$presenter = this;
        this.$component[name] = component;
    }
    deleteComponent(name) {
        this.$component[name].remove();
        delete this.$component[name];
    }

    redirect(path) {
        if (location.hash != `#${path}`) {
            location.hash = `#${path}`;
            return true;
        }
        return false;
    }
}

export class Component extends Selector {
    constructor(presenter) {
        super();
        this.$component = {};
        //this.$presenter = presenter;
        this.$parent = null;
    }

    get $presenter() {
        return document.querySelector('[data-ispresenter]');
    }

    attach(wrapper) {
        this.__wrapper = wrapper;
        wrapper.$component = this;
        wrapper.innerHTML = '';

        this.__container = document.createElement("section");
        wrapper.appendChild(this.__container);

        this.render();
        this.$ = {};
        this.$$ = {};
        this.__container.querySelectorAll('[id]:not([id=""])').forEach(x => {
            if (!this.$[x.id]) {
                this.$[x.id] = x;
                if (!!jQuery)
                    this.$$[x.id] = $(x);
            }
        });
        this.__readyComponents();
        this.__ready();
    }
    remove() {
        this.__disconnect();
        this.__wrapper.$component = null;
        this.__wrapper.innerHTML = '';
        this.__container = null;
    }

    __ready() {

    }
    __disconnect() {

    }

    render() {
        render(this.__render(), this.__container, { eventContext: this });
    }

    addComponent(name, component) {
        this.$component[name] = component;
        component.$parent = this;
    }
    deleteComponent(name) {
        this.$component[name].remove();
        delete this.$component[name];
    }
}

// export function html(strings) {
//     return strings;
// }


/* RESOURCE ---------------------------------------------------------------- */
// class ResourceError extends Error {
//     constructor(...args) {
//         super(...args)
//         // Error.captureStackTrace(this, ResourceError)
//     }
// }

export class Resource {
    static get HTTP_STATUS_OK() { return 200; }
    static get HTTP_STATUS_NO_CONTENT() { return 204; }
    static get HTTP_STATUS_BAD_REQUEST() { return 400; }
    static get HTTP_STATUS_UNAUTHORIZED() { return 401; }
    static get HTTP_STATUS_FORBIDDEN() { return 403; }
    static get HTTP_STATUS_NOT_FOUND() { return 404; }

    constructor(options) {
        this.$options = options || {};
    }

    static __$serializeURLParams(obj, prefix) {
        if (obj != null && typeof obj == 'object') {
            var str = [],
                p;
            for (p in obj) {
                if (obj.hasOwnProperty(p)) {
                    var k = prefix ? prefix + "[" + p + "]" : p,
                        v = obj[p];
                    str.push((v !== null && typeof v === "object") ?
                        Resource.__$serializeURLParams(v, k) :
                        encodeURIComponent(k) + "=" + encodeURIComponent(v));
                }
            }
            return '?' + str.join("&");
        }
        return undefined;
    }

    static onError(data, response) {
        console.error('Resource default error');
    }

    static $get(url, params, options = {}) {
        let query = Resource.__$serializeURLParams(params);
        if (params !== undefined)
            url += !!query ? query : `/${params}`;

        return new Promise(function(resolve, reject) {
            fetch(url, {
                method: 'GET',
                credentials: 'same-origin',
                headers: options.headers || {}
            }).then(response => {
                if (response.status >= 200 && response.status < 300) {
                    const contentType = response.headers.get("content-type");
                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        response.json().then(data => {
                            console.log(`> Resource.$get: [OK] "${url}" >> `, data);
                            resolve(data, response); //{ data: data, response, response }
                        });
                    } else if (contentType && contentType.indexOf("application/octec-stream") !== -1) {
                        response.blob().then(blob => {
                            console.log(`> Resource.$get: [OK] "${url}" >> `, blob);
                            resolve(blob, response);
                        })
                    } else {
                        resolve();
                    }
                } else {
                    response.text().then(data => {
                        console.error(`> Resource.$get: [Error] "${url}" >>`, data);
                        if (!!reject) reject(data, response);
                        Resource.onError(data, response);
                        // throw new ResourceError(data, response);
                    });
                }
            }).catch(e => {
                console.warn(`> Resource.$get: [Error] "${url}" >>`, e);
                if (!!reject) reject(e);
                Resource.onError(data, response);
            });
        });
    }

    static $post(url, data, params, options = {}) {
        let $this = this;
        let query = Resource.__$serializeURLParams(params);
        if (params !== undefined)
            url += !!query ? query : `/${params}`;

        let _options = {
            method: 'POST',
            credentials: 'same-origin',
            headers: options.headers || {}
        };

        if (data instanceof FormData) {
            _options.body = data;
        } else if (data instanceof Blob) {
            let form = new FormData();
            form.append('data', data);
            _options.body = form;
        } else {
            _options.body = JSON.stringify(data);
            _options.headers['Content-Type'] = 'application/json';
        }

        return new Promise(function(resolve, reject) {
            fetch(url, _options).then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log(`> Resource.$post: [OK] "${url}"`);
                    const contentType = response.headers.get("content-type");
                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        response.json().then(data => {
                            if (!!resolve) resolve(data, response);
                        });
                    } else {
                        if (!!resolve) resolve(null, response);
                    }
                } else {
                    response.text().then(data => {
                        console.error(`> Resource.$post: [Error] "${url}" >>`, data);
                        if (!!reject) reject(data, response);
                        Resource.onError(data, response);
                    });
                }
            }).catch(e => {
                console.error(`> Resource.$post: [Error] "${url}" >>`, e);
                if (!!reject) reject(e);
                Resource.onError(data, response);
            });
        });
    }

    static $delete(url, params, options = {}) {
        let query = Resource.__$serializeURLParams(params);
        if (params !== undefined)
            url += !!query ? query : `/${params}`;

        return new Promise(function(resolve, reject) {
            fetch(url, {
                method: 'DELETE',
                credentials: 'same-origin',
                headers: options.headers || {}
            }).then(response => {
                if (response.status >= 200 && response.status < 300) {
                    console.log(`> Resource.$delete: [OK] "${url}"`);
                    if (!!resolve) resolve(null, response);
                } else {
                    response.text().then(data => {
                        console.error(`> Resource.$delete: [Error] "${url}" >>`, data);
                        if (!!reject) reject(data, response);
                        Resource.onError(data, response);
                    });
                }
            }).catch(e => {
                console.error(`> Resource.$delete: [Error] "${url}" >>`, e);
                if (!!reject) reject(e);
                Resource.onError(data, response);
            });
        });
    }

    get api() {
        return '/api/v1/<resource>';
    }

    get(params) {
        return Resource.$get.apply(this, [this.api, params, this.$options]);
    }

    post(data, params) {
        return Resource.$post.apply(this, [this.api, data, params, this.$options]);
    }

    delete(params) {
        return Resource.$delete.apply(this, [this.api, params, this.$options]);
    }
}


/* ENTITY ---------------------------------------------------------------- */
export class Entity {
    static fromList(list) {
        return list.map(x => {
            let item = new this();
            item.fromDTO(x);
            return item;
        });
    }
    static fromObject(obj) {
        let item = new this();
        item.fromDTO(obj);
        return item;
    }

    static __copy(source, dest) {
        for (let attr in dest) {
            let value = source[attr];
            if (value !== undefined && typeof dest[attr] !== 'function') {
                // if (Array.isArray(dest[attr]) && Array.isArray(source[attr])) {
                //     dest[attr] = [];
                //     source[attr].forEach(x => {
                //         if(x instanceof Entity) {

                //         }
                //     });
                //     // forEach((el, index) => __copy(source[attr], dest[index]));
                // } else
                if (dest[attr] instanceof Entity) {
                    dest[attr].fromDTO(source[attr]);
                } else {
                    dest[attr] = value;
                }
            }
        }
    }

    fromDTO(dto) {
        Entity.__copy(dto, this);
    }
    toDTO() {
        return JSON.parse(JSON.stringify(this));
    }

    copy() {
        let entity = new this.prototype.constructor();
        entity.fromDTO(this.toDTO());
        return entity;
    }
}

