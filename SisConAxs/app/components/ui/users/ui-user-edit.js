import { UserEditView } from './ui-user-edit.view.js';
import { CompanyResource } from '../../resources/resource-company.js';
import { User, UserCompany, UserResource } from '../../resources/resource-user.js';
import { SessionResource } from '../../resources/resource-session.js';
import { UserSearchADComponent } from './ui-search-AD.js';
import { UIEditComponent } from '../common/ui-edit.js';

export class UserEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
        this.__sourceEntity = {};
        this.__edited = false;
    }

    get $template() {
        return new UserEditView(this);
    }

    get Resource() {
        return UserResource;
    }
    get Entity() {
        return User;
    }
    get entityID() {
        return 'UserID';
    }

    connectedCallback() {
        super.connectedCallback();
        
        const inputs = Array.from(this.$all("input.txt-disabled"));
        inputs.forEach(x => {
            x.addEventListener("mousedown", function(e) {
                e.preventDefault();
                this.blur();
                return false;
            });
        });
        // this.$.txtUserFirstName.addEventListener("mousedown", function(e) {
        //     e.preventDefault();
        //     this.blur();
        //     return false;
        // });
        this.$.btnSearchAD.focus();
    }

    searchAD() {
        if (this.$entity.UserID == 0) {
            this.$$.formModal.addClass('modal-opened');
            this.$.searchAD.show();
        }
    }
    setValuesFromAD(e) {
        const data = e.detail;
        const position = data.mail.trim().indexOf("@");
        const userAlias = data.mail.trim().substring(0, position);

        this.$.txtUserInternalID.value = userAlias;  // data.mail;
        this.$.txtUserLastName.value   = data.surname;
        this.$.txtUserFirstName.value  = data.givenName;
        // this.$.txtIdAzure.value = data.id;

        this.$$.formModal.removeClass('modal-opened');
    }
    closeModalSearchAD() {
        this.$$.formModal.removeClass('modal-opened');
    }

    async loadCompanies() {
        const select = this.$.lstCompany;
        if (!!SessionResource.session.UserRoleSysAdmin) {
            await CompanyResource.loadSelect(select);
        } else {
            select.innerHTML = '';
            SessionResource.session.User.UserCompanies.forEach(item => {
                const opt = document.createElement('option');
                opt.innerHTML = item.CompanyDisplay;
                opt.value = item.CompanyID;
                opt.$data = item;
                select.appendChild(opt);
            });
        }

        let optCurrentCompany = Array.from(select.options).find(x => x.$data.CompanyID == SessionResource.session.CompanyID);
        let index = optCurrentCompany !== undefined ? optCurrentCompany.index : 0;
        select.selectedIndex = index;

        // data
        Array.from(select.options).forEach(x => {
            let detail = this.$entity.UserCompanies.find(y => y.CompanyID == x.$data.CompanyID);
            if (detail == undefined) {
                detail = new UserCompany();
                detail.CompanyID = x.$data.CompanyID;
                this.$entity.UserCompanies.push(detail);
            }
        });
    }

    changeListCompany(e) {
        const companyID = parseInt(this.$.lstCompany.value);
        const detail = this.$entity.UserCompanies.find(x => x.CompanyID == companyID);
        if (detail !== undefined) {
            this.$.chkUserRole1.checked = detail.UserRole1;
            this.$.chkUserRole2.checked = detail.UserRole2;
            this.$.chkUserRole3.checked = detail.UserRole3;
            this.$.chkUserRole4.checked = detail.UserRole4;
            this.$.chkUserRole5.checked = detail.UserRole5;
            this.$.chkUserRole6.checked = detail.UserRole6;
            this.$.chkUserRole7.checked = detail.UserRole7;
        } else {
            this.$.chkUserRole1.checked = false;
            this.$.chkUserRole2.checked = false;
            this.$.chkUserRole3.checked = false;
            this.$.chkUserRole4.checked = false;
            this.$.chkUserRole5.checked = false;
            this.$.chkUserRole6.checked = false;
            this.$.chkUserRole7.checked = false;
        }
    }

    checkEdition() {
        this.__edited = !this.__sourceEntity.equal(this.$entity);
        var validateMinRoles = this.validateMinRoles();
        if (this.__edited == true)
            this.__edited = validateMinRoles;
        if (!validateMinRoles)
            iziToast.warning({ title: 'Favor de seleccionar al menos un rol cuando active un usuario.', timeout: 5000 });
        this.render();
    }
    checkStatus(e) {
        this.$entity.UserStatus = e.target.checked ? 1 : 0;
        if (this.$entity.UserStatus == 0) {
            this.$.chkUserRole1.checked = false;
            this.$.chkUserRole2.checked = false;
            this.$.chkUserRole3.checked = false;
            this.$.chkUserRole4.checked = false;
            this.$.chkUserRole5.checked = false;
            this.$.chkUserRole6.checked = false;
            this.$.chkUserRole7.checked = false;
            this.$entity.UserCompanies.forEach(company => {
                company.UserRole1 = 0;
                company.UserRole2 = 0;
                company.UserRole3 = 0;
                company.UserRole4 = 0;
                company.UserRole5 = 0;
                company.UserRole6 = 0;
                company.UserRole7 = 0;
            });
        }
        this.checkEdition();
    }
    validateMinRoles() {
        var companies = this.$entity.UserCompanies.filter(company => company.UserRole1 == 1 || company.UserRole2 == 1 || company.UserRole3 == 1 || company.UserRole4 == 1
            || company.UserRole5 == 1 || company.UserRole6 == 1 || company.UserRole7 == 1);
        return this.$entity.UserStatus == 1 ? companies.length > 0: true;
    }
    checkRole(e) {
        const companyID = parseInt(this.$.lstCompany.value);
        const detail    = this.$entity.UserCompanies.find(x => x.CompanyID == companyID);
        detail[e.target.dataset.attr] = e.target.checked ? 1 : 0;
        this.checkEdition();
    }

    add() {
        const entity = new User();
        entity.UserStatus = 1;
        this.edit(entity);
    }

    async edit(entity) {
        this.__edited = false;
        this.$entity = User.fromObject(entity);
        this.EntityToForm();

        await this.loadCompanies();
        this.changeListCompany();

        this.__sourceEntity = this.$entity.copy();

        this.$.btnSearchAD.disabled = entity.UserID > 0;
        this.$.txtUserInternalID.select();
        this.render();
        this.$$.formModal.modal({ backdrop: 'static' });
    }

    EntityToForm() {
        this.$.txtUserInternalID.value = this.$entity.UserInternalID;
        this.$.txtUserLastName.value   = this.$entity.UserLastName;
        this.$.txtUserFirstName.value  = this.$entity.UserFirstName;
        this.$.chkUserStatus.checked   = this.$entity.UserStatus;
    }
    FormToEntity() {
        this.$entity.UserInternalID = this.$.txtUserInternalID.value.trim();
        this.$entity.UserLastName   = this.$.txtUserLastName.value.trim();
        this.$entity.UserFirstName  = this.$.txtUserFirstName.value.trim();
        this.$entity.UserStatus     = this.$.chkUserStatus.checked ? 1 : 0;
    }


    validate(e) {
        return true;
    }

    // delete(e) {
    //     if (this.$entity.UserID > 0) {
    //         this.$presenter.question(
    //             'Borrar',
    //             'Desea borrar este registro?',
    //             (instance, toast) => {
    //                 this.$presenter.showLoading();
    //                 this.$parent.$resource.delete(this.$entity.UserID).then(
    //                     r => {
    //                         this.$$.formModal.modal('hide');
    //                         this.$presenter.hideLoading();
    //                         console.log('borrado!');
    //                         iziToast.success({ title: 'Borrado correctamente' });
    //                         this.dispatchEvent(new CustomEvent('delete', { detail: this.$entity.UserID }));
    //                     },
    //                     err => {
    //                         this.$presenter.hideLoading();
    //                         // console.error('error!', err);
    //                         iziToast.error({ title: err });
    //                     }
    //                 );
    //             }
    //         );
    //     }
    // }
}
customElements.define('user-edit', UserEditComponent);