import { Resource } from './core.js';
import { Router } from './core/core.js'
import { RoutesConfig } from './config/routes.js';
import './app.js';


// Presenter + Routes ----------------------------------------------------------- //
const router = new Router(RoutesConfig, document.querySelector('app-presenter'));
router.initialize();

// Si no tiene una session activa lo regresa al login, si no tiene permiso lo regresa al home
Resource.onError = (data, response) => {
    if (response.status == Resource.HTTP_STATUS_UNAUTHORIZED) {
        Router.redirect('/login');
    } else if (response.status == Resource.HTTP_STATUS_FORBIDDEN) {
        Router.redirect('/');
    }
}