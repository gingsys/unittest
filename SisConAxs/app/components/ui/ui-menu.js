import { Component, html } from '../../core/core.js';
import { SessionResource } from '../resources/resource-session.js';
import { MenuConfig } from '../../config/menu.js';

export class MenuComponent extends Component {
    constructor() {
        super(false);
    }

    get $template() {
            const session = SessionResource.session;

            return html`<ul class="list-unstyled components">
            ${MenuConfig.map(item => {
                if(item.mobile === undefined || (item.mobile === false && !Router.isMobile) || (item.mobile === true && Router.isMobile)) {
                    const haveItemPermission = session.havePermission(item.authlevel);
                    const haveSubitemsPermissions = item.subitems.reduce((prev, curr) => prev || session.havePermission(curr.authlevel), false);

                    if(haveSubitemsPermissions || haveItemPermission) {
                        const itemID = Math.floor(Math.random() * Math.pow(256, 3)).toString(16);
                        return html`
                        <li>
                            <a href="#menuitem_${itemID}" data-toggle="collapse" aria-expanded="false" class="collapsed">
                                <i class="${item.icon}"></i> ${item.title}
                            </a>
                            <ul class="collapse list-unstyled" id="menuitem_${itemID}">
                                ${item.subitems.map(subItem => {
                                    if(subItem.mobile === undefined || (subItem.mobile === false && !Router.isMobile) || (subItem.mobile === true && Router.isMobile)) {
                                        if(subItem.authlevel === undefined || session.havePermission(subItem.authlevel)) {
                                            return html`
                                            <li class="li-style">
                                                ${!!subItem.controller?
                                                    html`<a href="javascript:void(0)" @click=${e => { this.hideSidebar(); subItem.controller(e); }}>
                                                            <i class="fas fa-caret-right mr-1"></i> ${subItem.icon? html`<i class="${subItem.icon}"></i>` : ''} ${subItem.title}
                                                        </a>` :
                                                    html`<a href="${subItem.url}" @click=${this.hideSidebar}>
                                                            <i class="fas fa-caret-right mr-1"></i> ${subItem.icon? html`<i class="${subItem.icon}"></i>` : ''} ${subItem.title}
                                                        </a>`
                                                }
                                            </li>`;
                                        }
                                    }
                                    return '';
                                })}
                            </ul>
                        </li>`;
                    }
                }
                return '';
            })}
            </ul>`;
    }

    connectedCallback() {
        super.connectedCallback();
    }

    hideSidebar() {
        $('#sidebar').removeClass('active');  // hide sidebar
        $('.overlay').removeClass('active');  // hide overlay
    }
}
customElements.define('application-menu', MenuComponent);