import { Entity } from '../../core/core.js';

export class Session extends Entity {
    constructor() {
        super();
        this.User = null;

        this.CompanyID = 0;
        this.CompanyDisplay = '';
        this.CompanyName = '';
        this.UserRole1 = 0;
        this.UserRole2 = 0;
        this.UserRole3 = 0;
        this.UserRole4 = 0;
        this.UserRole5 = 0;
        this.UserRole6 = 0;
        this.UserRole7 = 0;
        this.UserRoleSysAdmin = 0;
        this.UserStatus = 0;

        this.sessionToken = null;
        this.sessionUser = null;
        this.sessionUserFullName = null;
    }

    get UserRole() {
        return this.UserRole1 |
                (this.UserRole2 << 1) |
                (this.UserRole3 << 2) |
                (this.UserRole4 << 3) |
                (this.UserRole5 << 4) |
                (this.UserRole6 << 5) |
                (this.UserRole7 << 6) |
                (this.UserRoleSysAdmin << 10)
        ;
    }

    havePermission(roles) {
        return (this.UserRole & roles) > 0;
    }

    fromDTO(dto) {
        super.fromDTO(dto);
        //this.User = User.fromObject(dto.User || {});
    }
}