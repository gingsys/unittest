import { Component, Router, html } from '../../core/core.js';
import { loginRequest, msalConfig, tokenRequest } from '../authConfig.js';
import { SessionResource } from '../resources/resource-session.js';
import { LoginView } from './ui-login.view.js';

export class LoginComponent extends Component {
    constructor() {
        super(false);
        this.$resourceSession = new SessionResource();
    }

    get $template() {
        return new LoginView(this);
    }

    connectedCallback() {
        super.connectedCallback();
    }

    login(e) {
        e.preventDefault();
        e.stopPropagation();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: "No ha ingresado los datos necesarios" });
            return;
        };

        this.$presenter.showLoading();
        this.$resourceSession.login({
            'grant_type': 'local',
            'username': this.$.txtUser.value.trim(),
            'credentials': '', //this.$.txtPassword.value.trim(),
            // 'company': this.$.cboCompany.value
        }).then(r => {
            this.$presenter.hideLoading();
            iziToast.success({ title: 'Login correcto' });
            Router.redirect('/');
        }, err => {
            this.$presenter.hideLoading();
            iziToast.error({ title: 'Usuario o contraseña inválida' });
        });
    }

    signIn() {
        const msalInstance = new Msal.UserAgentApplication(msalConfig);
        const response = msalInstance.loginPopup(loginRequest)
        response.then(loginResponse => {
            if (msalInstance.getAccount()) {
                const currentAccounts = msalInstance.getAccount();
                localStorage.setItem("userName", currentAccounts.userName);
                localStorage.setItem("name", currentAccounts.name);

                const accountId = msalInstance.getAccount().accountIdentifier;
                const rawIdToken = loginResponse.idToken.rawIdToken;
                this.getExtraInfoUser(accountId, rawIdToken)
                    .then(function (response) {
                        this.render(true);
                        this.$presenter.hideLoading();
                    })
                    .catch(
                        error => console.log('error', error)
                    );
            }
        }).catch(error => {
            console.log(error);
        });
    }

    async getExtraInfoUser(accountId, rawIdToken) {
       /* debugger;*/
        this.$presenter.showLoading()
        this.getTokenPopup(loginRequest)
            .then(r => {
                window.localStorage.setItem('Token', r.accessToken);
                var url = "https://graph.microsoft.com/v1.0/users/{" + accountId + "}?$select=displayName,companyName,postalCode,givenName,surname,jobTitle";
                this.seacrhEmpresaDocumento(r.accessToken, url)
                    .then(data => {
                        this.$presenter.hideLoading();

                       /* debugger;*/
                        localStorage.setItem("EmpresaAD", data.companyName),
                        localStorage.setItem("NumeroDocumento", data.postalCode),
                        localStorage.setItem("GivenName", data.givenName),
                        localStorage.setItem("SurName", data.surname),
                        localStorage.setItem("JobTitle", data.jobTitle) // AGREGADO CARGO

                        this.goToController(rawIdToken);                        
                    }).catch(err => {
                        this.$presenter.hideLoading();
                        iziToast.error({ title: err.data, position: 'center' });
                    });

                this.render(true);
            }).catch(err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err.data, position: 'center' });
            });
    }

    async getTokenPopup(request) {
        const $myMSALObj = new Msal.UserAgentApplication(msalConfig);
        return $myMSALObj.acquireTokenSilent(request)
        .catch(error => {
            console.log(error);
            console.log("silent token acquisition fails. acquiring token using popup");

            // fallback to interaction when silent call fails
            return myMSALObj.acquireTokenPopup(request)
                .then(tokenResponse => {
                    return tokenResponse;
                }).catch(error => {
                    console.log(error);
                });
        });
}

    async seacrhEmpresaDocumento(token, endpoint) {
        const myHeaders = new Headers();
        myHeaders.append("Authorization", "Bearer " + token);
        myHeaders.append("Cookie", "ARRAffinity=0a3517ba6ed8bb14ffe517099672a3eb4ea3c4b710ad8c6e0edaa70c2d244335; ARRAffinitySameSite=0a3517ba6ed8bb14ffe517099672a3eb4ea3c4b710ad8c6e0edaa70c2d244335");

        const requestOptions = {
            method: 'GET',
            headers: myHeaders,
            redirect: 'follow'
        };

        return fetch(endpoint, requestOptions)
            .then(function (response) {
                return response.json();
            })
            .catch(
                error => console.log('error', error)
            );
    }

    async goToController(rawIdToken) {
        this.$presenter.showLoading();
        this.$resourceSession.login({
            'grant_type': 'azureAD',
            'username': localStorage.getItem('userName'),
            'credentials': rawIdToken,
            'name': localStorage.getItem('name'),
            'empresa': localStorage.getItem('EmpresaAD'),
            'numeroDocumento': localStorage.getItem('NumeroDocumento'),
            'givenName': localStorage.getItem('GivenName'),
            'surname': localStorage.getItem('SurName'),
            'jobTitle': localStorage.getItem('JobTitle')
        }).then(r => {
            this.$presenter.hideLoading();
            iziToast.success({ title: 'Login correcto' });
            Router.redirect('/');
        }, err => {
            //this.signOut(); // se llama al método signOut de Azure PARA CERRAR la sesión , en caso no tenga permiso en la app
            this.$presenter.hideLoading();
            iziToast.error({ title: err });
        });
    }

    signOut() {
        const msalInstance = new Msal.UserAgentApplication(msalConfig);
        msalInstance.logout();
    }
}
customElements.define('application-login', LoginComponent);