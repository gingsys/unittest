import { Component, html } from '../../core.js';

class NavbarComponent extends Component {
    __ready() {
        return html `
        <!-- Sidebar Holder -->
        <nav id="sidebar" class="mCustomScrollbar">
            <div id="dismiss" class="ico-menu">
                <i class="fas fa-arrow-left"></i>
            </div>
            <div id="btnHelpMobile" class="d-block d-sm-none ico-menu btn-help" >
                <i class="fa-question-circle fas"></i>
            </div>
            <div id="sidebarHead" class="sidebar-header">
                <h4>
                    <img src="content/images/newLogo.png" class="nav-brand-logo" />
                    <span class="navbar-brand-title">ICARUS</span>
                </h4>
            </div>
            <div id="sidebarBody">
                <!-- menu -->
            </div>
        </nav>
        <!-- Page Content Holder -->
        <nav id="navbarHeader" class="navbar navbar-expand-lg">
            <!--class="container-fluid"-->
            <div class="navbar-header">
                <button type="button" id="sidebarCollapse" class="btn ico-menu ico-collapse" data-toggle="popover" data-placement="right" data-content="Click aquí para ver el menú">
                    <i class="fas fa-bars"></i>
                </button>
                <a class="navbar-brand" href="#">
                    <h4>
                        <img src="content/images/newLogo.png" class="nav-brand-logo" />
                        <span class="navbar-brand-title" style="margin-right: 5px;">ICARUS</span>
                    </h4>
                </a>
                <i id="iconOnline" class="fas fa-bolt ico-menu online" title="Conectado"></i>
                <!-- <i id="iconSync" class="fas fa-sync-alt" title="Sincronizar Datos"></i> -->
            </div>

            <!-- <div class="navbar-collapse collapse"> -->
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item dropdown" id="listItemCompany">
                        <a class="nav-link dropdown-toggle" href="#" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i class="fas fa-building"></i> <span id="lblNavbarTopCompany"></span>
                        </a>
                        <div class="dropdown-menu" aria-labelledby="navbarDropdown" id="dropdownCompany">
                            <!-- <a class="dropdown-item" href="#">Action</a>
                            <a class="dropdown-item" href="#">Another action</a>
                            <div class="dropdown-divider"></div>
                            <a class="dropdown-item" href="#">Something else here</a> -->
                        </div>
                    </li>
                </ul>
            <!-- </div> -->

            <ul class="navbar-nav mr-auto">
                <li class="nav-item">
                    <button type="button" class="navbar-toggle collapsed ico-menu" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false" style="padding-right: 0px;">
                        <i class="fas fa-user"></i> <small id="lblNavbarTopUser"></small>
                    </button>
                </li>
            </ul>

            <div class="navbar-collapse collapse">
                <ul class="navbar-nav mr-auto">
                    <li></li>
                </ul>
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <button id="btnChangelog" type="button" class="navbar-toggle collapsed ico-menu" style="padding-right: 0px;" title="Changelog">
                        <i class="fas fa-code-branch"></i>
                    </button>
                    </li>
                    <li class="nav-item">
                        <button id="btnHelp" type="button" class="navbar-toggle collapsed ico-menu" style="padding-right: 0px;" title="Ayuda">
                            <i class="fas fa-question-circle"></i>
                    </button>
                    </li>
                    <li class="nav-item">
                        <button id="btnLogout" type="button" class="navbar-toggle collapsed ico-menu" style="padding-right: 0px;" title="Cerrar Sesión">
                        <i class="fas fa-sign-out-alt"></i>
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
                                <br/>
                                <span id="lnkTipoServ" title="">
                                        Tipo Serv.: <label id="lblTipoServ"></label>
                                    </span>
                            </a>
                            <ul id="listProyectos" class="dropdown-menu"></ul>
                        </li>
                        <!-- <li id="liTipoServ" class="dropdown" style="display:none">
                                
                            </li> -->
                        <li id="liDateParams" class="liDtPrms" style="display: none">
                            Hoy: <label id="lblFchNow"></label><br/> Sem: <label style="font-size:inherit" class="label label-warning" id="lblSemNow"></label>
                        </li>
                    </ul>
                    <ul class="nav navbar-nav navbar-right">
                        <li id="liUsuario" class="dropdown" style="display:none">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                <i class="fas fa-user"></i>
                                <span id="spnUser"></span>
                                <span class="caret" />
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
        </nav>`;
    }
}