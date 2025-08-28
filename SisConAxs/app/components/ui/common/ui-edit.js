import { Component } from '../../../core/core.js';

export class UIEditComponent extends Component {
    static get SAVE_METHOD_DEFAULT() { return 'SAVE_METHOD_DEFAULT'; }
    static get SAVE_METHOD_ONLY_POST() { return 'SAVE_METHOD_ONLY_POST'; }

    constructor(options) {
        super(false);
        this.__options = Object.assign({
            saveMethod: UIEditComponent.SAVE_METHOD_DEFAULT
        }, options);
        this.$resource = new this.Resource();
        this.$entity = new this.Entity();
    }

    get Resource() {
        return null;
    }
    get Entity() {
        return null;
    }
    get entityID() {
        return '';
    }

    connectedCallback() {
        super.connectedCallback();
    }
    disconnectedCallback() {
        super.disconnectedCallback();
        this.$$.formModal.modal('hide');
    }

    add() {
        const entity = new this.Entity();
        this.edit(entity);
    }

    async edit(entity) {
        this.$entity = entity;
        this.EntityToForm();
        this.render();
        this.$$.formModal.modal({ backdrop: 'static' });        
    }

    async editById(id) {
        this.$presenter.showLoading();
        this.$resource.getById(id).then(
        // async ({data}) => {
        async data => {
            const entity = this.Entity.fromObject(data);
            await this.edit(entity);
            this.$presenter.hideLoading();
        },
        err => {
            this.$presenter.hideLoading();
            iziToast.error({ title: 'Error al cargar' });
        });
    }

    save(e) {
        e.preventDefault();
        if (!e.target.checkValidity()) {
            iziToast.error({ title: 'No ha ingresado los datos necesarios' });
            console.error(e.target.reportValidity());
            return;
        };
        if(!this.validate(e)) return;

        this.FormToEntity();
        this.$presenter.showLoading();

        let request;
        if(this.__options.saveMethod == UIEditComponent.SAVE_METHOD_ONLY_POST) {
            request = this.$resource.post(this.$entity);
        } else {  // default
            if(this.$entity[this.entityID] == 0) {
                request = this.$resource.post(this.$entity);
            } else if(this.$entity[this.entityID] > 0) {
                request = this.$resource.put(this.$entity);
            }
        }
        request.then(
            r => {
                this.$$.formModal.modal('hide');
                this.$presenter.hideLoading();
                console.log('grabado!');
                iziToast.success({ title: 'Grabado correctamente' });
                //this.dispatchEvent(new CustomEvent('save', {detail: {data: r.data}}));
                this.dispatchEvent(new CustomEvent('save', { detail: r }));
            },
            err => {
                this.$presenter.hideLoading();
                //iziToast.error({ title: err.data });
                iziToast.error({ title: err });
            }
        );
    }

    delete(e) {
        if (this.$entity[this.entityID] > 0) {
            this.$presenter.question(
                'Borrar',
                'Desea borrar este registro?',
                (instance, toast) => {
                    this.$presenter.showLoading();
                    this.$resource.delete(this.$entity[this.entityID]).then(
                        r => {
                            this.$$.formModal.modal('hide');
                            this.$presenter.hideLoading();
                            console.log('borrado!');
                            iziToast.success({ title: 'Borrado correctamente' });
                            this.dispatchEvent(new CustomEvent('delete'));
                        },
                        err => {
                            this.$presenter.hideLoading();
                            // console.error('error!', err);
                            //iziToast.error({ title: err.data });
                            iziToast.error({ title: err });
                        }
                    );
                }
            );
        }
    }

    validate(e) {
        return true;
    }

    EntityToForm() {
    }
    FormToEntity() {
    }
}