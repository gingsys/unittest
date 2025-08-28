import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class User extends Entity {
    static get ROLE_SOLICITANTE() { return 1; }
    static get ROLE_APROBADOR() { return 2; }
    static get ROLE_ADMINISTRADOR() { return 4; }
    static get ROLE_CREA_PERSONAS() { return 8; }
    static get ROLE_DAR_BAJA() { return 16; }
    static get ROLE_VER_SOLICITUDES_PROYECTO() { return 32; }
    static get ROLE_SYSADMIN() { return 1024; }
    static get ROLE_REPORTE() { return 64; }

    static get SOLICITANTE_ACCESS() { return 'UserRole1'; }
    static get APROBADOR_ACCESS() { return 'UserRole2'; }
    static get ADMINISTRADOR_ACCESS() { return 'UserRole3'; }
    static get CREAR_PERSONAS_ACCESS() { return 'UserRole4'; }
    static get DAR_BAJA_ACCESS() { return 'UserRole5'; }
    static get VER_SOLICITUDES_ACCESS() { return 'UserRole6'; }
    static get REPORTES_ACCESS() { return 'UserRole7'; }

    static get ROLE_NAMES() {
        return {
            [User.SOLICITANTE_ACCESS]: 'SOLICITANTE',
            [User.APROBADOR_ACCESS]: 'APROBADOR',
            [User.ADMINISTRADOR_ACCESS]: 'ADMINISTRADOR',
            [User.CREAR_PERSONAS_ACCESS]: 'CREAR PERSONAS',
            [User.DAR_BAJA_ACCESS]: 'DAR DE BAJA',
            [User.VER_SOLICITUDES_ACCESS]: 'VER SOLICITUDES PROYECTO',
            [User.REPORTES_ACCESS]: 'REPORTES'
        };
    }

    constructor() {
        super();
        this.UserAddress1 = null;
        this.UserAddress2 = null;
        this.UserCode = null;
        this.UserCompany = null;
        this.UserDocNum = null;
        this.UserDocType = 0;
        this.UserDocTypeDisplay = null;
        this.UserEMail = null;
        this.UserFirstName = '';
        this.UserID = 0;
        this.UserInternalID = '';
        this.UserLastName = '';
        this.UserName3 = null;
        this.UserName4 = null;
        this.UserPassword = '';
        this.UserPhone1 = null;
        this.UserPhone2 = null;
        this.UserRole1 = 0;
        this.UserRole2 = 0;
        this.UserRole3 = 0;
        this.UserRole4 = 0;
        this.UserRole5 = 0;
        this.UserRole6 = 0;
        this.UserRole7 = 0;
        this.UserStatus = 0;
        this.UserCompanies = [];
        this.UserStatusDesc = '';

        // Datos azure AD:
        this.displayName = null;              // nombres completos
        this.givenName = null;                // 2 nombres 
        this.jobTitle = null;                 // puesto de trabajo
        this.mail = null;                     // email
        this.officeLocation = null;           //  Localizaci�n de la oficina
        this.surname = null;                  // 2 apellidos
        this.userPrincipalName = null;        // correo usuario principal
        this.id = null;                        // ID de cada usuario que trae del azure ad
        this.postalCode = null;               // DNI
        this.companyName = null;              // Nombre compa�ia

    }

    // get UserStatusDesc() {
    //     return this.UserStatus == 0 ? 'Inactivo' : 'Activo';
    // }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.UserCompanies = UserCompany.fromList(dto.UserCompanies || []);
    }
}
export class UserCompany extends Entity {
    constructor() {
        super();
        this.UserID = 0;
        this.CompanyID = 0;
        this.UserRole1 = 0;
        this.UserRole2 = 0;
        this.UserRole3 = 0;
        this.UserRole4 = 0;
        this.UserRole5 = 0;
        this.UserRole6 = 0;
        this.UserRole7 = 0;
    }
}

export class UserResource extends ResourceBase {
    get api() {
        return '/api/Users';
    }

    // get from Active Directory
    getFromAD(params) {
        return UserResource.$get(`/api/Users/AD/`, params, this.__requestOptions);
    }

    getPaginate(params = undefined) {
        return UserResource.$get(`/api/UserPaginate/`, params, this.__requestOptions);
    }

    findUserAD(params) {
        return UserResource.$get(`/api/Users/findUserAD/`, params, this.__requestOptions);
    }

    setSysAdmin(userAlias, status) {
        const user = { 
            UserInternalID: userAlias,
            UserRoleSysAdmin: status
        };
        return UserResource.$post(`/api/Users/SysAdmin`, user, undefined, this.__requestOptions);
    }
}