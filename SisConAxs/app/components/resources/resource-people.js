import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class People extends Entity {
    static get TIPO_DOCUMENTO_DNI() { return 1; }
    static get TIPO_DOCUMENTO_PASAPORTE() { return 2; }
    static get TIPO_DOCUMENTO_CE() { return 3; }
    static get TIPO_DOCUMENTO_DIFERENTE() { return 4; }
    static get TIPO_DOCUMENTO_RUT() { return 682; }
    static get TIPO_DOCUMENTO_RUC() { return 1430; }
    static get TIPO_DOCUMENTO_CEDULA() { return 1458; }

    static get CLASIFICACION_COLABORADOR() { return 2655; }
    static get CLASIFICACION_PROVEEDOR() { return 2656; }
    static get CLASIFICACION_TERCERO() { return 2657; }
    static get CLASIFICACION_COLABORADOR_EN_PROCESO() { return 2829; }
    static get CLASIFICACION_CLIENTE() { return 2830; }

    constructor() {
        super();
        this.AssignedItems = [];
        this.ForApprove = 0;
        this.PendingApproveItems = null;
        this.PeopleAddress1 = "";
        this.PeopleAddress2 = "";
        this.PeopleAttribute2 = null;
        this.PeopleProject = null;
        this.PeopleAttribute3 = null;
        this.PeopleCompany = 0;
        this.PeopleCompanyName = "";
        this.PeopleCommonNames = "";
        this.PeopleDepartment = null;
        this.PeopleDepartmentName = null;
        this.PeopleDocNum = "";
        this.PeopleDocType = 0;
        this.PeopleDocTypeName = "";
        this.PeopleEmail = "";
        this.PeopleFirstName = "";
        this.PeopleFirstName2 = "";
        // this.PeopleFullname = "";
        this.PeopleID = 0;
        this.PeopleInternalID = "";
        this.PeopleLastName = "";
        this.PeopleLastName2 = "";
        this.PeopleGender = "";
        this.PeopleBirthday = null;
        this.PeopleOrdinalNames = "";
        this.PeopleOrgID = 0;
        this.PeoplePhone1 = "";
        this.PeoplePhone2 = "";
        this.PeoplePosition = null;
        this.PeoplePositionName = null;
        this.PeopleStatus = 1;
        this.PeopleStatusDesc = "";
        this.UserID = "";
        this.UserInternalID = "";
        this.PeopleCompanyName = "";        
        
        this.PeopleTypeClasificacion = 0;
        this.PeopleTypeClasificacionName = "";
        this.PeopleEmployeeType = null;
        this.PeopleEmployeeTypeName = "";

        this.PeopleIsSourceSAP = false;
        this.PeopleStartDate = null;

        this.Failed = false;
        this.FailedDescription = "";

        // Datos azure AD:PARA PERSONA
        //this.displayName = null;              // nombres completos
        //this.givenName = null;                // 2 nombres 
        //this.jobTitle = null;                 // puesto de trabajo
        //this.mail = null;                     // email
        //this.officeLocation = null;           //  Localizaci�n de la oficina
        //this.surname = null;                  // 2 apellidos
        //this.userPrincipalName = null;        // correo usuario principal
        //this.id = null;                        // ID de cada usuario que trae del azure ad
        //this.postalCode = null;               // DNI
        //this.companyName = null;              // Nombre compa�ia
    }

    get PeopleFullFirstName() {
        return `${this.PeopleFirstName} ${this.PeopleFirstName2 || ''}`.trim();
    }
    get PeopleFullLastName() {
        return `${this.PeopleLastName || ''} ${this.PeopleLastName2 || ''}`.trim();
    }
    get PeopleFullname() {
        return `${!!this.PeopleFullLastName? `${this.PeopleFullLastName},` : ''} ${this.PeopleFullFirstName || ''}`.trim();
    }

    fromDTO(dto) {
        super.fromDTO(dto);
    }
}
export class PeopleResource extends ResourceBase {
    get api() {
        return '/api/People';
    }

    getPaginate(params = undefined) {
        return PeopleResource.$get(`/api/PeoplePaginate/`, params, this.__requestOptions);
    }

    listHasAccess(params = undefined) {
        return PeopleResource.$get(`/api/People/hasAccess`, params, this.__requestOptions);
    }

    approvedRequestPending() {
        return PeopleResource.$get(`/api/People/approvers/pendings`, undefined, this.__requestOptions);
    }
}