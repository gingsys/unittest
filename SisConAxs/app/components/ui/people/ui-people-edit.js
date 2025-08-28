import { PeopleResource, People } from '../../resources/resource-people.js';
import { CommonValueResource, CommonValueSet } from '../../resources/resource-commonvalue.js';
import { SessionResource, Session } from '../../resources/resource-session.js';
import { User, UserResource } from '../../resources/resource-user.js';
import { UserSearchADComponent } from '../users/ui-search-AD.js';
import { UIEditComponent } from '../common/ui-edit.js';
import { PeopleEditView } from './ui-people-edit.view.js';
import '../controls/ui-control-select.js';

export class PeopleEditComponent extends UIEditComponent {

    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });

        this.$resource = new PeopleResource();
        this.$commonValueResource = new CommonValueResource();
        // this.$useresource = new UserResource();
        this.$entity = new People();
    }

    get $template() {
        return new PeopleEditView(this);
    }

    get Resource() {
        return PeopleResource;
    }
    get Entity() {
        return People;
    }
    get entityID() {
        return 'PeopleID';
    }

    async connectedCallback() {
        super.connectedCallback();

        $('#dptPeopleBirthday').datetimepicker({
            format: 'DD/MM/YYYY',
            maxDate: moment().subtract(18, 'years'),
            date: null,
            locale: 'es',
            widgetPositioning:
            {
                horizontal: 'left',
                vertical: 'bottom'
            },
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down',
                previous: 'fa fa-chevron-left',
                next: 'fa fa-chevron-right',
                today: 'fa fa-dot-circle-o',
                clear: 'fa fa-trash',
                close: 'fa fa-times'
            }
        });
        await this.fillControls();
    }

    async fillControls() {
        await Promise.all([
            CommonValueResource.loadSelect(this.$.cboPeopleDocType, CommonValueSet.SET_TIPO_DOCUMENTO),
            CommonValueResource.loadSelect(this.$.cboPeopleTypeClasificacion, CommonValueSet.SET_TIPO_CLASIFICACION),
            CommonValueResource.fillControlSelectData(this.$.cboPeopleDepartment, CommonValueSet.SET_AREAS),
            CommonValueResource.fillControlSelectData(this.$.cboPeoplePosition, CommonValueSet.SET_CARGOS),
            CommonValueResource.fillControlSelectData(this.$.cboPeopleEmployeeType, CommonValueSet.SET_TIPO_EMPLEADO),
            CommonValueResource.fillControlSelectData(this.$.cboPeopleAttribute2, CommonValueSet.SET_PROYECTOS)
        ]);
    }

    hideDocTypes() {
        const classification = parseInt(this.$.cboPeopleTypeClasificacion.value);
        let docTypes = [];
        if ([People.CLASIFICACION_COLABORADOR, People.CLASIFICACION_COLABORADOR_EN_PROCESO, People.CLASIFICACION_TERCERO].includes(classification)) {
            docTypes = [People.TIPO_DOCUMENTO_DNI,
                        People.TIPO_DOCUMENTO_RUT,
                        People.TIPO_DOCUMENTO_CE,
                        People.TIPO_DOCUMENTO_PASAPORTE,
                        People.TIPO_DOCUMENTO_CEDULA,
                        People.TIPO_DOCUMENTO_DIFERENTE];
        } else if (classification == People.CLASIFICACION_CLIENTE) {
            docTypes = [People.TIPO_DOCUMENTO_RUC, People.TIPO_DOCUMENTO_RUT];
        } else if (classification == People.CLASIFICACION_PROVEEDOR) {
            docTypes = [People.TIPO_DOCUMENTO_RUC, People.TIPO_DOCUMENTO_RUT];
        }
        Array.from(this.$.cboPeopleDocType.options)
        .forEach(o => {
            o.hidden = !docTypes.includes(parseInt(o.value));
        });
    }
    hideNames() {
        const classification = parseInt(this.$.cboPeopleTypeClasificacion.value);
        this.$.lblPeopleFirstName.innerHTML = "Nombres";
        this.$.messagePeopleFirstName.innerHTML = "Por favor, ingrese los Nombres.";
        this.$one('[data-people-name="last"]').hidden = false;
        
        const docType = this.$.cboPeopleDocType.value;
        if(docType == People.TIPO_DOCUMENTO_RUC ||
           [People.CLASIFICACION_CLIENTE, People.CLASIFICACION_PROVEEDOR].includes(classification)) {
            this.$.lblPeopleFirstName.innerHTML = "Razón Social";
            this.$.messagePeopleFirstName.innerHTML = "Por favor, ingrese la Razón Social.";
            this.$one('[data-people-name="last"]').hidden = true;
        }
    }

    hideControls() {
        const classification = parseInt(this.$.cboPeopleTypeClasificacion.value);
        const controls = Array.from(this.$all('[data-classification]'))
        const controlsFiltered = controls.filter(c => c.dataset.classification.split(',').includes(this.$.cboPeopleTypeClasificacion.value));
        controls.forEach(c => c.hidden = true);
        controlsFiltered.forEach(c => c.hidden = false);

        const roleAdministrator = SessionResource.session.havePermission(User.ROLE_ADMINISTRADOR);
        this.$one('[data-workflow]').hidden = !(roleAdministrator &&
                                                [People.CLASIFICACION_COLABORADOR,
                                                 People.CLASIFICACION_COLABORADOR_EN_PROCESO,
                                                 People.CLASIFICACION_TERCERO].includes(classification)
                                               )
        ;
    }

    requireControls() {
        this.$.txtPeopleLastName.required     = true;
        this.$.txtPeopleFirstName.required    = true;
        this.$.cboPeopleEmployeeType.required = true;
        this.$.cboPeopleAttribute2.required   = true;  // proyecto
        this.$.txtPeopleAttribute3.required   = true;  // puesto / cargo
        this.$.cboPeopleEmployeeType.required = true;

        const classification = parseInt(this.$.cboPeopleTypeClasificacion.value);

        if (this.$.cboPeopleDocType.value == People.TIPO_DOCUMENTO_RUC ||
            [People.CLASIFICACION_CLIENTE, People.CLASIFICACION_PROVEEDOR].includes(classification)) {
            this.$.txtPeopleLastName.required = false;
        }

        if([People.CLASIFICACION_TERCERO, People.CLASIFICACION_CLIENTE, People.CLASIFICACION_PROVEEDOR].includes(classification)) {
            this.$.cboPeopleEmployeeType.required = false;
        }

        if ([People.CLASIFICACION_CLIENTE, People.CLASIFICACION_PROVEEDOR].includes(classification)) {
            this.$.cboPeopleAttribute2.required   = false;  // proyecto
            this.$.txtPeopleAttribute3.required   = false;  // puesto / cargo;
        }

        if (SessionResource.session.havePermission(User.ROLE_SYSADMIN)) {
            this.$.cboPeopleEmployeeType.required = false;
        }
    }

    setControls() {
        // disable
        const isColaborador = this.$entity.PeopleTypeClasificacion == People.CLASIFICACION_COLABORADOR && !SessionResource.session.havePermission(User.ROLE_SYSADMIN);
        this.$.txtPeopleLastName.disabled     = isColaborador;
        this.$.txtPeopleFirstName.disabled    = isColaborador;
        this.$.cboPeopleDocType.disabled      = isColaborador;
        this.$.txtPeopleDocNum.disabled       = isColaborador;
        this.$.cboPeopleEmployeeType.disabled = isColaborador;
        this.$.btnProjectDelete.disabled      = isColaborador;
        this.$.cboPeopleAttribute2.disabled   = isColaborador;  // proyecto
        this.$.txtPeopleAttribute3.disabled   = isColaborador;  // puesto / cargo
        this.$.txtPeopleEmail.disabled        = isColaborador;
        this.$.chkPeopleStatus.disabled       = isColaborador;

        this.$.btnSearchAD.disabled = this.$entity.PeopleID > 0;
        // this.$.txtPeopleInternalID.readOnly = this.$entity.PeopleID > 0;
        this.$.txtPeopleDocNum.disabled = this.$entity.PeopleID > 0;
        this.$.cboPeopleDocType.disabled = this.$entity.PeopleID > 0;

        this.requireControls();

        // hide
        this.hideDocTypes();
        this.hideNames();
        this.hideControls();
    }

    searchAD() {
        if (this.$entity.PeopleID == 0) {
            this.$$.formModal.addClass('modal-opened');
            this.$.searchAD.show();
        }
    }

    changeDocType(e) {
        this.hideNames();
        this.requireControls();
        this.$.txtPeopleLastName.value = '';
        this.$.txtPeopleFirstName.value = '';
        if(this.$entity.PeopleID == 0) {
            this.$.txtPeopleInternalID.value = '';
        }
        this.$.txtPeopleDocNum.value = '';
        this.$.txtPeopleDocNum.focus();
    }
    inputDocNum(e) {
        const docType = this.$.cboPeopleDocType.value;
        if (docType == People.TIPO_DOCUMENTO_DNI) {
            this.inputDocNumDNI(e);
        } else if (docType == People.TIPO_DOCUMENTO_RUC) {
            this.inputDocNumRUC(e);
        } else if (docType == People.TIPO_DOCUMENTO_RUT) {
            this.inputDocNumRUT(e);
        } else if (docType == People.TIPO_DOCUMENTO_PASAPORTE) {
            this.inputDocNumPassport(e);
        } else if (docType == People.TIPO_DOCUMENTO_CE) {
            this.inputDocNumCE(e);
        } else if (docType == People.TIPO_DOCUMENTO_CEDULA) {
           this.inputDocNumOtro(e);  // no se valida ?
        } else if (docType == People.TIPO_DOCUMENTO_DIFERENTE) {
            this.inputDocNumOtro(e); // no se valida ?
        } else {
            e.preventDefault();
            e.target.value = '';
        }
    }
    //referencia: https://www2.sunat.gob.pe/pdt/pdtModulos/independientes/p695/TipoDoc.htm
    inputDocNumDNI(e) {
        e.target.value = e.target.value.replace(/[^0-9+]/g, '').substring(0,8);
    }
    inputDocNumRUC(e) {
        e.target.value = e.target.value.replace(/[^0-9+]/g, '').substring(0,11);
    }
    inputDocNumRUT(e) {
        e.target.value = e.target.value.replace(/[^0-9\-kK+]/g, '').substring(0,10);
    }
    inputDocNumPassport(e) {
        e.target.value = e.target.value.replace(/[^0-9a-zA-Z+]/g, '').substring(0,12);
    }
    inputDocNumCE(e) {
        e.target.value = e.target.value.replace(/[^0-9a-zA-Z+]/g, '').substring(0,12);
    }
    inputDocNumOtro(e) {
        e.target.value = e.target.value.substring(0,15);
    }

    changeDocNum(e) {
        // if(this.$entity.PeopleID == 0) {
            this.$.txtPeopleInternalID.value = e.target.value;
        // }
    }

    btnProjectDeleteClick(e) {
        this.$.cboPeopleAttribute2.value = null;
    }
    btnDepartmentDeleteClick(e) {
        this.$.cboPeopleDepartment.value = null;
    }
    btnPositionDeleteClick(e) {
        this.$.cboPeoplePosition.value = null;
    }

    add(classificationType) {
        const entity = new People();
        entity.PeopleTypeClasificacion = classificationType;
        entity.PeopleCompany = SessionResource.session.CompanyID;
        entity.PeopleCompanyName = SessionResource.session.CompanyName;
        this.edit(entity);
    }

    async edit(entity) {
        // this.$presenter.showLoading();
        // await this.fillControls();
        this.$entity = entity;
        this.EntityToForm();
        this.setControls();
        this.render();

        // this.$presenter.hideLoading();
        this.$$.formModal.modal({ backdrop: 'static' });
    }

    setValuesFromAD(e) {
        debugger
        const data = e.detail;
        
        this.$$.formModal.removeClass('modal-opened');
        if (data.surname) {
            this.$.txtPeopleLastName.value  = data.surname.trim();      // apellidos
        }
        if (data.givenName) {
            this.$.txtPeopleFirstName.value = data.givenName.trim();    // nombres
        }
        this.$.cboPeopleDocType.value   = data.UserDocType || People.TIPO_DOCUMENTO_DNI; // es el tipo de documento, por defecto es DNI
        if (data.postalCode.trim() == "EXTERNO" || "SERVICIO") {
            this.$.txtPeopleInternalID.value = "";
        } else {
            this.$.txtPeopleInternalID.value = (data.postalCode || '').trim(); // nro de documento
        }
        this.$.txtPeopleDocNum.value     = (data.postalCode || '').trim();     // nro de documento
        this.$.txtPeopleEmail.value      = (data.mail || '').trim();
        this.$.txtUserID.value           = (data.mail || '').split("@")[0];
        this.$.txtPeopleAttribute3.value = (data.jobTitle || '').trim();       // cargos
        //this.$.cboPeopleTypeClasificacion.value = data.cboPeopleTypeClasificacion || People.CLASIFICACION_COLABORADOR_EN_PROCESO; // Clasificacion, por defecto Colaborador en Proceso
    }
    closeModalSearchAD() {
        this.$$.formModal.removeClass('modal-opened');
        $('body').addClass('modal-open');
    }

    validate(e) {
        // validar formatos de num de documento !
        return true;
    }

    EntityToForm() {
        const entity = this.$entity;
        this.$.txtPeopleInternalID.value        = entity.PeopleInternalID;
        this.$.txtPeopleLastName.value          = `${entity.PeopleLastName} ${entity.PeopleLastName2 || ''}`.trim();
        this.$.txtPeopleFirstName.value         = `${entity.PeopleFirstName || ''} ${entity.PeopleFirstName2 || ''}`.trim();
        this.$.cboPeopleDocType.value           = entity.PeopleDocType;
        this.$.txtPeopleDocNum.value            = entity.PeopleDocNum;
        this.$.cboPeopleTypeClasificacion.value = entity.PeopleTypeClasificacion;
        this.$.cboPeopleEmployeeType.value      = entity.PeopleEmployeeType;
        this.$.lblCompanyName.innerHTML         = entity.PeopleCompanyName;  //LoginResource.auth.CompanyName;

        this.$.cboPeopleAttribute2.value        = entity.PeopleProject;      // Project
        this.$.txtPeopleAttribute3.value        = entity.PeopleAttribute3;   // Position
        this.$.txtPeopleEmail.value             = entity.PeopleEmail;
        this.$.cboPeopleDepartment.value        = entity.PeopleDepartment;   // Department Workflow //this.$.txtPeopleDepartmentName.value = entity.PeopleDepartmentName;
        this.$.cboPeoplePosition.value          = entity.PeoplePosition;     // Position Workflow
        this.$.txtUserID.value                  = entity.UserID;
        this.$.chkPeopleStatus.checked          = entity.PeopleStatus;

        // COMENTADO POR AHORA !!!!!!!!!!!
        // if(entity.PeopleBirthday == null)
        //     this.$$.dptPeopleBirthday.data("DateTimePicker").date(null);
        // else
        //     this.$$.dptPeopleBirthday.data("DateTimePicker").date(moment(entity.PeopleBirthday));
        // this.$.cboPeopleGender.value = entity.PeopleGender;
    }
    FormToEntity() {
        const classification = parseInt(this.$entity.PeopleTypeClasificacion);   // parseInt(this.$.cboPeopleTypeClasificacion.value);
        
        // los colaboradores NO se editan a excepción de los atributos de Workflow
        if (classification != People.CLASIFICACION_COLABORADOR || SessionResource.session.havePermission(User.ROLE_SYSADMIN)) {
        //if(this.$.cboPeopleTypeClasificacion.value != People.CLASIFICACION_COLABORADOR) {
            // if(this.$entity.PeopleID == 0) {
                this.$entity.PeopleInternalID = this.$.txtPeopleInternalID.value.trim();
            // }
    
            if(People.TIPO_DOCUMENTO_RUC == this.$.cboPeopleDocType.value ||
               [People.CLASIFICACION_CLIENTE, People.CLASIFICACION_PROVEEDOR].includes(classification)) {
                this.$entity.PeopleLastName      = '';
                this.$entity.PeopleFirstName     = this.$.txtPeopleFirstName.value.trim();
            } else {
                this.$entity.PeopleLastName      = this.$.txtPeopleLastName.value.trim();
                this.$entity.PeopleFirstName     = this.$.txtPeopleFirstName.value.trim();
            }
            this.$entity.PeopleLastName2         = '';
            this.$entity.PeopleFirstName2        = '';
    
            this.$entity.PeopleDocType           = this.$.cboPeopleDocType.value;
            this.$entity.PeopleDocNum            = this.$.txtPeopleDocNum.value.trim();
            this.$entity.PeopleAttribute2        = this.$.cboPeopleAttribute2.display;         // Project : string
            this.$entity.PeopleProject           = this.$.cboPeopleAttribute2.value;           // Project : int
            // this.$entity.PeopleTypeClasificacion = this.$.cboPeopleTypeClasificacion.value;
            this.$entity.PeopleEmployeeType      = this.$.cboPeopleEmployeeType.value;
            //this.$entity.PeopleCompany           = LoginResource.auth.CompanyID;
    
            this.$entity.PeopleAttribute3        = this.$.txtPeopleAttribute3.value.trim(); // Position
            this.$entity.PeopleEmail             = this.$.txtPeopleEmail.value.trim();
            this.$entity.UserID                  = this.$.txtUserID.value.trim();
            this.$entity.PeopleStatus            = this.$.chkPeopleStatus.checked ? 1 : 0;
        }

        this.$entity.PeopleDepartment    = this.$.cboPeopleDepartment.value;            // Department Workflow
        this.$entity.PeoplePosition      = this.$.cboPeoplePosition.value;              // Position Workflow

        // COMENTADO POR AHORA !!!!!!!!!!!
        // this.$entity.PeopleBirthday = this.$$.dptPeopleBirthday.data("DateTimePicker").date().toDate();
        // this.$entity.PeopleGender   = this.$.cboPeopleGender.value;
    }
}
customElements.define('people-edit', PeopleEditComponent);