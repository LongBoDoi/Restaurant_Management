import MLBaseService from "./baseService";
import Role from "@/models/Role";

class RoleService extends MLBaseService<Role> {
    protected entityName: string = 'Role';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default RoleService;

const roleService = new RoleService();
export {roleService};