import { Entity } from './entity.js';
import { LocalDbStorage } from './localdbstorage.js';

export class Resource {
    static get HTTP_STATUS_OK() { return 200; }
    static get HTTP_STATUS_NO_CONTENT() { return 204; }
    static get HTTP_STATUS_BAD_REQUEST() { return 400; }
    static get HTTP_STATUS_UNAUTHORIZED() { return 401; }
    static get HTTP_STATUS_FORBIDDEN() { return 403; }
    static get HTTP_STATUS_NOT_FOUND() { return 404; }
    static get HTTP_STATUS_METHOD_NOT_ALLOWED() { return 405; }
    static get HTTP_STATUS_INTERNAL_SERVER_ERROR() { return 500; }
    static get HTTP_STATUS_NOT_IMPLEMENTED() { return 501; }
    static get HTTP_STATUS_INTERNAL_SERVER_ERROR() { return 500; }

    constructor({request, handlers, entityClass, localStorage} = {}) {
        this.__requestOptions = Object.assign({
            // default options ...
        }, request || {});
        this.__handlers = Object.assign({
            successGetById: ResourceFetch.responseEntityHandler(this),
            successGet: ResourceFetch.responseEntityListToLocalStorageHandler(this),
            successPost: null,
            successPut: null,
            successDelete: null,
            // example handler ***
            // <handlerName>: (response, handler) => {
            //     if (this.hasEntity && ResourceHandler.isContentType(response, "application/json")) {
            //         response.json().then(data => handler(this.entityClass.fromObject(data), response));
            //         return;
            //     }
            //     ResourceHandler.responseHandler(response, handler);  // handler by default
            // },
        }, handlers || {});

        this.__entityClass;
        if(!!entityClass) {
            if(entityClass.prototype instanceof Entity) {
                this.__entityClass = entityClass;
            } else {
                throw `> Resource: [Error] '${this.constructor.name}' >> entityClass isn't a inherited class of Entity`;
            }
        }

        this.__localStorage;
        if(!!localStorage) {
            if(localStorage instanceof LocalDbStorage) {
                this.__localStorage = localStorage;
            } else {
                throw `> Resource: [Error] '${this.constructor.name}' >> localStorage isn't a LocalDbStorage instance`;
            }
        }
    }

    static __serializeURLParams(obj, prefix) {
        if (obj != null && typeof obj == 'object') {
            var str = [],
                p;
            for (p in obj) {
                if (obj.hasOwnProperty(p)) {
                    var k = prefix ? prefix + "[" + p + "]" : p,
                        v = obj[p];
                    str.push((v !== null && typeof v === "object") ?
                        Resource.__serializeURLParams(v, k) :
                        encodeURIComponent(k) + "=" + encodeURIComponent(v));
                }
            }
            return '?' + str.join("&");
        }
        return undefined;
    }

    static __getRequest(url, method, options = {}, data = undefined) {
        // Documentation: https://developer.mozilla.org/en-US/docs/Web/API/Request/Request
        const controller = window.AbortController? new AbortController() : {};  // <-- experimental feature
        const requestOptions = Object.assign({}, options, {
            method,
            headers: Object.assign({}, options.headers || {}),       //– associated Headers object
            // body: options.body,
            mode: options.mode,                                //– [cors, no-cors, same-origin, navigate]. The default is cors.
            credentials: options.credentials || 'same-origin', //– [omit, same-origin, include]. The default is same-origin
            cache: options.cache,                              //– cache mode [default, reload, no-cache]
            redirect: options.redirect,                        //– [follow, error, manual]
            referrer: options.referrer,                        //– A USVString specifying no-referrer, client, or a URL. The default is about:client
            integrity: options.integrity || '',                //– subresource integrity value of the request (e.g., sha256-BpfBw7ivV8q2jLiT13fxDYAe2tJllusRSZ273h2nFSE=)
            signal: controller.signal
        });
        Resource.__prepareBody(data, requestOptions);
        const request = new Request(url, requestOptions);
        request.$data = requestOptions.$data;
        return request;
    }
    static __prepareBody(data, options) {
        if(data !== undefined) {
            options.headers = options.headers || {};
            options.$data = data;
            if (data instanceof FormData /*|| data instanceof BufferSource*/ || data instanceof ArrayBuffer || data instanceof ReadableStream || data instanceof URLSearchParams /*|| data instanceof USVString*/) {
                options.body = data;
            } else if (data instanceof Blob) {
                if(options.blobToFormData) {  //if(options.blobToFormData === undefined || options.blobToFormData == true) {
                    let form = new FormData();
                    form.append('data', data);
                    options.body = form;
                } else {
                    options.body = data;
                }
                options.$data = options.body;
            } else if (data != null && typeof data === 'object') {
                options.headers['Content-Type'] = 'application/json';
                options.body = JSON.stringify(data);
            }
        }
    }

    static $get(url, params, options = {}, {successHandler, errorHandler} = {}) {
        let query = Resource.__serializeURLParams(params);
        url += query !== undefined? `${query}` : '';

        let request = Resource.__getRequest(url, 'GET', options);
        return new Promise((resolve, reject) => {
            ResourceFetch.fetch(
                this,
                request,
                {resolve, reject},
                {successHandler, errorHandler}
            );
        });
    }

    static $post(url, data, params, options = {}, {successHandler, errorHandler} = {}) {
        let query = Resource.__serializeURLParams(params);
        if (params !== undefined)
            url += query !== undefined? query : `/${params}`;

        let request = Resource.__getRequest(url, 'POST', options, data);
        return new Promise((resolve, reject) => {
            ResourceFetch.fetch(
                this,
                request,
                {resolve, reject},
                {successHandler, errorHandler}
            );
        });
    }

    static $put(url, data, params, options = {}, {successHandler, errorHandler} = {}) {
        let query = Resource.__serializeURLParams(params);
        if (params !== undefined)
            url += query !== undefined? query : `/${params}`;

        let request = Resource.__getRequest(url, 'PUT', options, data);
        return new Promise((resolve, reject) => {
            ResourceFetch.fetch(
                this,
                request,
                {resolve, reject},
                {successHandler, errorHandler}
            );
        });
    }

    static $delete(url, params, options = {}, {successHandler, errorHandler} = {}) {
        let query = Resource.__serializeURLParams(params);
        if (params !== undefined)
            url += query !== undefined? query : `/${params}`;

        let request = Resource.__getRequest(url, 'DELETE', options);
        return new Promise((resolve, reject) => {
            ResourceFetch.fetch(
                this,
                request,
                {resolve, reject},
                {successHandler, errorHandler}
            );
        });
    }

    static onSuccess(data, response) {
    }
    static onError(data, response) {
    }

    get api() {
        return '/api/<resource>'
    }

    get hasEntity() {
        return !!this.__entityClass && this.__entityClass.prototype instanceof Entity;
    }
    get entityClass() {
        return this.__entityClass;
    }

    get requestOptions() {
        return this.__requestOptions;
    }
    get handlers() {
        return this.__handlers;
    }

    get hasLocalStorage() {
        return !!this.__localStorage && this.__localStorage instanceof LocalDbStorage;
    }
    get localStorage() {
        return this.__localStorage;
    }

    getById(id) {
        return this.constructor.$get(`${this.api}/${id}`, undefined, this.__requestOptions, { successHandler: this.__handlers.successGetById });
    }
    get(params) {
        return this.constructor.$get(this.api, params, this.__requestOptions, { successHandler: this.__handlers.successGet });
    }
    post(data, params) {
        return this.constructor.$post(this.api, data, params, this.__requestOptions, { successHandler: this.__handlers.successPost });
    }
    put(data, params) {
        return this.constructor.$put(this.api, data, params, this.__requestOptions, { successHandler: this.__handlers.successPut });
    }
    delete(params) {
        return this.constructor.$delete(this.api, params, this.__requestOptions, { successHandler: this.__handlers.successDelete });
    }
}

export class ResourceFetch {
    static get RESPONSE_AUTO() { return 'RESPONSE_AUTO'; }
    static get RESPONSE_ARRAY_BUFFER() { return 'RESPONSE_ARRAY_BUFFER'; }
    static get RESPONSE_BLOB() { return 'RESPONSE_BLOB'; }
    static get RESPONSE_JSON() { return 'RESPONSE_JSON'; }
    static get RESPONSE_TEXT() { return 'RESPONSE_TEXT'; }
    static get RESPONSE_FORMDATA() { return 'RESPONSE_FORMDATA'; }

    static fetch(resourceClass, request, {resolve, reject}, {successHandler, errorHandler}) {
        const success = (data, response) => {
            console.log(`> Resource.${request.method}: [OK] "${request.url}" -> Data >>`, data);
            if(!!resolve && typeof(resolve) === typeof(Function)){
                resolve({data, response});
            }
            resourceClass.onSuccess({data, response});
        }
        const error = (data, response) => {
            console.error(`> Resource.${request.method}: [Error] "${request.url}" -> Data >>`, data);
            if(data instanceof TypeError) {
                data = data.message;
            }
            if(!!reject && typeof(reject) === typeof(Function)) {
                reject({data, response});
            }
            resourceClass.onError({data, response});
        }

        console.log(`> Resource.${request.method}: [Sending] "${request.url}" >>`, request.$data || ''); //request.body || '');
        fetch(request).then(response => {
            if (response.ok) {  // response was successful (status in the range 200-299)
                console.log(`> Resource.${request.method}: [OK] "${request.url}" -> Response >>`, response);
                if(!!successHandler && typeof(successHandler) === typeof(Function)) {
                    successHandler(response, success);
                    return;
                }
                ResourceFetch.responseHandler(response, success);  // handler by default
            } else {
                console.error(`> Resource.${request.method}: [Error] "${request.url}" -> Response >>`, response);
                if(!!errorHandler && typeof(errorHandler) === typeof(Function)) {
                    errorHandler(response, error);
                    return;
                }
                ResourceFetch.responseHandler(response, error);  // handler by default
            }
        }).catch(e => error(e, null));
    }

    static responseHandler(response, handler, responseType = ResourceFetch.RESPONSE_AUTO) {
        if(responseType == this.RESPONSE_AUTO) {
            const contentType = response.headers.get("content-type");
            if (contentType && contentType.indexOf("application/json") !== -1) {
                response.json().then(data => handler(data, response));
            } else if (contentType && contentType.indexOf("application/octec-stream") !== -1) {
                response.blob().then(blob => handler(blob, response))
            } else {
                response.text().then(text => handler(text, response));
            }
        } else if(responseType == this.RESPONSE_ARRAY_BUFFER) {
            response.arrayBuffer().then(buf => handler(buf, response));
        } else if(responseType == this.RESPONSE_BLOB) {
            response.blob().then(blob => handler(blob, response));
        } else if(responseType == this.RESPONSE_JSON) {
            response.json().then(json => handler(json, response));
        } else if(responseType == this.RESPONSE_TEXT) {
            response.text().then(text => handler(text, response));
        } else if(responseType == this.RESPONSE_FORMDATA) {
            response.formData().then(data => handler(data, response));
        } else {
            throw `> ResourceFetch: [Error] >> responseType param isn't valid : ${responseType}`;
        }
    }

    static responseEntityHandler(resource) {
        return function(response, handler) {
            if (this.hasEntity && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => handler(this.entityClass.fromObject(data), response));
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        }.bind(resource);
    }
    static responseEntityListHandler(resource) {
        return function(response, handler) {
            if (this.hasEntity && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => handler(this.entityClass.fromList(data), response));
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        }.bind(resource);
    }
    static responseEntityListToLocalStorageHandler(resource) {
        return function (response, handler) {
            if (this.hasEntity && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => {
                    data = this.entityClass.fromList(data);
                    if(this.hasLocalStorage) {
                        this.localStorage.sync(data).then(() => handler(data, response)).catch(e => { throw e });
                        return;
                    }
                    handler(data, response);
                });
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        }.bind(resource);
    }

    static responseCustomEntityHandler(entityClass) {
        return (response, handler) => {
            if (ResourceFetch.isEntityClass(entityClass) && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => handler(entityClass.fromObject(data), response));
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        };
    }
    static responseCustomEntityListHandler(entityClass) {
        return (response, handler) => {
            if (ResourceFetch.isEntityClass(entityClass) && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => handler(entityClass.fromList(data), response));
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        };
    }
    static responseCustomEntityListToLocalStorageHandler(localStorage) {
        return (response, handler) => {
            if (ResourceFetch.isLocalStorage(localStorage) && ResourceFetch.isContentType(response, "application/json")) {
                response.json().then(data => {
                    data = localStorage.entityClass.fromList(data);
                    localStorage.sync(data).then(() => handler(data, response)).catch(e => { throw e });
                });
                return;
            }
            ResourceFetch.responseHandler(response, handler);
        }
    }

    static isLocalStorage(localStorage) {
        return !!localStorage && localStorage instanceof LocalDbStorage;
    }
    static isEntityClass(entityClass) {
        return !!entityClass && entityClass.prototype instanceof Entity;
    }
    static isContentType(response, type) {
        if(response instanceof Response) {
            const contentType = response.headers.get("content-type");
            return contentType && contentType.indexOf(type) !== -1;
        }
        throw `> ResourceFetch: [Error] >> response param isn't a valid instance of Response`;
    }
}