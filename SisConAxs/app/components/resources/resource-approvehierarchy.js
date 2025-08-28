import { Entity } from '../../core/core.js';
import { ResourceBase } from './resource-base.js';

export class WorkflowApproveHierarchy extends Entity {
    constructor() {
        super();
        this.WfApproveHierarchyDepartment = null;
        this.WfApproveHierarchyDepartmentName = null;
        this.WfApproveHierarchyID = 0;
        this.WfApproveHierarchyName = "";
        this.WorkflowHierarchyMembers = [];
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        this.WorkflowHierarchyMembers = WorkflowHierarchyMember.fromList(this.WorkflowHierarchyMembers || []);
    }
}
export class WorkflowHierarchyMember extends Entity {
    constructor() {
        super();
        this.WfApproveHierarchyID = 0;
        this.WfHierarchyMemberCompany = null;
        this.WfHierarchyMemberCompanyName = "";
        this.WfHierarchyMemberCompanyDisplay = "";
        this.WfHierarchyMemberDepartment = 0;
        this.WfHierarchyMemberDepartmentName = "";
        this.WfHierarchyMemberDescription = "";
        this.WfHierarchyMemberID = 0;
        this.WfHierarchyMemberOrder = 0;
        this.WfHierarchyMemberPosition = 0;
        this.WfHierarchyMemberPositionName = "";
        this.WfHierarchyMemberPerson = "";
    }
}
export class WorkflowApproveHierarchyResource extends ResourceBase {
    get api() {
        return '/api/WorkflowApproveHierarchy';
    }

    getPeopleMemberWorkflow(data) {
        return WorkflowApproveHierarchyResource.$get(`/api/WorkflowApproveHierarchy/GetMember/${data.WfHierarchyMemberDepartment}/${data.WfHierarchyMemberPosition}`, undefined, {
            headers: {
                'X-Auth-Token': this.constructor.session.sessionToken || {}
            }
        });
    }
}