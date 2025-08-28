import { User } from '../components/resources/resource-user.js';

export const MenuConfig = [
    {
        title: 'Solicitudes',
        icon: 'fas fa-file-alt',
        subitems: [
            {   
                url: '#/request/send',
                authlevel: User.ROLE_SOLICITANTE,
                title: 'Mis Solicitudes Emitidas'
            }
            , { 
                url: '#/request/forapprove',
                authlevel: User.ROLE_APROBADOR,
                title: 'Mis Solicitudes por Revisar'
            }
            , { 
                url: '#/request/search',
                authlevel: User.ROLE_VER_SOLICITUDES_PROYECTO | User.ROLE_ADMINISTRADOR,
                title: 'Consultar Solicitudes'
            }
            , { 
                url: '#/request/template',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Solicitudes Base'
            }
            , { 
                url: '#/request/massdeactivation',
                authlevel: User.ROLE_DAR_BAJA,
                title: 'Baja Masiva'
            }                                                       
        ]
    },
    {
        title: 'Sistema',
        icon: 'fas fa-server',
        subitems: [
            {   
                url: '#/company',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Empresas'
            },
            {   
                url: '#/calendar',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Calendario'
            },
            {   
                url: '#/notification',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Configuración de Notificaciones'
            },
            {   
                url: '#/category',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Categorías',
            }
        ]
    },
    {
        title: 'Configuración',
        icon: 'fas fa-cog',
        subitems: [
            {   
                url: '#/user',
                authlevel: User.ROLE_SYSADMIN,   //ROLE_ADMINISTRADOR,
                title: 'Usuarios',
            },
            {   
                url: '#/people',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_CREA_PERSONAS,
                title: 'Destinatarios',
            },
            {   
                url: '#/commonvalue',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Valores Comunes',
            },
            {   
                url: '#/approvehierarchy',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Jerarquías de Aprobación'
            },
            {   
                url: '#/workflow',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Workflows',
            },
            {   
                url: '#/accesstype',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Tipos de Accesos',
            },
            {                              
                url: '#/resource',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Recursos',
            }
        ]
    },
    {   
        title: 'Integraciones',
        icon: 'fas fa-sync',
        subitems: [
            {   
                url: '#/integration/oracle',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_SYSADMIN,
                title: 'Oracle'
            }                                                       
        ]
    },
    {                               
        title: 'Reportes',
        icon: 'fas fa-chart-bar',
        subitems: [
            {                    
                url: '#/report/resourcepeople',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_REPORTE,
                title: 'Reporte de Recursos por Destinatario',
            },
            {   
                url: '#/report/request',
                authlevel: User.ROLE_ADMINISTRADOR,
                title: 'Reporte de Accesos del Personal',
                // controller: () => {
                //     let url = `/Reports/frm_rpt_people.aspx?token=${LoginResource.auth.sessionToken}`;
                //     let width = 1150;
                //     let height = 500;
                //     window.open(url, '_blank', `location=no,menubar=no,width=${width},height=${height},left=${(screen.width - width) / 2},top=${(screen.height - height) / 2}`);
                // }
            },
            {                    
                url: '#/report/bitacora',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_REPORTE,
                title: 'Reporte de Bitácora de Solicitudes',
            },
            {   
                url: '#/report/approver-requestforapproval',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_REPORTE,
                title: 'Aprobadores vs Solicitudes Pendientes',
            },
            {                    
                url: '#/report/workflow-hierarchy',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_REPORTE,
                title: 'Reporte de Jerarquía de Aprobaciones',
            },
            {   
                url: '#/report/oracle-audit',
                authlevel: User.ROLE_ADMINISTRADOR | User.ROLE_REPORTE,
                title: 'Reporte Consolidado de Solicitudes ORACLE - Auditoria',
            },
            {   
                url: '#/report/request-approver',
                authlevel: User.ROLE_APROBADOR | User.ROLE_REPORTE,
                title: 'Reporte de Solicitudes por Aprobador',
            },
            {  
                url: '#/report/users-audit',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Reporte de usuarios para auditoria', // 'Reporte de Auditoria para Usuarios',
            },                    
            {   
                url: '#/report/users-roles',
                authlevel: User.ROLE_SYSADMIN | User.ROLE_ADMINISTRADOR,
                title: 'Reporte de Usuarios/Roles',
            },
            {   
                url: '#/report/oracle-sync',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Reporte de Ejecuciones de la Sincronización con Oracle (datos,recursos y accesos)',
            },
            {
                url: '#/report/oracle-sync-requests',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Reporte Ejecuciones automáticas Oracle de solicitudes',
            },
            {
                url: '#/report/sap-sync-log',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Reporte Ejecuciones Sincronización SAP',
            },
            {
                url: '#/report/aad-sync-log',
                authlevel: User.ROLE_SYSADMIN,
                title: 'Reporte Ejecuciones Sincronización Azure AD',
            }
        ]
    }
];