import { Router, Presenter, DOM } from './core/core.js'
import { SessionResource } from './components/resources/resource-session.js';

import { WorkflowComponent } from './components/ui-workflow.js';
import { ResourceComponent } from './components/ui-resource.js';
import { RequestComponent } from './components/ui-request.js';
import { RequestMassiveDeactivationComponent } from './components/ui-request-massivedeactivation.js';

import { ApplicationConfig } from './config/application.js';
import { ApplicationView } from './app.view.js';
import './config/components.js';
import { msalConfig } from './components/authConfig.js';

class AppPresenter extends Presenter {
    
    constructor() {
        super(false);
        this.$resourceLogin = new SessionResource();
        this.__listCompanies = [];

        this.addComponent('workflow', new WorkflowComponent(this));
        this.addComponent('resource', new ResourceComponent(this));
        this.addComponent('request', new RequestComponent(this));
        this.addComponent('request-massivedeactivation', new RequestMassiveDeactivationComponent(this));
    }

    get $template() {
        return new ApplicationView(this);
    }

    get applicationName() {
        return ApplicationConfig.ApplicationName;
    }
    get applicationFullname() {
        return ApplicationConfig.ApplicationFullname;
    }
    get applicationVersion() {
        return ApplicationConfig.ApplicationVersion;
    }
    get companyName() {
        return !!SessionResource.session && !!SessionResource.session.CompanyDisplay? SessionResource.session.CompanyDisplay : '[Ninguno]';
    }
    get userFullname() {
        return !!SessionResource.session && !!SessionResource.session.sessionUserFullName? SessionResource.session.sessionUserFullName : '';
    }
    
    get logged() {
        return !!SessionResource.session.sessionUser;
    }

    connectedCallback() {
        super.connectedCallback();

        this.showLoading();
        const $this = this;
        // iziToast global settings ---------------------------------------------- //
        iziToast.settings({
            timeout: 3000,
            zindex: 9999
        });

        // sidebar --------------------------------------------------------------- //
        $("#sidebar").mCustomScrollbar({
            theme: "minimal"
        });
        $('#sidebarCollapse').on('click', () => {
            // open sidebar
            $('#sidebar').addClass('active');
            // fade in the overlay
            $('.overlay').addClass('active');
            $('.collapse.in').toggleClass('in');
            $('a[aria-expanded=true]').attr('aria-expanded', 'false');
            // this.$$.sidebarCollapse.popover('dispose');
        });
        $('#dismiss, .overlay').on('click', () => {
            $('#sidebar').removeClass('active');
            // hide overlay
            $('.overlay').removeClass('active');
        });

        // Events ---------------------------------------------------------------- //
        // online, offline
        window.addEventListener('offline', this.handleConnectionChange.bind(this));
        window.addEventListener('online', this.handleConnectionChange.bind(this));

        // Actions ------------------------------------------------------------- //
        this.hideLoading();
        this.render();
        // this.$$.sidebarCollapse.popover({
        //     delay: { show: 500, hide: 2000 }
        // });
        //this.$$.sidebarCollapse.popover('show');
    }

    render(rescan) {
        super.render(rescan);
        this.$.menu.render();
    }

    handleConnectionChange(event) {
        if (event.type == "offline") {
            this.$.iconOnline.classList.remove('online');
            this.$.iconOnline.title = 'Desconectado';
        }
        if (event.type == "online") {
            this.$.iconOnline.classList.add('online');
            this.$.iconOnline.title = 'Conectado';
        }
    }

    async verifyLogin() {
        let response = false;
        this.showLoading();
        await this.$resourceLogin.verifyLogin().then(
            r => {
                response = true;
                this.getCompanies();
                this.hideLoading();
            }, err => {
                this.hideLoading();
                this.$resourceLogin.removeAuthData();
            });
        return response;
    }

    getCompanies() {
        const session = SessionResource.session;
        this.__listCompanies = [];
        if (session.User != null) {
            this.__listCompanies = session.User.UserCompanies
                .filter(c => c.CompanyID != session.CompanyID && c.CompanyActive > 0)
                .sort((a, b) => a.CompanyDisplay.localeCompare(b.CompanyDisplay));
        }
    }
    changeCompany(companyID) {
        this.question(
            'Cambiar empresa',
            'Desea cambiar de empresa?',
            (instance, toast) => {
                this.showLoading();
                this.$resourceLogin.changeCompany(companyID)
                    .then(r => {
                        Router.redirect("/login");
                        this.hideLoading();
                    })
                    .catch(err => {
                        this.hideLoading();
                        iziToast.error({ title: err });
                    });
            }
        );
    }

    showLoading() {
        this.$$.dvLoading.show();
    }
    hideLoading() {
        this.$$.dvLoading.hide();
    }

    question(title, message, fnOk) {
        iziToast.question({
            timeout: 20000,
            close: false,
            overlay: true,
            displayMode: 'once',
            title: title,
            message: message,
            position: 'center',
            buttons: [
                ['<button><b>Si</b></button>', function(instance, toast) {
                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                    fnOk(instance, toast);
                }, true],
                ['<button>No</button>', function(instance, toast) {
                    instance.hide({ transitionOut: 'fadeOut' }, toast, 'button');
                }],
            ]
        });
    }

    showChangelog() {
        const win = window.open('/Help/ChangeLog.html', '_blank');
        win.focus();
    }
    showHelp() {
        const win = window.open('/media/user-guide.html', '_blank');
        win.focus();
    }

    logout() {
        this.$resourceLogin.logout();
        Router.redirect('/login');
    }
    signOut() {       
        this.$myMSALObj = new Msal.UserAgentApplication(msalConfig);
        this.$myMSALObj.logout(); // borra la sesión del AZURE AD
        this.logout();            // borra la sesión de la aplicación 
    }


    renderContent(component) { // versión antigua, por eliminar
        $('.modal').modal('hide');
        this.$presenter.hideLoading();

        return async(params = {}) => {
            if (!await this.verifyLogin()) {
                Router.redirect('/login');
                return;
            }
            if(component instanceof HTMLElement) {
                DOM.replaceContent(this.$.content, component);
            } else if(/\<.*?\>.*/g.test(component)) {
                this.$.content.innerHTML = component;
            } else {
                this.$component[component].params = params || {};
                this.$component[component].attach(this.$.content);
            }
            this.render();
        }
    }
    async setContent(component) {  // versión nueva
        $('.modal').modal('hide');
        this.$presenter.hideLoading();

        if (!await this.verifyLogin()) {
            Router.redirect('/login');
            return;
        }
        DOM.replaceContent(this.$.content, component);
        this.render();
    }
}
customElements.define('app-presenter', AppPresenter);