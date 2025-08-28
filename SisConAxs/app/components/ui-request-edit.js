import { Component, html } from '../core.js';
import { SessionResource } from './resources/resource-session.js';
import { User } from './resources/resource-user.js';
import { AccessType } from './resources/resource-accesstype.js';
import { RequestResource, Request, RequestDetail } from './resource-request.js';
import { CommonValueResource, CommonValueSet } from './resources/resource-commonvalue.js';

import { RequestEditView } from './ui-request-edit-view.js';
import { TreeSelectResource } from './ui-resource.js';

import { RequestDetailHistoryComponent } from './ui-request-detail-history.js';
import { RequestConfirmComponent } from './ui-request-confirm.js';
import { AlertComponent } from './ui-alert.js';
import { ResourceResource, Resource } from './resource-resource.js';
import { RequestDetailApplicantComponent } from './ui-request-detail-applicant.js';

import './ui/people/ui-people.js';
import './ui/people/ui-people-search.js';
import { Constants } from './resources/constants.js';
import { People } from './resources/resource-people.js';

import { IntegrationOracleResource } from './resources/resource-integration-oracle.js';

export class RequestEditComponent extends Component {
    static get ACTA_RIESGOS() { return 'ACTA_RIESGOS' };
    static get NO_MATERIALIZA() { return 'NO_MATERIALIZA' };
    static get ACTA_PDF_URL() { return '/Files/AS-15-Aceptacion_de_riesgo_relacionado_conflico_SOD.pdf' }
    static get ACTA_PDF_NAME() { return 'AS-15-Aceptacion_de_riesgo_relacionado_conflico_SOD.pdf' }
    
    constructor(presenter) {
        super(presenter);

        this.$entity = new Request();
        this.$entityDetail = null;
        this.$currentPeople = null;
        this.$resourceCommonValueSet = new CommonValueResource();
        this.$resourceIntOracle = new IntegrationOracleResource();
        this.$dataResource = [];

        this.detailExpanded = false;

        this.addComponent('requestDetailHistory', new RequestDetailHistoryComponent(presenter));

        this.addComponent('requestConfirm', new RequestConfirmComponent(presenter));
        this.$component.requestConfirm.onConfirm = (value) => this.save();
        this.$component.requestConfirm.onClose = () => this.closeModalRequestConfirm();

        this.addComponent('alert', new AlertComponent(presenter));
        this.addComponent('detailApplicant', new RequestDetailApplicantComponent(presenter));
        this.$component.detailApplicant.onClose = () => this.closeModalRequestConfirm();

        this.$tableView = null;

        this.conflictoDetectado = false;
        this.sodResources = [];        
    }

    __render() {
        return RequestEditView.render(this);
    }

    __ready() {
        let $this = this;
        this.treeSelectResource = ReactDOM.render(React.createElement(TreeSelectResource), this.$.searchResource);
        CommonValueResource.loadSelect(this.$.cboType, CommonValueSet.SET_TIPO_SOLICITUD);
        CommonValueResource.loadSelect(this.$.cboPriority, CommonValueSet.SET_PRIORIDAD_SOLICITUD);

        this.$tableView = new RequestDetailTableView(this, this.$$.tableDetail);

        this.resetCategoryResource();
    }


    btnPeopleSelfClick(e) {
        if (this.$entity.RequestID == 0) {
            if (this.$currentPeople == null) {
                iziToast.error({ title: 'No tiene un registro de persona asociada a su usuario.' });
                return;
            }
            this.generateDetailFromPeople(this.$currentPeople);
        }
    }
    btnPeopleSearchClick(e) {
        if (this.$entity.RequestID == 0) {
            this.$.peopleSearch.show();
        }
    }
    btnPeopleAddClick(e) {
        if (this.$entity.RequestID == 0) {
            this.$.peopleAdd.add();
        }
    }

    btnShowToggleNoteAdd(e) {
        this.showNoteAdd = !this.showNoteAdd;
        this.render();
        if (this.showNoteAdd)
            this.$.txtNoteAdd.select();
    }

    async btnAddDetailClick(e) {
        const $this = this;
        if (this.$entity.RequestID == 0 && this.requestPeopleToData != null) {
            const selected = this.treeSelectResource.getSelectedData();
            if (selected.length > 0) {
                const detail = this.$tableView.getData();
                const resourcesID = detail.map(x => x.ResourceID);
                const toAdd = RequestDetail.fromList(selected.filter(x => !resourcesID.includes(x.ResourceID)))
                    .map(x => {
                        x.RequestDetType = Request.TYPE_ALTA;
                        return x;
                    });
                
                // valida los recursos de Oracle
                const hasOracleResource = toAdd.some(r => r.ResourceCategoryID == Constants.CATEGORY_ORACLE_ERP);
                if (hasOracleResource && !(this.requestPeopleToData.PeopleTypeClasificacion == People.CLASIFICACION_COLABORADOR || this.requestPeopleToData.PeopleTypeClasificacion == People.CLASIFICACION_COLABORADOR_EN_PROCESO)) {
                    this.$component.alert.show($.parseHTML(`No puede seleccionar recursos de Oracle si el destinatario NO es de Tipo Colaborador o Colaborador Manual.`));
                    return;
                }
                if(hasOracleResource && ((this.requestPeopleToData.UserID || '') == '')) {
                    this.$component.alert.show($.parseHTML(`No puede seleccionar recursos de Oracle si el destinatario NO tiene un usuario válido.`));
                    return;
                }

                // // valida las responsabilidades de Oracle
                // if(hasOracleResource) {
                //     const oracleResources = toAdd.filter(r => r.ResourceCategoryID == Constants.CATEGORY_ORACLE_ERP);
                //     console.warn(oracleResources);

                //     let haveRespConflitcs = false;
                //     await this.$resourceIntOracle.checkResponsabilityConflicts()
                //     .then(r => {
                //         console.warn('checkResponsabilityConflicts', r);
                //     })
                //     .catch(e => {
                //         haveRespConflitcs = true;
                //         console.warn('checkResponsabilityConflicts : Error', e);
                //     });
                //     if(haveRespConflitcs) return;
                // }

                // mostramos una alerta si los items seleccionados ya se encuentran o estan pendientes
                const itemsExists = RequestDetail.fromList(selected.filter(x => resourcesID.includes(x.ResourceID))).map(x => {
                    return `<li>${"<b>" + x.ResourceCategoryDisplay + "</b>/" + x.ResourceFullName}</li>`;
                }).sort(obj => obj.ResourceCategoryDisplay);

                if (itemsExists.length > 0) {
                    this.$component.alert.show($.parseHTML(`Los siguientes recursos se encuentran asignados o están en proceso:<br><br><ul>` + itemsExists.join('') + `</ul>`));
                    return;
                }
                
                const sodResourcesToAdd = toAdd.filter(i => i.ResourceParamValue != null);
                sodResourcesToAdd.forEach(i => {
                    this.sodResources.push(i)
                })
                if (this.sodResources.length > 0) {
                    if ((this.requestPeopleToData.UserInternalID || "") == "") {
                        this.$component.alert.show($.parseHTML(`No puede seleccionar recursos de Acceso a Icarus si el destinatario NO tiene un usuario válido.`));
                        return;
                    }
                    this.prepareSodMatrix(detail, this.sodResources);
                } else {
                    this.conflictoDetectado = false;
                }

                const toTable = detail.concat(toAdd).sort((a, b) => `${a.ResourceCategoryDisplay} ${a.ResourceFullName}`.localeCompare(`${b.ResourceCategoryDisplay} ${b.ResourceFullName}`));
                this.$tableView.load(toTable);
                //Agregamos al detalle de la entidad los recursos agregados
                toAdd.forEach(function (element) {
                    $this.$entity.AccessRequestDetails.push(element);
                });

                this.treeSelectResource.clearSelected();
                this.treeSelectResource.clearFilter();
            }
        }
    }
    buildSodConflictMap() {
        const reglasConflictoSOD = [
            // Conflictos basados en la matriz SOD
            // Fila: Solicitante (UserRole1)
            [User.SOLICITANTE_ACCESS, User.ADMINISTRADOR_ACCESS, {      // Solicitante vs. Administrador
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],
            [User.SOLICITANTE_ACCESS, User.APROBADOR_ACCESS, {          // Solicitante vs. Aprobador
                type: RequestEditComponent.NO_MATERIALIZA,
                //messageTemplate: (r1, r2) => `los roles "${r1}" y "${r2}", no se materializaría el conflicto por restricción en el sistema`,
                disablesInput: false
            }],

            // Fila: Administrador (UserRole3)
            [User.ADMINISTRADOR_ACCESS, User.SOLICITANTE_ACCESS, {      // Administrador vs. Solicitante
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],
            [User.ADMINISTRADOR_ACCESS, User.APROBADOR_ACCESS, {        // Administrador vs. Aprobador
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],
            [User.ADMINISTRADOR_ACCESS, User.CREAR_PERSONAS_ACCESS, {   // Administrador vs. Crear Personas
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],
            [User.ADMINISTRADOR_ACCESS, User.ADMINISTRADOR_ACCESS, {   // Administrador vs. Crear Personas
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],

            // Fila: Aprobador (UserRole2)
            [User.APROBADOR_ACCESS, User.ADMINISTRADOR_ACCESS, {        // Aprobador vs. Administrador
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }],
            [User.APROBADOR_ACCESS, User.SOLICITANTE_ACCESS, {          // Aprobador vs. Solicitante
                type: RequestEditComponent.NO_MATERIALIZA,
                //messageTemplate: (r1, r2) => `los roles "${r1}" y "${r2}", no se materializaría el conflicto por restricción en el sistema`,
                disablesInput: false
            }],

            // Fila: Crear Personas(UserRole4)
            [User.CREAR_PERSONAS_ACCESS, User.ADMINISTRADOR_ACCESS, {   // Crear Personas vs. Administrador
                type: RequestEditComponent.ACTA_RIESGOS,
                //messageTemplate: (r1, r2) => `conflicto entre el rol "${r1}" y el rol "${r2}" solicitado`,
                disablesInput: true
            }]
        ];

        const mapaReglasConflicto = new Map();
        reglasConflictoSOD.forEach(regla => {
            const rolA = regla[0];
            const rolB = regla[1];
            const propiedades = regla[2];

            // Asegurarse de que ambas direcciones estén cubiertas: (A,B) y (B,A) en el mapa para fácil consulta
            if (!mapaReglasConflicto.has(rolA)) {
                mapaReglasConflicto.set(rolA, new Map());
            }
            mapaReglasConflicto.get(rolA).set(rolB,propiedades);

            if (!mapaReglasConflicto.has(rolB)) {
                mapaReglasConflicto.set(rolB, new Map());
            }
            mapaReglasConflicto.get(rolB).set(rolA,propiedades);
        });

        return mapaReglasConflicto;
    }
    prepareSodMatrix(requestDetails, toAdd) {
        let currentResourcesWithParams = requestDetails.filter(i => i.ResourceParamValue != null);
        let currentResourceWithParamsToAdd = toAdd;

        const currentRoles = currentResourcesWithParams.map(rol => this.extractRoleFromParameter(rol.ResourceParamValue))
        const requestedRoles = currentResourceWithParamsToAdd.map(rol => this.extractRoleFromParameter(rol.ResourceParamValue))        

        this.validateSodConflicts(currentRoles, requestedRoles, this.buildSodConflictMap());
    }
    extractRoleFromParameter(resourceParamValue) {
        const rol = resourceParamValue.split('-');
        if (rol.length > 0) {
            return rol[0]; // Retorna 'UserRole1', 'UserRole2', etc.
        }
        return '';
    }
    validateSodConflicts(rolesActuales, rolesSolicitados, reglasConflictoMap) {
        this.conflictoDetectado = false;
        this.$.txtNote.value = '';

        let conflictosDetectados = [];
        // Convertir a Sets para búsquedas rápidas
        const setRolesActuales = new Set(rolesActuales);
        const setRolesSolicitados = new Set(rolesSolicitados);

        // --- Función auxiliar para detectar y almacenar un conflicto ---
        const addConflict = (rol1, rol2) => {
            // Normalizar el par de roles para la clave del mapa (rolA-rolB, rolB-rolA)
            // Y para User.ROLE_NAMES
            const nombreRol1 = User.ROLE_NAMES[rol1] || rol1;
            const nombreRol2 = User.ROLE_NAMES[rol2] || rol2;

            // Recuperar las propiedades del conflicto directamente del mapa anidado
            const propiedades = reglasConflictoMap.get(rol1)?.get(rol2);

            if (propiedades) {
                // Crear una clave única para evitar duplicados en conflictosDetectados
                const key = [rol1, rol2].sort().join('-');
                if (!conflictosDetectados.some(c => c.key === key)) {
                    conflictosDetectados.push({
                        rol1, rol2, key,
                        propiedades, // Almacena las propiedades del conflicto (type, messageTemplate, disablesInput)
                        nombreRol1, nombreRol2 // Nombres amigables para usar en el mensaje
                    });
                }
            }
        };

        // --- 1. Validar conflictos entre roles solicitados y roles actuales ---
        // (Un rol de 'solicitados' VS un rol de 'actuales')
        for (const rolSolicitado of setRolesSolicitados) {
            const conflictosDeRolSolicitadoMap = reglasConflictoMap.get(rolSolicitado); // Obtiene el Map anidado para este rol
            if (conflictosDeRolSolicitadoMap) {
                for (const rolActual of setRolesActuales) {
                    if (conflictosDeRolSolicitadoMap.has(rolActual)) { // Verifica si el par (rolSolicitado, rolActual) tiene una entrada
                        addConflict(rolSolicitado, rolActual);
                    }
                }
            }
        }

        // --- 2. Validar conflictos dentro de la lista de roles solicitados ---
        const arrayRolesSolicitados = Array.from(setRolesSolicitados);
        for (let i = 0; i < arrayRolesSolicitados.length; i++) {
            const rol1Solicitado = arrayRolesSolicitados[i];
            const conflictosDeRol1SolicitadoMap = reglasConflictoMap.get(rol1Solicitado);

            if (conflictosDeRol1SolicitadoMap) {
                for (let j = i; j < arrayRolesSolicitados.length; j++) {
                    const rol2Solicitado = arrayRolesSolicitados[j];
                    if (conflictosDeRol1SolicitadoMap.has(rol2Solicitado)) {
                        addConflict(rol1Solicitado, rol2Solicitado);
                    }
                }
            }
        }

        // --- CONSTRUCCIÓN DEL MENSAJE FINAL Y ESTADO DEL CONFLICTO ---
        let mensajePrincipal = 'Se realiza la validación del Análisis SOD "automática"';
        let mensajeSecundario = '';

        // Filtra los conflictos que requieren Acta de Riesgos y los que no deshabilitan
        const conflictosActaDeRiesgos = conflictosDetectados.filter(c => c.propiedades.type === RequestEditComponent.ACTA_RIESGOS);
        const conflictosNoMaterializacion = conflictosDetectados.filter(c => c.propiedades.type === RequestEditComponent.NO_MATERIALIZA);

        // Determinar si el input debe deshabilitarse
        this.conflictoDetectado = conflictosActaDeRiesgos.some(c => c.propiedades.disablesInput);

        if (conflictosActaDeRiesgos.length > 0) {
            // Construir la lista de pares de roles para el mensaje de Acta de Riesgos
            //const paresDeRolesEnConflicto = conflictosActaDeRiesgos.map(c =>
            //    c.propiedades.messageTemplate(c.nombreRol1, c.nombreRol2)
            //).join(' y ');

            mensajeSecundario = `, identificándose conflicto con los roles solicitados; el dueño del proceso a la aprobación de esta solicitud, estará aceptando el Acta de Aceptación de Riesgos que se encuentra como parte del adjunto.`;
        } else if (conflictosNoMaterializacion.length > 0) {
            // Si no hay conflictos de Acta de Riesgos, pero sí de no materialización
            //const paresDeRolesNoMaterializa = conflictosNoMaterializacion.map(c =>
            //    c.propiedades.messageTemplate(c.nombreRol1, c.nombreRol2)
            //).join(' y ');

            mensajeSecundario = `, identificando el conflicto con los roles solicitados; no se materializaría el conflicto por restricción en el sistema.`;
            this.clearFileAttached()
        } else if (setRolesSolicitados.size > 0) { // Si no hay conflictos y hay roles solicitados
            mensajeSecundario = `, no se identifica conflicto.`;
            this.clearFileAttached()
        } else { // Si no hay roles solicitados
            mensajeSecundario = ''; // Mensaje más específico
            this.clearFileAttached()
        }

        if (rolesSolicitados.length > 0) {
            this.$.txtNote.value = mensajePrincipal + mensajeSecundario;
        } else {
            this.$.txtNote.value = mensajeSecundario;
        }
        this.render(); // Para actualizar la UI
    }
    async attachActaPDF() {
        try {
            const response = await fetch(RequestEditComponent.ACTA_PDF_URL);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const pdfBlob = await response.blob();
            const actaFile = new File([pdfBlob], RequestEditComponent.ACTA_PDF_NAME, { type: 'application/pdf' });
            return actaFile;
        } catch (error) {
            console.error('Error al adjuntar el Acta de Aceptación de Riesgos:', error);
            this.$component.alert.show($.parseHTML(`Error al adjuntar el Acta de Aceptación de Riesgos. Por favor, inténtelo de nuevo o contacte a soporte.`));
        }
    }
    btnDeleteDetailClick(e) {
        if (this.$entity.RequestID == 0 && this.requestPeopleToData != null) {
            let selected = this.treeSelectResource.getSelectedData().map(x => x.ResourceID);
            if (selected.length > 0) {
                let detail = this.$tableView.getData();
                detail = detail.filter(x => !selected.includes(x.ResourceID) || !!x.RequestDetPrevData);
                detail.filter(x => selected.includes(x.ResourceID) && x.RequestDetPending == false)
                    .forEach(x => {
                        if (x.RequestDetType == null)
                            x.RequestDetType = Request.TYPE_BAJA;
                    });
                detail = detail.sort((a, b) => `${a.ResourceCategoryDisplay} ${a.ResourceFullName}`.localeCompare(`${b.ResourceCategoryDisplay} ${b.ResourceFullName}`));

                //mostramos una alerta a los recursos seleccionados que no son válidos de baja: Pendientes y que no existan.
                let itemsNotValidToDelete = this.treeSelectResource.getSelectedData().filter(x => !detail.map(p => p.ResourceID).includes(x.ResourceID) || x.RequestDetPending == true).map(x => {
                    return `<li>${"<b>" + x.ResourceCategoryDisplay + "</b>/" + x.ResourceFullName}</li>`;
                });

                if (itemsNotValidToDelete.length > 0) {
                    this.$component.alert.show($.parseHTML(`Los siguientes recursos no están asignados o están en proceso:<br><br><ul>` + itemsNotValidToDelete.join('') + `</ul>`));
                } else {
                    //Agregamos al detalle de la entidad los recursos quitados
                    let $this = this;
                    let listdetail = this.$tableView.getData();
                    $this.$entity.AccessRequestDetails = [];
                    listdetail.forEach(function (element) {
                        if (element.RequestDetType == Request.TYPE_ALTA || element.RequestDetType == Request.TYPE_BAJA) {
                            $this.$entity.AccessRequestDetails.push(element);
                        }
                    });
                    this.$tableView.load(detail);
                    this.treeSelectResource.clearSelected();
                }
            }
        }
    }

    btnApproveAllClick() {

        this.$entity.AccessRequestDetails.forEach(x => x.RequestResponse = RequestDetail.REPONSE_ACCEPT);
        this.$tableView.refreshView();
        this.$.btnApproveAll.classList.add('active');
        this.$.btnRejectAll.classList.remove('active');
    }
    btnRejectAllClick() {
        this.$entity.AccessRequestDetails.forEach(x => x.RequestResponse = RequestDetail.REPONSE_REJECT);
        this.$tableView.refreshView();
        this.$.btnRejectAll.classList.add('active');
        this.$.btnApproveAll.classList.remove('active');
    }

    btnExpandDetailClick(e) {
        this.detailExpanded = !this.detailExpanded;
        this.render();
        this.$tableView.refreshView();
    }

    cboCategoryResourceChange(e) {
        let data = this.$dataResource.filter(x => x.ResourceCategoryName == this.$.cboCategoryResource.value);
        this.treeSelectResource.setData(data);
        this.render();
    }

    // editDetail(entity) {
    //     this.$entityDetail = entity;
    //     this.$.txtDetailName.value = entity.RequestDetailName;
    //     this.$.txtDetailDisplay.value = entity.RequestDetailDisplay;
    //     this.$.txtDetailDescription.value = entity.RequestDetailDesc;
    //     this.$$.formModalDetail.modal();
    //     this.$.txtDetailName.select();
    // }

    cboTypeChange(e) {
        this.cboTypeUpdateStyle();
        if (this.$entity.RequestID == 0 && this.requestPeopleToData != null) {
            this.generateDetailFromPeople(this.requestPeopleToData);
        }
    }
    cboTypeUpdateStyle() {
        let classes = '';
        if (this.$.cboType.value == Request.TYPE_ALTA)
            classes = 'request-alta';
        else if (this.$.cboType.value == Request.TYPE_MODIFICACION)
            classes = 'request-modificacion';
        else if (this.$.cboType.value == Request.TYPE_BAJA)
            classes = 'request-baja';
        this.$.cboType.className = 'custom-select ' + classes;
    }

    downloadFile(e) {
        var filename = e.target.dataset.filename;
        this.$presenter.showLoading();
        this.$parent.$resource.downloadFile(filename, this.formatNumber(this.$entity.RequestNumber, 8)).then(result => {
            const url = window.open("/api/AccessRequest/download/?FileName=" + filename + "&RequestNumber=" + this.formatNumber(this.$entity.RequestNumber, 8));  /*ASI DESCARGA PERO CON ERROR DENTRO DEL DOC */
            this.$presenter.hideLoading();
            iziToast.success({ title: 'Cargado correctamente' });
        },
            err => {
                this.$presenter.hideLoading();
                iziToast.error({ title: err }); //'Error al cargar'});
            });
    }

    validateFileExtension(fileInput) {
        var filePath = fileInput.value;
        var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.pdf|\.xls|\.xlsx|\.docx|\.pptx|\.rar|\.zip)$/i;
        if (!allowedExtensions.exec(filePath)) {
            this.$component.alert.show($.parseHTML("Por favor subir archivos solo con la siguiente extensión jpg, jpeg, png, pdf, xls, xlsx, docxs, pptx, rar, zip"));
            this.clearFileAttached();
        }
    }

    // People ------------------------------------------------------------------ //
    onPeopleSelect(e) {
        const people = e.detail;
        this.generateDetailFromPeople(people);
    }
    onPeopleSave(e) {
        const people = e.detail;
        this.generateDetailFromPeople(people);
    }

    get requestTypeData() {
        return this.$.cboType.options[this.$.cboType.selectedIndex].$data;
    }

    get requestPeopleToData() {
        return this.$.txtToName.$data;
    }
    set requestPeopleToData(people) {
        this.$.txtToName.$data = people;
    }

    // Detail -------------------------------------------------------------------------- //
    async generateDetailFromPeople(people) {
        this.$presenter.showLoading();
        this.requestPeopleToData = people;

        let entity = this.$entity;
        entity.setRequestTo(people);
        this.$.txtToName.value = entity.RequestToName;
        this.$.txtToProject.value = `${entity.RequestToNumDoc}/${entity.RequestToCompanyName}/${entity.RequestToProject}/${entity.RequestToPosition}`;
        this.$.txtToPosition.value = entity.RequestToPosition;
        this.$.txtToEmail.value = entity.RequestToEmail;
        this.$entity.RequestTo = people.PeopleID;
        this.$entity.AccessRequestDetails = [];

        await this.$parent.$resource.generateDetailFromPeople(people.PeopleID, this.requestTypeData.CommonValueName)
            .then(r => {
                let requestType = Array.from(this.$.cboType.options).find(x => x.$data.CommonValueName == r.requestType).$data;
                let detail = RequestDetail.fromList(r.data.filter(x => x.RequestDetPrevData));
                let requestCategory = r.data.map(x => x.ResourceCategoryName).filter((x, i, a) => a.indexOf(x) == i);

                //Se cargan las categorías
                this.resetCategoryResource();

                this.$dataResource = r.data;
                requestCategory.forEach(item => {
                    let opt = document.createElement('option');
                    opt.innerHTML = item;
                    opt.value = item;
                    opt.$data = item;
                    this.$.cboCategoryResource.appendChild(opt);
                });
                this.$.cboCategoryResource.value = null;

                // select type
                this.$.cboType.value = requestType.CommonValueID;
                this.cboTypeUpdateStyle();

                // table + treeSelect
                this.$tableView.load(detail);

                // if request BAJA: add to detail
                if (this.$.cboType.value == Request.TYPE_BAJA) {
                    this.$entity.AccessRequestDetails = detail;
                }
                // se resetean los controles de automatización sod
                this.clearSodAnalysis();
            })
            .catch(err => {
                iziToast.error({ title: err });
            });

        this.$presenter.hideLoading();
    }

    clearSodAnalysis() {
        this.conflictoDetectado = false;
        this.sodResources = [];
        this.$.txtNote.value = '';
        this.clearFileAttached();
        this.render();
    }

    clearFileAttached() {
        this.$.fileAttached.value = null;
        this.$.fileAttached.onchange();
    }
    // Actions ------------------------------------------------------------------------- //

    add(people) {
        this.$currentPeople = people;
        this.resetCategoryResource();
        let entity = new Request();
        entity.setRequestBy(people);
        this.edit(entity, RequestDetailTableView.MODE_EDIT);
    }

    async edit(entity, mode = RequestDetailTableView.MODE_VIEW) {
        this.$one(`#cboType option[value="${Request.TYPE_BAJA}"]`).hidden = entity.RequestID == 0 && SessionResource.session.UserRole5 == 0;

        this.mode = mode;
        this.treeSelectResource.setData(null);
        this.showNoteAdd = false;
        this.detailExpanded = false;
        this.requestPeopleToData = null;
        this.clearFileAttached();

        this.$entity = entity;
        this.$.cboType.value = entity.RequestType;
        this.$.cboPriority.value = entity.RequestPriority;
        this.$.txtByName.value = entity.RequestByName;
        this.$.txtByProject.value = entity.RequestByProject;
        this.$.txtByPosition.value = entity.RequestByPosition;
        this.$.txtToName.value = entity.RequestToName;
        this.$.txtToProject.value = `${entity.RequestToNumDoc}/${entity.RequestToCompanyName}/${entity.RequestToProject}/${entity.RequestToPosition}`;
        this.$.txtToPosition.value = entity.RequestToPosition;
        this.$.txtToEmail.value = entity.RequestToEmail;
        this.$.txtNote.value = entity.RequestID > 0 ? ((entity.RequestNote || '').trim() != '' ? entity.RequestNote : '<Sin observaciones>') : ''; //this.mode == RequestDetailTableView.MODE_VIEW ? entity.RequestNote : '';
        this.$.txtNoteAdd.value = '';
        this.$.txtTicket.value = entity.AttentionTicket;
        this.$.txtOracleUser.value = entity.OracleUser;
        this.$.txtOracleMenu.value = entity.OracleMenu;

        this.disabledApproverButtons(entity);

        this.$tableView.buildTable(mode);
        this.$tableView.load(entity.AccessRequestDetails);
        this.cboTypeUpdateStyle();
        this.$.btnApproveAll.classList.remove('active');
        this.$.btnRejectAll.classList.remove('active');


        this.render();

        // show modal
        this.$$.formModal.modal({ backdrop: 'static' });
        this.$tableView.refreshView();
        this.$presenter.hideLoading();
        this.showApproverWarning(entity, mode);
    }

    disabledApproverButtons(entity) {
        let buttons = document.querySelectorAll('.approve-button');

        buttons.forEach(b => {
            b.disabled = entity.FlagIsApprover === 1;
        });
    }

    showApproverWarning(entity, type) {
        if (entity.FlagIsApprover == 1 && type == RequestDetailTableView.MODE_APPROVE) {
            iziToast.warning({
                title: 'Usted no puede aprobar esta solicitud',
                timeout: 10000
            });
        }
    }

    callConfirm(e) {
        e.preventDefault();
        if (this.$entity.RequestID > 0)
            return;

        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            return;
        };

        if (this.$entity.AccessRequestDetails.length == 0) {
            iziToast.error({ title: 'No ha ingresado el detalle' });
            return;
        }

        if (this.$.cboType.value == Request.TYPE_BAJA) {
            if (this.$entity.AccessRequestDetails.some(x => !!x.RequestDetPending)) {
                iziToast.error({ title: 'Tiene detalles pendientes.' });
                return;
            }
        }

        // valida modificaciones
        let modifyItem = this.$entity.AccessRequestDetails
            .sort((a, b) => a.ResourceFullName.localeCompare(b.ResourceFullName))
            .filter(x => x.RequestDetType == Request.TYPE_MODIFICACION)
            .find(x => {
                let dataResource = RequestDetail.fromObject(this.$dataResource.find(dr => dr.ResourceID == x.ResourceID));
                let DetailValidityFrom = 0;
                let DetailValidityUntil = 0;
                let ValidityFrom = 0;
                let ValidityUntil = 0;

                if (x.ReqDetValidityFrom != null)
                    DetailValidityFrom = moment(x.ReqDetValidityFrom).startOf('date').toDate().getTime();
                if (x.ReqDetValidityUntil != null)
                    DetailValidityUntil = moment(x.ReqDetValidityUntil).startOf('date').toDate().getTime();
                if (dataResource.ReqDetValidityFrom != null)
                    ValidityFrom = moment(dataResource.ReqDetValidityFrom).startOf('date').toDate().getTime();
                if (dataResource.ReqDetValidityUntil != null)
                    ValidityUntil = moment(dataResource.ReqDetValidityUntil).startOf('date').toDate().getTime();

                return (
                    x.RequestDetIntValue == dataResource.RequestDetIntValue &&
                    x.RequestDetStrValue == dataResource.RequestDetStrValue &&
                    x.ReqDetTemporal == dataResource.ReqDetTemporal &&
                    DetailValidityFrom == ValidityFrom &&
                    DetailValidityUntil == ValidityUntil
                );
            });
        if (modifyItem != undefined) {
            iziToast.error({ title: `Tiene detalles sin modificar. '${modifyItem.ResourceFullName}'` });
            return;
        }

        this.$$.formModal.addClass('modal-opened');
        this.$component.requestConfirm.show(this.$entity);
    }

    showDetailApplicant(e) {
        e.preventDefault();
        this.$$.formModal.addClass('modal-opened');
        this.$component.detailApplicant.show(this.$entity, this.requestTypeData.CommonValueName);
    }

    onChangeFile(e) {
        var sizeFile = ((this.$.fileAttached.files[0].size) / 1000) / 1000;
        if (sizeFile > 10) {
            this.$component.alert.show($.parseHTML("Se exedió el tamaño máximo permitido: 10 mb"));
            this.clearFileAttached();
        } else {
            this.validateFileExtension(this.$.fileAttached);
        }
    }

    async save() {
        this.$entity.RequestType = this.$.cboType.value;
        this.$entity.RequestPriority = this.$.cboPriority.value;
        this.$entity.RequestNote = this.$.txtNote.value;
        this.$entity.AccessRequestDetails.map(x => {
            x.RequestDetDisplayValue = null;
            return x;
        })


        let formData = new FormData();
        if (this.conflictoDetectado) {
            const file = await this.attachActaPDF();
            formData.append('file', file);
        } else {
            formData.append('file', this.$.fileAttached.files[0]);
        }        
        formData.append('entity', JSON.stringify(this.$entity));

        this.$presenter.showLoading();
        this.$parent.$resource.post(formData).then(
            r => {
                this.$$.formModal.modal('hide');
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
                this.$parent.loadData();
            },
            err => {
                this.$presenter.hideLoading();
                console.error('error!', err);
                iziToast.error({ title: err }); //'Error al grabar'});                
            }
        );
    }

    approve(e) {
        e.preventDefault();
        if (this.$entity.AccessRequestDetails.some(x => x.RequestResponse == null)) {
            iziToast.error({ title: 'Tiene detalles sin aprobar/rechazar.' });
            return;
        }
        if (this.$entity.AccessRequestDetails.some(x => x.RequestResponse > 0) || this.$entity.AccessRequestDetails.some(x => x.RequestResponse == 0)) {
            // if (this.$entity.RequestStatus == Request.STATUS_PENDIENTE) {
            this.$presenter.question(
                ' ',
                'Desea guardar los cambios en la solicitud?',
                (instance, toast) => {
                    this.$presenter.showLoading();
                    let formData = new FormData();
                    formData.append('file', this.$.fileAttached.files[0]);
                    formData.append('entity', JSON.stringify({
                        RequestID: this.$entity.RequestID,
                        RequestNote: this.$.txtNoteAdd.value.trim(),
                        AttentionTicket: (this.$.txtTicket.value == "" ? null : this.$.txtTicket.value.trim()),
                        OracleUser: (this.$.txtOracleUser.value == "" ? null : this.$.txtOracleUser.value.trim()),
                        OracleMenu: (this.$.txtOracleMenu.value == "" ? null : this.$.txtOracleMenu.value.trim()),
                        ApproveDetail: this.$entity.AccessRequestDetails
                    }));

                    this.$presenter.showLoading();
                    this.$parent.$resource.approve(formData).then(r => {
                        this.$$.formModal.modal('hide');
                        this.$presenter.hideLoading();
                        console.log('grabado!');
                        iziToast.success({ title: 'Grabado correctamente' });
                        this.$parent.loadData();
                    }).catch(err => {
                        this.$presenter.hideLoading();
                        console.error('error!', err);
                        iziToast.error({ title: err }); //'Error al grabar'});                
                    });
                });
        }
        // Rechazar
        //if (this.$entity.AccessRequestDetails.some(x => x.RequestResponse == 0)) {
        //    // if (this.$entity.RequestStatus == Request.STATUS_PENDIENTE) {
        //    this.$presenter.question(
        //        'Rechazar solicitud',
        //        'Desea rechazar esta solicitud?',
        //        (instance, toast) => {
        //            this.$presenter.showLoading();
        //            let formData = new FormData();
        //            formData.append('file', this.$.fileAttached.files[0]);
        //            formData.append('entity', JSON.stringify({
        //                RequestID: this.$entity.RequestID,
        //                RequestNote: this.$.txtNoteAdd.value.trim(),
        //                AttentionTicket: (this.$.txtTicket.value == "" ? null : this.$.txtTicket.value.trim()),
        //                OracleUser: (this.$.txtOracleUser.value == "" ? null : this.$.txtOracleUser.value.trim()),
        //                OracleMenu: (this.$.txtOracleMenu.value == "" ? null : this.$.txtOracleMenu.value.trim()),
        //                ApproveDetail: this.$entity.AccessRequestDetails
        //            }));

        //            this.$presenter.showLoading();
        //            this.$parent.$resource.approve(formData).then(r => {
        //                this.$$.formModal.modal('hide');
        //                this.$presenter.hideLoading();
        //                console.log('grabado!');
        //                iziToast.success({ title: 'Grabado correctamente' });
        //                this.$parent.loadData();
        //            }).catch(err => {
        //                this.$presenter.hideLoading();
        //                console.error('error!', err);
        //                iziToast.error({ title: err }); //'Error al grabar'});                
        //            });
        //        });
        //}

    }

    Annul(e) {
        if (this.$entity.RequestID > 0) {
            // if (this.$entity.RequestStatus == Request.STATUS_PENDIENTE) {
            this.$presenter.question(
                'Anular solicitud',
                'Desea anular esta solicitud?',
                (instance, toast) => {
                    this.$presenter.showLoading();
                    this.$parent.$resource.annul(this.$entity.RequestID).then(
                        r => {
                            this.$$.formModal.modal('hide');
                            this.$presenter.hideLoading();
                            console.log('anulado!');
                            iziToast.success({ title: 'Anulado correctamente' });
                            this.$parent.loadData();
                        },
                        err => {
                            this.$presenter.hideLoading();
                            console.error('error!', err);
                            iziToast.error({ title: 'Error al anular' });
                        }
                    );
                }
            );
            // }
        }




    }

    formatNumber(number, width = 8) {
        return new Array(width - number.toString().length + 1).join('0') + number;
    }
    iconStatus(entity) {
        if (entity.RequestID == 0)
            return html`<i class="fas fa-plus"></i>`;
        else if (entity.RequestStatus == Request.STATUS_ANULADO)
            return html`<i class="fas fa-ban"></i>`;
        else if (entity.RequestStatus == Request.STATUS_APROBADO)
            return html`<i class="fas fa-check"></i>`;
        else if (entity.RequestStatus == Request.STATUS_ATENDIDO)
            return html`<i class="fas fa-hand-holding"></i>`;
        else if (entity.RequestStatus == Request.STATUS_EN_PROCESO)
            return html`<i class="fas fa-clock"></i>`;
        else if (entity.RequestStatus == Request.STATUS_INDEFINIDO)
            return html`<i class="fas fa-question"></i>`;
        else if (entity.RequestStatus == Request.STATUS_PARCIAL)
            return html`<i class="fas fa-hourglass-half"></i>`;
        else if (entity.RequestStatus == Request.STATUS_PENDIENTE)
            return html`<i class="fas fa-clock"></i>`;
        else if (entity.RequestStatus == Request.STATUS_RECHAZADO)
            return html`<i class="fas fa-times"></i>`;
        else if (entity.RequestStatus == Request.STATUS_EXPIRADO)
            return html`<i class="fas fa-clock"></i>`;
    }

    resetCategoryResource() {
        // Se cargan las categorías
        let select = this.$.cboCategoryResource;
        select.innerHTML = "";
        let opt = document.createElement('option');
        opt.innerHTML = "Seleccione categoría";
        opt.value = null;
        opt.$data = null;
        select.appendChild(opt);

        // Se limpia el tree control
        this.cboCategoryResourceChange(select);
    }

    closeModalRequestConfirm() {
        this.$$.formModal.removeClass('modal-opened');
        // this.$$.formModal.removeClass('modal-applicant');
        $('body').addClass('modal-open');
    }
}



export class RequestDetailTableView {
    static get MODE_VIEW() { return 'MODE_VIEW' }
    static get MODE_EDIT() { return 'MODE_EDIT' }
    static get MODE_APPROVE() { return 'MODE_APPROVE' }

    constructor(component, table) {
        this.$component = component;
        this.$table = table;
        this.type = RequestDetailTableView.TYPE_VIEW;
        this.$table.bootstrapTable();
    }

    buildTable(type) {
        let $this = this;
        if (type == RequestDetailTableView.MODE_VIEW) {
            this.type = type;
            this.$table.bootstrapTable('refreshOptions', {
                data: [],
                columns: [
                    { title: "Categoría", field: "ResourceCategoryDisplay", align: "left", valign: "middle", width: 200 },
                    { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterRecurso },
                    { title: "Valor", field: "RequestDetDisplayValue", align: "left", valign: "middle", formatter: $this.tableFormatterValor },
                    { title: "Tipo", field: "RequestDetTypeDisplay", align: "center", valign: "middle", cellStyle: $this.tableCellStyleType },
                    { title: "Vigencia", field: "RequestDetValidityDisplay", align: "left", valign: "middle" },
                    { title: "Estado", field: "RequestDetStatusName", align: "right", valign: "middle" },
                    { title: "", field: "RequestDetStatus", align: "center", valign: "middle", class: "", formatter: $this.tableFormatterStatus },
                    { title: "", field: "", align: "center", valign: "middle", width: 90, formatter: $this.tableFormatterHistory, events: $this.tableEventColumnHistory }
                ],
                rowStyle: null,
                onPostBody: (data) => { },
                onDblClickRow: (row, element, field) => {
                    if (row.RequestDetID != null || row.RequestDetID > 0) {
                        this.$component.$component.requestDetailHistory.show(this.$component.$entity, row);
                    }
                }
            });
        } else if (type == RequestDetailTableView.MODE_EDIT) {
            this.type = type;
            this.$table.bootstrapTable('refreshOptions', {
                data: [],
                columns: [
                    { title: "", field: "RequestDetPrevData", align: "center", valign: "middle", width: 30, cellStyle: $this.tableCellStyleCurrentResource, formatter: $this.tableFormatterCurrentResource },
                    { title: "Categoría", field: "ResourceCategoryDisplay", align: "left", valign: "middle", width: 200, class: "" },
                    { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", formatter: $this.tableFormatterRecurso },
                    { title: "Valor", field: "RequestDetDisplayValue", align: "left", valign: "middle", formatter: $this.tableFormatterValue.bind($this), events: $this.tableEventValueOperate },
                    { title: "Vigencia", field: "ReqDetTemporal", align: "left", valign: "middle", width: 350, formatter: $this.tableFormatterValidity.bind($this), events: $this.tableEventColumnOperate },
                    { title: "", field: "", align: "center", valign: "middle", width: 100, class: "", formatter: $this.tableFormatterOperate.bind($this), events: $this.tableEventColumnOperate }
                ],
                rowStyle: $this.rowStyle,
                onPostBody: (data) => {
                    $('.selectpicker').selectpicker({
                        noneResultsText: 'No se encontaron resultados para {0}',
                        noneSelectedText: 'Nada seleccionado',
                        size: 4
                    });
                    $('.date-request').datetimepicker({
                        format: 'DD/MM/YYYY',
                        widgetPositioning: {
                            horizontal: 'left',
                            vertical: 'bottom'
                        }
                    });

                    data.forEach((x, index) => {
                        let select = $(`#tableDetail tbody`).find(`select.input-select-simple[data-index='${index}']`);
                        select.val(x.RequestDetIntValue);

                        if ((x.ResourceTemporal == 1 || x.ReqDetTemporal > 0) &&
                            ([Request.TYPE_ALTA, Request.TYPE_MODIFICACION].includes(x.RequestDetType))) {
                            let dateStart = $(`#tableDetail tbody`).find(`div.date-request[data-index='${index}'][data-type="start"]`);
                            let dateEnd = $(`#tableDetail tbody`).find(`div.date-request[data-index='${index}'][data-type="end"]`);

                            dateStart.data("DateTimePicker").minDate(moment().startOf('day'));
                            dateStart.on('dp.change', (e) => {
                                console.log(e.date._d);
                                if (e.date._d == null) {
                                    dateEnd.data("DateTimePicker").minDate(null);
                                } else {
                                    dateEnd.data("DateTimePicker").minDate(new Date(e.date._d));
                                }
                                this.$component.$entity.AccessRequestDetails.find(y => y.ResourceID == x.ResourceID).ReqDetValidityFrom = e.date._d;
                            });

                            dateEnd.on('dp.change', (e) => {
                                console.log(e.date._d);
                                if (e.date._d == null) {
                                    dateStart.data("DateTimePicker").maxDate(null);
                                } else {
                                    dateStart.data("DateTimePicker").maxDate(new Date(e.date._d));
                                }
                                this.$component.$entity.AccessRequestDetails.find(y => y.ResourceID == x.ResourceID).ReqDetValidityUntil = e.date._d;
                            });

                            let resourcetemp = this.$component.$entity.AccessRequestDetails.find(y => y.ResourceID == x.ResourceID);
                            if (resourcetemp != undefined) {
                                if (resourcetemp.ReqDetValidityFrom != null) {
                                    dateStart.data("DateTimePicker").date(moment(resourcetemp.ReqDetValidityFrom).format('DD/MM/YYYY HH:mm'));
                                }
                                if (resourcetemp.ReqDetValidityUntil != null) {
                                    dateEnd.data("DateTimePicker").date(moment(resourcetemp.ReqDetValidityUntil).format('DD/MM/YYYY HH:mm'));
                                }
                            }
                        }

                        if ([AccessType.TYPE_SELECT_SIMPLE, AccessType.TYPE_SELECT_MULTIPLE].includes(x.ResourceAccessTypeType)) {
                            let selectMultiple = $(`#tableDetail tbody`).find(`.selectpicker[data-index='${index}']`);
                            // if (x.ResourceAccessTypeType == AccessType.TYPE_SELECT_SIMPLE) {
                            //     $(selectMultiple).selectpicker('val', x.RequestDetIntValue);
                            // } else
                            if (x.ResourceAccessTypeType == AccessType.TYPE_SELECT_MULTIPLE) {
                                $(selectMultiple).selectpicker('val', (x.RequestDetStrValue || '').split(','));
                            }
                            $(selectMultiple).on('changed.bs.select', (e, clickedIndex, isSelected, previousValue) => {
                                if (selectMultiple.required) {
                                    if (this.value != null) {
                                        $(selectMultiple).selectpicker('setStyle', 'is-invalid', 'remove');
                                        $(selectMultiple).selectpicker('setStyle', 'is-valid', 'add');
                                    } else {
                                        $(selectMultiple).selectpicker('setStyle', 'is-valid', 'remove');
                                        $(selectMultiple).selectpicker('setStyle', 'is-invalid', 'add');
                                    }
                                } else {
                                    $(selectMultiple).selectpicker('setStyle', 'is-invalid', 'remove');
                                    $(selectMultiple).selectpicker('setStyle', 'is-valid', 'add');
                                }

                                let selected = Array.from(e.currentTarget.selectedOptions);
                                let detail = this.$component.$entity.AccessRequestDetails.find(y => y.ResourceID == x.ResourceID);
                                detail.RequestDetStrValue = selected.map(s => s.value).join(',');
                                detail.RequestDetDisplayValue = selected.map(s => s.text.trim().substring(0, 55)).join('</br>').trim();
                            });
                        }
                    });
                }
            });
        } else if (type == RequestDetailTableView.MODE_APPROVE) {
            this.$table.bootstrapTable('refreshOptions', {
                data: [],
                columns: [
                    { title: "Categoría", field: "ResourceCategoryDisplay", align: "left", valign: "middle", width: 200, class: "" },
                    { title: "Recurso", field: "ResourceFullName", align: "left", valign: "middle", width: 200, formatter: $this.tableFormatterRecurso },
                    { title: "Valor", field: "RequestDetDisplayValue", align: "left", valign: "middle", formatter: $this.tableFormatterValor },
                    { title: "Vigencia", field: "ReqDetTemporal", align: "center", valign: "middle", width: 180, formatter: $this.tableFormatterValidityApprove.bind($this) },
                    { title: "Tipo", field: "RequestDetTypeDisplay", align: "center", valign: "middle", width: 120, cellStyle: $this.tableCellStyleType },
                    { title: "", field: "RequestDetID", align: "center", valign: "middle", width: 200, formatter: $this.tableFormatterApprove, events: $this.tableEventColumnApprove }
                ],
                onDblClickRow: (row, element, field) => {
                    if (row.RequestDetID != null || row.RequestDetID > 0) {
                        this.$component.$component.requestDetailHistory.show(this.$component.$entity, row);
                    }
                }
            });
        }
    }

    rowStyle(row, index) {
        if (row.RequestDetType == Request.TYPE_BAJA) {
            return {
                css: {
                    color: '#F55'
                }
            }
        } else if (row.RequestDetType == Request.TYPE_MODIFICACION) {
            return {
                css: {
                    color: 'orange'
                }
            }
        } else if (row.RequestDetType == Request.TYPE_ALTA) {
            return {
                css: {
                    color: '#04B404'
                }
            }
        } else if (row.RequestDetType == null && row.RequestDetPending == true) {
            return {
                css: {
                    color: '#007bff'
                }
            }
        }
        return {
            css: {}
        }
    }

    tableCellStyleType(value, row, index, field) {
        let classes = '';
        if (row.RequestDetType == Request.TYPE_ALTA)
            classes = 'request-alta';
        else if (row.RequestDetType == Request.TYPE_MODIFICACION)
            classes = 'request-modificacion';
        else if (row.RequestDetType == Request.TYPE_BAJA)
            classes = 'request-baja';
        return {
            classes: classes,
            css: {}
        };
    }

    tableCellStyleCurrentResource(value, row, index, field) {
        let classes = '';
        if (!!value) {
            if (row.RequestDetType == null && row.RequestDetPending == false)
                classes = 'current-resource';
            else if (row.RequestDetType == null && row.RequestDetPending == true)
                classes = 'current-resource-pending';
            else if (row.RequestDetType == Request.TYPE_MODIFICACION)
                classes = 'current-resource-modify';
            else if (row.RequestDetType == Request.TYPE_BAJA)
                classes = 'current-resource-delete';
        } else {
            if (row.RequestDetType == Request.TYPE_ALTA)
                classes = 'current-resource-add';
        }
        return {
            classes: classes,
            css: {}
        };
    }
    tableFormatterCurrentResource(value, row, index) {
        if (!!value && row.RequestDetType == null && row.RequestDetPending == false)
            return '<i class="fas fa-equals" title="Acceso actual"></i>';
        else if (!!value && row.RequestDetType == null && row.RequestDetPending == true)
            return '<i class="fas fa-clock" title="Acceso pendiente"></i>';
        else if (row.RequestDetType == Request.TYPE_ALTA)
            return '<i class="fas fa-plus" title="Acceso nuevo"></i>';
        else if (row.RequestDetType == Request.TYPE_MODIFICACION)
            return '<i class="fas fa-edit" title="Acceso modificado"></i>';
        else if (row.RequestDetType == Request.TYPE_BAJA)
            return '<i class="fas fa-minus" title="Acceso a dar de baja"></i>';
    }

    tableFormatterRecurso(value, row, index) {
        return `<strong>${row.ResourceName}</strong><br/><small>${row.ResourceFullName}</small>`;
    }

    tableFormatterValor(value, row, index) {
        if (row.RequestDetDisplayValue != null) {
            var array = row.RequestDetDisplayValue.split(',');
            if (array.length > 1) {
                return array.join("</br>");
            } else {
                return row.RequestDetDisplayValue;
            }
        } else {
            return row.RequestDetDisplayValue;
        }
    }

    tableFormatterValue(value, row, index) {
        if ([Request.TYPE_ALTA, Request.TYPE_MODIFICACION].includes(row.RequestDetType)) {
            if (row.ResourceAccessTypeType == AccessType.TYPE_TEXTO) {
                return `<input type="text" value="${row.RequestDetStrValue || ''}" class="form-control input-text" min="3" placeholder="Ingrese un texto" required />`;
            } else if (row.ResourceAccessTypeType == AccessType.TYPE_SELECT_SIMPLE) {
                return `<select class="custom-select input-select-simple" data-index="${index}" required>
                        ${row.ResourceAccessTypeValues.map(x => `<option value="${x.TypeValueID}" ${row.RequestDetIntValue == x.TypeValueID ? 'selected' : ''}>${x.TypeValueDisplay}</option>`).join('')}
                    </select>`;
            } else if (row.ResourceAccessTypeType == AccessType.TYPE_SELECT_MULTIPLE) {
                return `<select class="selectpicker" data-index="${index}" multiple data-live-search="true" required>
                            ${row.ResourceAccessTypeValues.map(x => `<option value="${x.TypeValueID}">${x.TypeValueDisplay}</option>`).join('')}
                        </select>`;
            } else if (row.ResourceAccessTypeType == AccessType.TYPE_ENTERPRISE_EMAIL) {
                return `<input type="email" value="${row.RequestDetStrValue || ''}" class="form-control input-email" placeholder="ejemplo: admin@gym.com" required />`;
            }
        } else {
            return this.tableFormatterValor(value, row, index);
        }
    }

    tableFormatterStatus(value, row, index) {
        if (value == Request.STATUS_ANULADO)
            return `<i class="fas fa-ban"></i>`;
        else if (value == Request.STATUS_APROBADO)
            return `<i class="fas fa-check"></i>`;
        else if (value == Request.STATUS_ATENDIDO)
            return `<i class="fas fa-hand-holding"></i>`;
        else if (value == Request.STATUS_EN_PROCESO)
            return `<i class="fas fa-clock"></i>`;
        else if (value == Request.STATUS_INDEFINIDO)
            return `<i class="fas fa-question"></i>`;
        else if (value == Request.STATUS_PARCIAL)
            return `<i class="fas fa-hourglass-half"></i>`;
        else if (value == Request.STATUS_PENDIENTE)
            return `<i class="fas fa-clock"></i>`;
        else if (value == Request.STATUS_RECHAZADO)
            return `<i class="fas fa-times"></i>`;
        else if (value == Request.STATUS_EXPIRADO)
            return `<i class="fas fa-clock"></i>`;
    }

    tableFormatterOperate(value, row, index) {
        let buttons = '';

        if (this.$component.$.cboType.value != Request.TYPE_BAJA) { // Si es ALTA, MODIFICACION
            if (!!row.RequestDetPrevData) {  // Si ya tiene acceso a este recurso
                if (row.RequestDetType == null && row.RequestDetPending == false) {
                    if (row.ResourceAccessTypeType != AccessType.TYPE_SIMPLE || row.ReqDetTemporal > 0) {
                        buttons = `<button type="button" class="btn btn-outline-secondary btn-modify" title="Modificar"><i class="fas fa-edit"></i></button>`;
                    }
                    if (SessionResource.session.UserRole5 == 1) { // si tiene permiso de dar de baja
                        buttons += `<button type="button" class="btn btn-outline-danger btn-delete" title="Dar de Baja">Baja <i class="fas fa-minus"></i></button>`;
                    }
                }
                else if (row.RequestDetType == Request.TYPE_MODIFICACION)
                    buttons = `<button type="button" class="btn btn-outline-secondary btn-modify-cancel" title="Cancelar edición">Cancelar <i class="fas fa-ban"></i></button>`;
                else if (row.RequestDetType == Request.TYPE_BAJA)
                    buttons = `<button type="button" class="btn btn-outline-secondary btn-delete-remove" title="Cancelar la Baja">Cancelar <i class="fas fa-ban"></i></button>`;
            } else {
                if (row.RequestDetType == Request.TYPE_ALTA)
                    buttons = `<button type="button" class="btn btn-outline-danger btn-delete-added" title="Quitar">Quitar <i class="fas fa-times"></i></button>`;
            }
        }

        return `<div class="btn-group btn-group-sm" role="group" aria-label="Basic example" style="width:90px">
                    ${buttons}
                </div>`;
    }

    tableFormatterApprove(value, row, index) {
        return `<div class="btn-group btn-group-sm check-approve" data-toggle="buttons">
                    <button class="btn btn-outline-success btn-accept ${row.RequestResponse == RequestDetail.REPONSE_ACCEPT ? 'active' : ''}" title="Aprobar"  type="button" ${row.FlagIsApprover == 1 ? 'disabled':''}>
                        <input type="radio" name="options-${row.RequestDetID}" value="1" autocomplete="off" />
                        <i class="fas fa-check fa-sm btn-approve"></i>
                    </button>
                    <button class="btn btn-outline-danger btn-reject ${row.RequestResponse == RequestDetail.REPONSE_REJECT ? 'active' : ''}" title="Rechazar" type="button" ${row.FlagIsApprover == 1 ? 'disabled' : ''}>
                        <input type="radio" name="options-${row.RequestDetID}" value="0" autocomplete="off" />
                        <i class="fas fa-times fa-sm btn-approve"></i>
                    </button>
                </div>
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-outline-primary btn-history" title="Historial">
                        <i class="fas fa-history"></i> Historial
                    </button>
                </div>`;
    }

    tableFormatterValidity(value, row, index) {
        if (row.ResourceTemporal > 0 || row.ReqDetTemporal > 0) {
            if ([Request.TYPE_ALTA, Request.TYPE_MODIFICACION].includes(row.RequestDetType)) {
                return `
                <div class="row">
                    <div class="col-sm-2" style="padding-right: 0px;">
                        <div class="input-group-prepend">
                            <div class="input-group-text my-1 ml-2">
                                ${row.ResourceTemporal == Resource.TEMPORAL_OPTIONAL ?
                        `<input type="checkbox" class="chk-temporal" ${row.ReqDetTemporal > 0 ? 'checked' : ''}>` :
                        `<input type="checkbox" disabled checked>`}
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-5" style="padding-right: 0px;">
                        <div class='input-group date date-request' data-control="datepicker" data-index="${index}" data-type="start" data-field="ReqDetValidityFrom" data-resourceid="${row.ResourceID}">
                            <input type='text' class="form-control" required
                                ${row.ResourceTemporal == Resource.TEMPORAL_OPTIONAL && row.ReqDetTemporal == 0 ? 'disabled' : ''}
                            />
                            <span class="input-group-addon">
                                <span><i class="fa fa-calendar btn btn-outline-secondary" aria-hidden="true"></i></span>
                            </span>
                        </div>
                    </div>
                    <div class="col-sm-5" style="padding-left: 3px;">
                        <div class='input-group date date-request' data-control="datepicker" data-index="${index}" data-type="end" data-field="ReqDetValidityUntil" data-resourceid="${row.ResourceID}">
                            <input type='text' class="form-control" required
                                ${row.ResourceTemporal == Resource.TEMPORAL_OPTIONAL && row.ReqDetTemporal == 0 ? 'disabled' : ''}
                            />
                            <span class="input-group-addon">
                                <span><i class="fa fa-calendar btn btn-outline-secondary" aria-hidden="true"></i></span>
                            </span>
                        </div>
                    </div>
                </div>`;
            } else {
                const validityFrom = row.ReqDetValidityFrom? moment(row.ReqDetValidityFrom).format('DD/MM/YYYY') : '';
                const validityUntil = row.ReqDetValidityUntil? moment(row.ReqDetValidityUntil).format('DD/MM/YYYY') : '';
                return `${validityFrom} - ${validityUntil}`.trim();
            }
        }
    }
    tableFormatterValidityApprove(value, row, index) {
        if (row.ReqDetTemporal > 0) {
            const validityFrom = row.ReqDetValidityFrom? moment(row.ReqDetValidityFrom).format('DD/MM/YYYY') : '';
            const validityUntil = row.ReqDetValidityUntil? moment(row.ReqDetValidityUntil).format('DD/MM/YYYY') : '';
            return `${validityFrom} - ${validityUntil}`.trim();
        }
    }

    tableFormatterHistory(value, row, index) {
        return `<div class="btn-group btn-group-sm" role="group" aria-label="Historial">
                    <button type="button" class="btn btn-outline-primary btn-history" title="Historial"><i class="fas fa-history"></i> Historial</button>
                </div>`;
    }

    // Table Events Resource Value ------------------------------------------------------------------ //
    get tableEventValueOperate() {
        let $this = this;
        return {//aqui
            // 'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'input .input-text': $this.inputResourceValueText.bind($this),
            'input .input-email': $this.inputResourceValueText.bind($this),
            'input .input-select-simple': $this.inputResourceValueSelectSimple.bind($this)
            // 'input .selectpicker': $this.inputResourceValueSelectMultiple.bind($this)
        }
    }
    inputResourceValueText(e, value, row, index) {
        row.RequestDetStrValue = e.currentTarget.value.trim();
        let detail = this.$component.$entity.AccessRequestDetails.find(x => x.ResourceID == row.ResourceID);
        detail.RequestDetStrValue = e.currentTarget.value.trim();
        detail.RequestDetDisplayValue = e.currentTarget.value.trim();
    }
    inputResourceValueSelectSimple(e, value, row, index) {
        row.RequestDetIntValue = parseInt(e.currentTarget.value);
        let detail = this.$component.$entity.AccessRequestDetails.find(x => x.ResourceID == row.ResourceID);
        detail.RequestDetIntValue = parseInt(e.currentTarget.value);
        detail.RequestDetDisplayValue = e.currentTarget.selectedOptions[0].text;
    }
    inputResourceValueSelectMultiple(e, value, row, index) {
        let selected = Array.from(e.currentTarget.selectedOptions);
        let detail = this.$component.$entity.AccessRequestDetails.find(x => x.ResourceID == row.ResourceID);
        detail.RequestDetStrValue = selected.map(s => s.value).join(',');
        detail.RequestDetDisplayValue = selected.map(s => s.text.trim()).join(',');
    }

    // Table Events ------------------------------------------------------------------ //
    get tableEventColumnOperate() {
        let $this = this;
        return {
            // 'click .btn-edit': $this.btnColumnEditClick.bind($this),
            'click .chk-temporal': $this.btnColumnTemporalClick.bind($this),
            'click .btn-modify': $this.btnColumnModifyClick.bind($this),
            'click .btn-modify-cancel': $this.btnColumnModifyCancelClick.bind($this),
            'click .btn-delete': $this.btnColumnDeleteClick.bind($this),
            'click .btn-delete-remove': $this.btnColumnDeleteRemoveClick.bind($this),
            'click .btn-delete-added': $this.btnColumnDeleteAddedClick.bind($this)
        }
    }

    // btnColumnEditClick(e, value, row, index) {
    // let entity = new RequestDetail();
    // entity.fromDTO(row);
    // this.editDetail(row);
    // }
    btnColumnTemporalClick(e, value, row, index) {
        row.ReqDetTemporal = 1 - row.ReqDetTemporal;
        let dateStart = $(`div.date-request[data-control="datepicker"][data-index='${index}'][data-type="start"]`);
        let dateEnd = $(`div.date-request[data-control="datepicker"][data-index='${index}'][data-type="end"]`);
        dateEnd.data("DateTimePicker").date(null);
        dateStart.data("DateTimePicker").date(null);
        this.$table.bootstrapTable('updateRow', { index, row });
    }
    btnColumnModifyClick(e, value, row, index) {
        row.RequestDetType = Request.TYPE_MODIFICACION;
        this.$component.$entity.AccessRequestDetails.push(row);  //Agregamos el recurso de la lista ($entity.AccessRequestDetails)
        this.$table.bootstrapTable('updateRow', { index, row });
    }
    btnColumnModifyCancelClick(e, value, row, index) {
        //row.RequestDetType = null;
        let data = this.$component.$dataResource.find(r => r.ResourceID == row.ResourceID);  // Se revierten los cambios obteniendo el registro desde la caché
        row = RequestDetail.fromObject(data);
        this.$table.bootstrapTable('updateRow', { index, row });

        //Quitamos el recurso de la lista ($entity.AccessRequestDetails)
        let indexData = this.$component.$entity.AccessRequestDetails.findIndex(x => x.ResourceID == row.ResourceID);
        this.$component.$entity.AccessRequestDetails.splice(indexData, 1);
    }
    btnColumnDeleteClick(e, value, row, index) {
        row.RequestDetType = Request.TYPE_BAJA;
        this.$component.$entity.AccessRequestDetails.push(row);  //Agregamos el recurso de la lista ($entity.AccessRequestDetails)
        this.$table.bootstrapTable('updateRow', { index, row });
    }
    btnColumnDeleteRemoveClick(e, value, row, index) {
        row.RequestDetType = null;
        this.$table.bootstrapTable('updateRow', { index, row });

        //Quitamos el recurso de la lista ($entity.AccessRequestDetails)
        let indexData = this.$component.$entity.AccessRequestDetails.findIndex(x => x.ResourceID == row.ResourceID);
        this.$component.$entity.AccessRequestDetails.splice(indexData, 1);
    }
    btnColumnDeleteAddedClick(e, value, row, index) {
        let data = this.getData();
        let dataIndex = data.findIndex(x => x.ResourceID == row.ResourceID);
        data.splice(dataIndex, 1);

        if (row.ResourceParamValue != null || row.ResourceParamValue != '') {
            let sodIndex = this.$component.sodResources.findIndex(x => x.ResourceID == row.ResourceID)
            this.$component.sodResources.splice(sodIndex, 1);

            this.$component.prepareSodMatrix(data, this.$component.sodResources);
        }

        //Quitamos el recurso de la lista ($entity.AccessRequestDetails)
        let indexData = this.$component.$entity.AccessRequestDetails.findIndex(x => x.ResourceID == row.ResourceID);
        this.$component.$entity.AccessRequestDetails.splice(indexData, 1);

        this.load(data);
    }

    // Table Events Approve ------------------------------------------------------------------ //
    get tableEventColumnApprove() {

        let $this = this;
        return {
            'click .btn-accept': $this.btnColumnAcceptClick.bind($this),
            'click .btn-reject': $this.btnColumnRejectClick.bind($this),
            'click .btn-history': $this.btnColumnHistoryClick.bind($this.$component)
        }
    }

    btnColumnAcceptClick(e, value, row, index) {


        row.RequestResponse = RequestDetail.REPONSE_ACCEPT;
        if (this.$component.$entity.AccessRequestDetails.every(x => x.RequestResponse == RequestDetail.REPONSE_ACCEPT)) {

            this.$component.$.btnApproveAll.classList.add('active');
        }
        this.$component.$.btnRejectAll.classList.remove('active');
    }
    btnColumnRejectClick(e, value, row, index) {
        row.RequestResponse = RequestDetail.REPONSE_REJECT;
        if (this.$component.$entity.AccessRequestDetails.every(x => x.RequestResponse == RequestDetail.REPONSE_REJECT)) {

            this.$component.$.btnRejectAll.classList.add('active');
        }
        this.$component.$.btnApproveAll.classList.remove('active');
    }

    // Table Events History ------------------------------------------------------------------ //
    get tableEventColumnHistory() {
        let $this = this;
        return {
            'click .btn-history': $this.btnColumnHistoryClick.bind($this.$component)
        }
    }
    btnColumnHistoryClick(e, value, row, index) {
        this.$component.requestDetailHistory.show(this.$entity, row);
    }


    load(data = null) {
        this.$table.bootstrapTable('load', data || []);
    }
    getData() {
        return this.$table.bootstrapTable('getData');
    }

    refreshView() {
        let entity = this.$component.$entity;
        if (this.$component.detailExpanded) {
            this.$table.bootstrapTable('refreshOptions', {
                height: window.innerHeight - 400
            });
        }
        else if (
            (this.$component.mode == RequestDetailTableView.MODE_EDIT || this.$component.mode == RequestDetailTableView.MODE_VIEW)
            && entity.RequestID > 0 && entity.RequestNote == ''
        ) {
            this.$table.bootstrapTable('refreshOptions', {
                height: window.innerHeight - 515
            });
        }
        else {
            this.$table.bootstrapTable('refreshOptions', {
                height: window.innerHeight - 576
            });
        }
    }
}