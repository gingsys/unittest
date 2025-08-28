import { UIEditComponent } from '../common/ui-edit.js';
import { CategoryResource, Category } from '../../resources/resource-category.js';
import { CategoryEditView } from './ui-category-edit.view.js';

export class CategoryEditComponent extends UIEditComponent {
    constructor() {
        super({ saveMethod: UIEditComponent.SAVE_METHOD_ONLY_POST });
        this.$entity = new Category();
    }

    get $template() {
        return new CategoryEditView(this);
    }

    get Resource() {
        return CategoryResource;
    }
    get Entity() {
        return Category;
    }
    get entityID() {
        return 'CategoryID';
    }

    connectedCallback() {
        super.connectedCallback();
    }

    EntityToForm() {
        this.$.txtName.value        = this.$entity.CategoryName;
        this.$.txtDescription.value = this.$entity.CategoryDescription;
    }
    FormToEntity() {
        this.$entity.CategoryName        = this.$.txtName.value.trim();
        this.$entity.CategoryDescription = this.$.txtDescription.value.trim();
    }

    validate(e) {
        return true;
    }
}
customElements.define('category-edit', CategoryEditComponent);