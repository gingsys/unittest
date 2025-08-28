import { ComponentView, html } from "./core/core.js";

export class ApplicationView extends ComponentView {
    render() {
        const $component = this.$component;
        return html`
        <div class="divLoading" id="dvLoading">
            <div class="lds-dual-ring"></div>
        </div>

        <!-- Sidebar Holder -->
        <nav id="sidebar" class="mCustomScrollbar">
            <div id="dismiss" class="ico-menu">
                <i class="fas fa-arrow-left"></i>
            </div>
            <div id="btnHelpMobile" class="d-block d-sm-none ico-menu btn-help" @click=${$component.showHelp}>
                <i class="fa-question-circle fas"></i>
            </div>
            <div id="sidebarHead" class="sidebar-header">
                <h4>
                    <img src="content/images/favicon.png" class="nav-brand-logo" />
                    <span class="navbar-brand-title">ICARUS</span>
                </h4>
            </div>
            <div id="sidebarBody">
                <application-menu id="menu"></application-menu>
            </div>
        </nav>
        <!-- Page Content Holder -->
        <nav id="navbarHeader" class="navbar navbar-expand-lg" style="display:${$component.logged? 'flex' : 'none'}">
            <!--class="container-fluid"-->
            <div class="navbar-header">
                <button type="button" id="sidebarCollapse" class="btn ico-menu ico-collapse" data-toggle="popover" data-placement="right" data-content="Click aquí para ver el menú">
                    <i class="fas fa-bars"></i>
                </button>
                <a class="navbar-brand" href="#/">
                    <h4 class="mb-0">
                        <img src="content/images/favicon.png" class="nav-brand-logo" />
                        <span class="navbar-brand-title">ICARUS</span>
                    </h4>
                </a>
                <i id="iconOnline" class="fas fa-bolt ico-menu online" title="Conectado"></i>
                <!-- <i id="iconSync" class="fas fa-sync-alt" title="Sincronizar Datos"></i> -->
            </div>

            <!-- <div class="navbar-collapse collapse"> -->
            <ul class="navbar-nav mr-auto" style="margin-top: -4px;">
                <li class="nav-item dropdown" id="listItemCompany">
                    <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <i class="fas fa-building"></i> <span id="lblNavbarTopCompany">${$component.companyName}</span>
                    </a>
                    ${$component.__listCompanies.length > 0? html`
                    <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                        ${$component.__listCompanies.map(c => html`
                            <a class="dropdown-item" href="javascript:void(0)" @click=${e => $component.changeCompany(c.CompanyID)}>${c.CompanyDisplay}</a>
                        `)}
                    </div>
                    ` : ''}
                </li>
            </ul>
            <!-- </div> -->

            <ul class="navbar-nav mr-auto" style="margin-top: -4px;">
                <li class="nav-item">
                    <div data-toggle="collapse" class="navbar-info-user" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
                        <i class="fas fa-user"></i> <small id="lblNavbarTopUser">${$component.userFullname}</small>
                    </div>
                </li>
            </ul>

            <div class="navbar-collapse collapse">
                <ul class="navbar-nav mr-auto">
                    <li></li>
                </ul>
                <ul class="navbar-nav">
                    <!-- <li class="nav-item">
                    <button id="btnChangelog" type="button" class="btn navbar-toggle collapsed ico-menu" title="Changelog" @click=${$component.showChangelog}>
                        <i class="fas fa-code-branch"></i>
                    </button>
                </li> -->
                    <li class="nav-item">
                        <button id="btnHelp" type="button" class="btn navbar-toggle collapsed ico-menu" title="Ayuda" @click=${$component.showHelp}>
                            <i class="fas fa-question-circle"></i>
                        </button>
                    </li>
                    <li class="nav-item">
                        <button id="btnLogout" type="button" class="btn btn-warning navbar-toggle collapsed" title="Cerrar Sesión" @click=${$component.signOut}>
                            <i class="fas fa-sign-out-alt"></i> Cerrar Sesión
                        </button>
                     
                    </li>
                </ul>
            </div>
            <div style="background-color:#434758">
                <!-- Collect the nav links, forms, and other content for toggling -->
                <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <ul class="nav navbar-nav">
                        <li id="liProyecto" class="dropdown" style="display:none;">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                <span>
                                    Proyecto: <label id="lblProyecto"></label>
                                </span>
                                <br />
                                <span id="lnkTipoServ" title="">
                                    Tipo Serv.: <label id="lblTipoServ"></label>
                                </span>
                            </a>
                            <ul id="listProyectos" class="dropdown-menu"></ul>
                        </li>
                        <!-- <li id="liTipoServ" class="dropdown" style="display:none">

                    </li> -->
                        <li id="liDateParams" class="liDtPrms" style="display: none">
                            Hoy: <label id="lblFchNow"></label><br /> Sem: <label style="font-size:inherit" class="label label-warning" id="lblSemNow"></label>
                        </li>
                    </ul>
                    <ul class="nav navbar-nav navbar-right">
                        <li id="liUsuario" class="dropdown" style="display:none">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                <i class="fas fa-user"></i>
                                <span id="spnUser"></span>
                                <span class="caret"></span>
                            </a>
                            <ul class="dropdown-menu">
                                <li>
                                    <a href="#">
                                        Perfil:
                                        <span id="spnPerfil"></span>
                                    </a>
                                </li>
                                <li role="separator" class="divider"></li>
                                <li id="liLogOut">
                                    <a href="#">
                                        <i class="btn btn-danger btn-xs glyphicon glyphicon-off"></i> Cerrar Sesión
                                    </a>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
                <!-- /.navbar-collapse -->
            </div>
        </nav>
        <!-- Content -->
        <div id="content"></div>
        <!-- Dark Overlay element -->
        <div class="overlay"></div>
        `;
    }
}