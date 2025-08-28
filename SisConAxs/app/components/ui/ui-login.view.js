import { ComponentView, html } from "../../core/core.js";

export class LoginView extends ComponentView {
    render() {
        const $component = this.$component;
        
        return html`
        <style>
            body {
                    background-color: #DDD;
                    background-image: url('content/images/background.jpg');
                    background-position: center;
                    background-attachment: fixed; 
                    background-size: cover;
                }

            application-login .modal-dialog {
                position: absolute;
                top: 50%;
                transform: translateY(-50%);
            }

            application-login .modal-content {
                border: 0;
                background-color: transparent;
            }
            application-login .modal-content:before {
                content: "";
                background-color: rgba(175,175,175,.35);
                position: absolute;
                width: 100%;
                height: 100%;
                display: block;
                z-index: -1;
                backdrop-filter: blur(5px);
                border-radius: 6px;
            }

            application-login .modal-header {
                text-align: center;
                padding: 0;
                border: none;
            }

            application-login .modal-body {
                padding: 0 15px;
                background-color: #FFF;
                box-shadow: 0 0px 25px 2px rgba(0,0,0,.7);
            }

            application-login .modal-footer {
                border: none;
            }

            application-login #logoApp {
                height: 200px;
                margin: auto;
                padding-top:1.4rem;
                /*filter: grayscale(100%);*/
            }

            application-login #logoOffice {
                height: 200px;
                margin: auto;
                padding-top:1.4rem;
                /*filter: grayscale(100%);*/
            }

            application-login .logoEmpresa {
                padding: .5em;
                text-align: center;
                border-radius: 6px 6px 0 0px;
            }
                application-login .logoEmpresa img {
                    height: 50px;
                }

            @media (min-width: 768px) {
                application-login .modal-sm {
                    width: 400px;
                }
                application-login .modal-dialog {
                    margin: auto;
                    top: 50%;
                    left: 50%;
                    transform: translateX(-50%) translateY(-50%);
                }
            }

            application-login .microsoft {
                font: Poppins,sans-serif 15px;
                font-weight: 600;
                color: #5E5E5E;
                background:  #000000;
                border: 1px #8C8C8C;
            }
            application-login .version {
                font-size: .85em;
                font-weight: 600;
            }
        </style>
        <div class="modal" id="formModal" tabindex="-1" role="dialog" style="display:block">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                <form class="form-horizontal login-form" @submit=${$component.login} novalidate>
                    <div class="modal-header login-header">
                        <div class="col-md-12 logoEmpresa">
                           <img src="content/images/logo-powered-aenza.png"  />
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12" style="width: 33rem;height: 19rem;text-align:center;">
                                <img id="logoApp" src="content/images/logo-app.png" title="Interfaz Control de Accesos y Registro Único de Solicitudes" />
                            </div>
                        </div>
                    </div>
                    <div  class="col-md-12" style="text-align: center">
                        <div class="text-center py-3">
                            <button type="button" id="signIn" class="btn btn-warning btn-lg btn-block" @click=${$component.signIn} style="background-color: #E5BE01;border-color:#E5BE01;color:#000000;font-size: 14pt;">
                                <img src="content/images/Logo office Negro.png" /> Continuar con Office 365
                            </button>
                        </div>
                    </div>
                    ${this.debugLogin}
                    
                    <div class="text-center version">
                        versión: ${$component.$presenter.applicationVersion}
                    </div>
                </form>
            </div>
        </div>
        `;
    }

    get debugLogin() {
        if(window.__ENVIROMENT__ == "DEBUG") {
            return html`
            <div  class="col-md-12" style="text-align: center">
                <div class="text-center py-3">
                    <input type="text" id="txtUser" class="form-control">
                    <button type="button" id="btnLogin" class="btn btn-info btn-lg btn-block" @click=${this.$component.login}>
                        <i class="fas fa-sign-in-alt"></i> Login
                    </button>
                </div>
            </div>
            `;
        }
        return '';
    }
}