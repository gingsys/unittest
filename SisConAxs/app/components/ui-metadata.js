import { ControlSelect } from "./ui-control-select.js";
import { Metadata, MetadataResource } from "./resource-metadata.js";

export class SelectMetadata extends ControlSelect {
    constructor(select) {
        super(select);
        $(select).on('show.bs.select', () => {
            $(this.__select).selectpicker('refresh');
        });
    }

    refresh(parentID, fn = null) {
        return MetadataResource.loadSelect(this.__select, parentID, fn).then(r => {
            $(this.__select).selectpicker('refresh');
        });
    }
}
