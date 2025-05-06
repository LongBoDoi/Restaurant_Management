import MLBaseService from "./baseService";
import Permission from "@/models/Permission";

class PermissionService extends MLBaseService<Permission> {
    protected entityName: string = 'Permission';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default PermissionService;

const permissionService = new PermissionService();
export {permissionService};