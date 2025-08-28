import { DOM, Router } from "../core/core.js";

export const RoutesConfig = {
    '/login': async function() {
        $('.modal').modal('hide');
        this.$presenter.hideLoading();
        
        if (await this.verifyLogin()) {
            Router.redirect('/');
            return;
        }
        const component = document.createElement('application-login');
        DOM.replaceContent(this.$.content, component);
        this.render();
        
    },
    '/': async function() {
        const component = document.createElement('application-home');
        this.setContent(component);
    },
    
    // system -------------------------------------------------- //
    '/company': async function() {
        const component = document.createElement('company-main');
        this.setContent(component);
    },
    '/calendar': async function() {
        const component = document.createElement('config-calendar');
        this.setContent(component);
    },
    '/notification': async function() {
        const component = document.createElement('config-notification');
        this.setContent(component);
    },

    // configuration ------------------------------------------- //
    '/commonvalue': async function() {
        const component = document.createElement('commonvalue-main');
        this.setContent(component);
    },
    '/accesstype': async function() {
        const component = document.createElement('accesstype-main');
        this.setContent(component);
    },
    '/workflow': async function() {
        this.renderContent('workflow')();
    },
    '/category': async function() {
        const component = document.createElement('category-main');
        this.setContent(component);
    },
    '/resource': async function() {
        this.renderContent('resource')();
    },
    '/people': async function() {
        const component = document.createElement('people-main');
        this.setContent(component);
    },
    '/user': async function() {
        const component = document.createElement('user-main');
        this.setContent(component);
    },
    '/approvehierarchy': async function() {
        const component = document.createElement('approvehierarchy-main');
        this.setContent(component);
    },
    // request ------------------------------------------------- //
    '/request/send': async function() {
        this.renderContent('request')({ type: 'send' });
    },
    '/request/forapprove': async function() {
        this.renderContent('request')({ type: 'forapprove' });
    },
    '/request/forapprove/:id': async function({parameters}) {
        this.renderContent('request')({ type: 'forapprove', id: parameters.id });
    },
    '/request/search': async function() {
        this.renderContent('request')({ type: 'search' });
    },
    '/request/template': async function() {
        const component = document.createElement('request-template-main');
        this.setContent(component);
    },
    '/request/massdeactivation': async function() {
        this.renderContent('request-massivedeactivation')();
    },
    // reports ------------------------------------------------- //
    '/report/resourcepeople': async function() {
        const component = document.createElement('report-resourcepeople');
        this.setContent(component);
    },
    '/report/bitacora': async function() {
        const component = document.createElement('report-bitacora');
        this.setContent(component);
    },
    '/report/request': async function() {
        const component = document.createElement('report-request');
        this.setContent(component);
    },
    '/report/approver-requestforapproval': async function() {
        const component = document.createElement('report-approver-requestforapproval');
        this.setContent(component);
    },
    '/report/workflow-hierarchy': async function() {
        const component = document.createElement('report-workflow-hierarchy');
        this.setContent(component);
    },
    '/report/oracle-audit': async function () {
        const component = document.createElement('report-oracle-audit');
        this.setContent(component);
    },
    '/report/request-approver': async function () {
        const component = document.createElement('report-request-approver');
        this.setContent(component);
    },
    '/report/users-audit': async function () {
        const component = document.createElement('report-users-audit');
        this.setContent(component);
    },
    '/report/users-roles': async function () {
        const component = document.createElement('report-users-roles');
        this.setContent(component);
    },

    '/report/oracle-sync': async function () {
        const component = document.createElement('report-oracle-sync');
        this.setContent(component);
    },
    '/report/oracle-sync-requests': async function () {
        const component = document.createElement('report-oracle-sync-requests');
        this.setContent(component);
    },
    '/report/sap-sync-log': async function () {
        const component = document.createElement('report-sap-sync-log');
        this.setContent(component);
    },
    '/report/aad-sync-log': async function () {
        const component = document.createElement('report-aad-sync-log');
        this.setContent(component);
    },

    // integracion -------------------------------------------- //
    '/integration/oracle': async function() {
        const component = document.createElement('integration-oracle-main');
        this.setContent(component);
    },

    // defaults
    [Router.ERROR_404]: async function() {
        Router.redirect('/');
    },
    // [Router.ERROR_VALIDATE]: async function() {
    //     this.renderContent('alert-notpermission');
    // }
}