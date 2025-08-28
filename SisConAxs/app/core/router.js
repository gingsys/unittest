export class Router {
    static get ERROR_404() { return 404; }
    static get ERROR_VALIDATE() { return "ERROR_VALIDATE"; }

    constructor(routes, scope) {
        this.__routes = routes;
        this.__scope = scope;
        this.__currentState = { path: null, parameters: [], search: [] };
    }

    static error404() {
        console.error('> Router: [Error] >> Default error 404 (Not found).');
    }

    static redirect(path) {
        if (location.hash != `#${path}`) {
            location.hash = `#${path}`;
            return true;
        }
        return false;
    }

    static get isMobile() {
        return navigator.userAgent.match(/Android/i)
                || navigator.userAgent.match(/webOS/i)
                || navigator.userAgent.match(/iPhone/i)
                || navigator.userAgent.match(/iPad/i)
                || navigator.userAgent.match(/iPod/i)
                || navigator.userAgent.match(/BlackBerry/i)
                || navigator.userAgent.match(/Windows Phone/i)
                || navigator.userAgent.match(/IEMobile/i);
    }

    __recursiveRoute(currentState, routes, scope, request, prefix = '') {
        for (const path in routes) {
            if (path.startsWith("/")) {
                const haveChildren = Object.keys(routes[path]).some(r => r.startsWith("/"));
                const currentPath = `${prefix}${path}`;
                
                const regexPathParams = `${prefix}${path.replace(/\:([a-z]+)/ig, `(?<$1>[^?]+)`)}`;
                const regexChildren = haveChildren? `[a-z0-9\/\-]*` : ``;
                const regexQueryParams = `(?<__query__>\\?[\\w-]+=[\\w-]+(?:&[\\w-]+=[\\w-]+)?)?`;
                const regex = new RequestRegExp(`^${regexPathParams}${regexChildren}${regexQueryParams}$`, 'ig');
                const matches = regex.exec(request);

                if (matches !== null) {
                    console.log(`> Router.route: [Search] Rule -> "${currentPath}" >> Request -> "${request}"`, matches.groups);
                    
                    let handler;
                    if(!!routes[path] && typeof routes[path] == typeof(Function)) {
                        handler = routes[path];
                    } else if(!!routes[path]['@controller'] && typeof routes[path]['@controller'] == typeof(Function)) {
                        handler = routes[path]['@controller'];
                    }

                    if(handler !== undefined) {
                        if(!!scope) {
                            handler = handler.bind(scope);
                        }
                        // if currentPath is different to currentState: path + parameters, execute handler
                        const parameters = matches.groups;
                        const query = parameters.__query__ !== undefined? Object.fromEntries(new URLSearchParams(parameters.__query__)) : [];

                        if(currentState.path !== currentPath || (currentState.path == currentPath && JSON.stringify(currentState.parameters) !== JSON.stringify(matches.groups))) {
                            currentState.path = currentPath,
                            currentState.parameters = parameters;
                            currentState.query = query;
                            handler(currentState);
                            console.log(`> Router.route: [Found] Request -> "${request}"`, currentState);
                            return true;
                        }
                    }
                    
                    if(haveChildren) {
                        if(request !== path) {
                            return this.__recursiveRoute(currentState, routes[path], scope, request, currentPath);
                        }
                    } else {
                        throw new Error(`> Router.route: [Error] >> Route "${prefix}${path}" haven't a controller.`);
                    }

                    return true;
                }
            }
        }
    }

    __route() {
        // Get the parsed URl from the addressbar
        const request = location.hash.slice(1).toLowerCase() || '/';
        if(this.__recursiveRoute(this.__currentState, this.__routes, this.__scope, request)) {
            //
        } else {
            if (!!this.__routes[Router.ERROR_404]) {
                if(typeof this.__routes[Router.ERROR_404] == typeof(Function)) {
                    console.warn(`> Router.route: [NotFound] Rule -> "${request}"`);

                    if(!!this.__scope)
                        this.__routes[Router.ERROR_404].apply(this.__scope);
                    else
                        this.__routes[Router.ERROR_404]();
                }
                else
                    throw new Error(`> Router: [Error] >> Route '${Router.ERROR_404}' haven't a function.`);
            } else {
                Router.error404();
            }
        }
    }

    initialize() {
        // Listen on hash change + page load:
        window.addEventListener('hashchange', this.__route.bind(this));
        window.addEventListener('load', this.__route.bind(this));
    }
}

class RequestRegExp extends RegExp {
    [Symbol.matchAll](str) {
        let result = RegExp.prototype[Symbol.matchAll].call(this, str);
        if (!result) {
            return null;
        }
        return Array.from(result);
    }
}