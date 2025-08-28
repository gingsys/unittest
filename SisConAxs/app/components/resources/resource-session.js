
import { ResourceBase } from './resource-base.js';
//import { User } from './resource-user.js';
export { Session } from './entity-session.js';

export class SessionResource extends ResourceBase {

    verifyLogin() {
        return SessionResource.$get('/api/Home', undefined, this.__requestOptions);
    }

    login(data) {
        // let $this = this;
        return new Promise(function(resolve, reject) {
            // if(!!$this.auth.sessionToken) {
            //     resolve(this.auth);
            // } else {
                SessionResource.$post('/api/Account', data)
                .then(r => {
                    window.localStorage.setItem('auth', JSON.stringify(r));
                    resolve(r);
                },
                err => {
                    if (!!reject)
                        reject(err);
                });
            // }
        });
    }

    logout() {
        this.removeAuthData();
        return SessionResource.$get('/api/Home/logout');
    }

    removeAuthData() {
        window.localStorage.removeItem('auth');
    }

    changeCompany(companyID) {
        const $this = this;
        return new Promise(function(resolve, reject) {
            // if(!!$this.auth.sessionToken) {
            //     resolve(this.auth);
            // } else {
                SessionResource.$post('/api/Account/changecompany', null, companyID, $this.__requestOptions)
                .then(r => {
                    window.localStorage.setItem('auth', JSON.stringify(r));
                    resolve(r);
                },
                err => {
                    if (!!reject)
                        reject(err);
                });
            // }
        });
    }
}