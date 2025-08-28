export { Entity } from './entity.js';
export { EntityForm } from './entity-form.js';
export { LocalDbStorage } from './localdbstorage.js';

//export { Resource, ResourceFetch } from './resource.js';                                  // descomentar cuando todos los módulos se adapten a la versión nueva
export { Resource, ResourceFetch } from './resource-adapterv1.js';                          // quitar cuando todos los módulos se adapten a la versión nueva

//export { Component, Presenter, html, classMap, styleMap } from './component.js';          // descomentar cuando todos los módulos se adapten a la versión nueva
export { Component, Presenter, html, classMap, styleMap } from './component-adapterv1.js';  // quitar cuando todos los módulos se adapten a la versión nueva

export { ComponentView } from './component-view.js';
export { Router } from './router.js';
export { DOM } from './dom.js';

// UTILS ------------------------------------------------------------------------------ /