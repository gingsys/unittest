import { Resource as ResourceV2 } from "./resource.js";
export { ResourceFetch } from "./resource.js";

export class Resource extends ResourceV2 {
    static $get(url, params, options = {}, {successHandler, errorHandler} = {}) {
        const request = super.$get(url, params, options, {successHandler, errorHandler});
        return new Promise((resolve, reject) => {
            request.then(
                ({data}) => resolve(data),
                ({data}) => reject(data)
            );
        });
    }

    static $post(url, data, params, options = {}, {successHandler, errorHandler} = {}) {
        const request = super.$post(url, data, params, options, {successHandler, errorHandler});
        return new Promise((resolve, reject) => {
            request.then(
                ({data}) => resolve(data),
                ({data}) => reject(data)
            );
        });
    }

    static $put(url, data, params, options = {}, {successHandler, errorHandler} = {}) {
        const request = super.$put(url, data, params, options, {successHandler, errorHandler});
        return new Promise((resolve, reject) => {
            request.then(
                ({data}) => resolve(data),
                ({data}) => reject(data)
            );
        });
    }

    static $delete(url, params, options = {}, {successHandler, errorHandler} = {}) {
        const request = super.$delete(url, params, options, {successHandler, errorHandler});
        return new Promise((resolve, reject) => {
            request.then(
                ({data}) => resolve(data),
                ({data}) => reject(data)
            );
        });
    }
}