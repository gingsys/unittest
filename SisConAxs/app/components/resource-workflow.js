import { Entity } from '../core.js'
import { ResourceBase } from './resource-base.js'

export class Workflow extends Entity {
    constructor() {
        super();
        this.WfActivo = 0;
        this.WfApproveHierarchyID = 0;
        this.WfApproveHierarchyName = "";
        this.WfDescription = "";
        this.WfID = 0;
        this.WfName = "";
        this.WorkflowItems = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        // this.WorkflowItems.forEach(x => {
        //     x.$Workflow = this;
        // });
        this.WorkflowItems = WorkflowItem.fromList(this.WorkflowItems || []);
    }

    changeApproveHierarchy(id = null) {
        this.WfApproveHierarchyID = id;
        this.WorkflowItems
            .filter(x => x.WfItemType == WorkflowItem.TYPE_ACCION)
            .forEach(x => {
                if (x.WfItemActionValue > 1) x.WfItemActionValue = 1;
            });
    }

    addItem(item) {
        let index = this.WorkflowItems.indexOf(item);
        if (index == -1) {
            this.WorkflowItems.push(item);
            this.recalcOrder();
        }
    }

    deleteItem(item) {
        let step = item.WfItemStep;
        let index = this.WorkflowItems.indexOf(item);

        this.WorkflowItems.splice(index, 1);
        this.WorkflowItems.forEach(x => {
            if (x.WfItemNextItem == step) x.WfItemNextItem = null;
            if (x.WfItemApproveItem == step) x.WfItemApproveItem = null;
            if (x.WfItemRejectItem == step) x.WfItemRejectItem = null;
            if (x.WfItemTimeoutItem == step) x.WfItemTimeoutItem = null;
        });
        this.recalcOrder();
    }

    recalcOrder() {
        let values = {};
        this.WorkflowItems.forEach((x, index) => {
            values[x.WfItemStep] = index + 1;
        });
        this.WorkflowItems.forEach(x => {
            x.WfItemStep = values[x.WfItemStep];
            if (x.WfItemNextItem != null) x.WfItemNextItem = values[x.WfItemNextItem];
            if (x.WfItemApproveItem != null) x.WfItemApproveItem = values[x.WfItemApproveItem];
            if (x.WfItemRejectItem != null) x.WfItemRejectItem = values[x.WfItemRejectItem];
            if (x.WfItemTimeoutItem != null) x.WfItemTimeoutItem = values[x.WfItemTimeoutItem];
        });
    }

    getGraphData() {
        let nodes = [];
        if (this.WorkflowItems.length > 0) {
            nodes.push({
                id: 0,
                label: 'Inicio',
                title: 'Inicio',
                shape: 'icon',
                icon: {
                    face: '"Font Awesome 5 Free"',
                    code: '\uf192',
                    size: 30, //50,
                    color: '#4a4a4a'
                }
            });
        }
        nodes = nodes.concat(
            this.WorkflowItems.map(x => {
                let code = null;
                if (x.WfItemType == WorkflowItem.TYPE_ACCION)
                    code = '\uf0a6';
                if (x.WfItemType == WorkflowItem.TYPE_CONSULTA)
                    code = '\uf059'; //f0e0';
                if (x.WfItemType == WorkflowItem.TYPE_NOTIFICACION)
                    code = '\uf1d8';
                return {
                    id: x.WfItemStep,
                    label: x.WfItemStep.toString(),
                    title: x.WfItemName,
                    shape: 'icon',
                    icon: {
                        face: '"Font Awesome 5 Free"',
                        code: code,
                        size: 30, //50,
                        color: '#4a4a4a'
                    }
                };
            })
        );
        let edges = [];
        if (this.WorkflowItems.length > 0)
            edges.push({ from: 0, to: this.WorkflowItems[0].WfItemStep, title: 'siguiente' });
        this.WorkflowItems.forEach(x => {
            if (x.WfItemNextItem != null)
                edges.push({ from: x.WfItemStep, to: x.WfItemNextItem, title: 'siguiente' });
            if (x.WfItemApproveItem != null)
                edges.push({
                    from: x.WfItemStep,
                    to: x.WfItemApproveItem,
                    title: 'aprobado',
                    color: { color: 'green', highlight: 'green' }
                });
            if (x.WfItemRejectItem != null)
                edges.push({
                    from: x.WfItemStep,
                    to: x.WfItemRejectItem,
                    title: 'rechazado',
                    color: { color: 'red', highlight: 'red' }
                });
            if (x.WfItemTimeoutItem != null)
                edges.push({
                    from: x.WfItemStep,
                    to: x.WfItemTimeoutItem,
                    title: 'tiempo expirado',
                    color: { color: '#e3c042', highlight: '#e3c042' }
                });
        });
        return {
            nodes: nodes,
            edges: edges
        }
    }
}
export class WorkflowItem extends Entity {
    static get TYPE_NOTIFICACION() { return 37; }
    static get TYPE_CONSULTA() { return 38; }
    static get TYPE_ACCION() { return 39; }

    static get DEST_APROBADOR() { return 1; }
    static get DEST_SOLICITANTE() { return 2; }
    static get DEST_SOLICITADO_PARA() { return 3; }
    static get DEST_OTRO() { return 4; }
    static get DEST_EJECUTOR() { return 5; }

    constructor() {
        super();
        // this.$Workflow = null;
        this.WfItemActionProperty = 1;
        this.WfItemActionValue = 1;
        this.WfItemApproveItem = null;
        this.WfItemDestMail = "";
        this.WfItemDestType = 1;
        this.WfItemEnterCondition = null;
        this.WfItemEnterParams = null;
        this.WfItemExitValues = null;
        this.WfItemId = 0;
        this.WfItemMessage = "";
        this.WfItemName = "";
        this.WfItemNextItem = null;
        this.WfItemPrevSibling = null;
        this.WfItemRejectItem = null;
        this.WfItemStep = 0;
        this.WfItemSubject = "";
        this.WfItemTimeoutDueTime = 0;
        this.WfItemTimeoutDueUnits = 0;
        this.WfItemTimeoutItem = null;
        this.WfItemType = 0;
        this.WfItemTypeName = "";
        this.WfItemWfID = 0;
        this.WorkflowItemNext = [];
        this.WorkflowItemNextParents = [];

        this.WfItemCcType = 0;
        this.WfItemCcMail = "";
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.WorkflowItemNext = WorkflowItemNext.fromList(this.WorkflowItemNext || []);
        this.WorkflowItemNextParents = WorkflowItemNext.fromList(this.WorkflowItemNextParents || []);
    }
}
export class WorkflowItemNext extends Entity {
    constructor() {
        super();
        this.WfItemNextItemID = 0;
        this.WfItemNextStep = 0;
        this.WfItemNextType = 0;
        this.WfItemNextWfID = 0;
        this.WfParentItemID = 0;
    }
}
export class WorkflowResource extends ResourceBase {
    get api() {
        return '/api/Workflows';
    }

    static loadSelect(select, name) {
        select.innerHTML = '';
        return this.$get(`/api/Workflows`, undefined, {
            headers: {
                'X-Auth-Token': this.session.sessionToken || {}
            }
        }).then(r => {
            r.forEach(item => {
                let opt = document.createElement('option');
                opt.innerHTML = item.WfName;
                opt.value = item.WfID;
                opt.$data = item;
                select.appendChild(opt);
            });
            select.value = null;
        });
    }

    copyWorkflow(id) {
        return WorkflowResource.$post(`/api/Workflows/${id}`, {}, undefined, {
            headers: {
                'X-Auth-Token': this.constructor.session.sessionToken || {}
            }
        });
    }
}