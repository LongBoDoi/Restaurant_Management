import Employee from "./Employee";
import MLEntity from "./MLEntity";
import Permission from "./Permission";

interface Role extends MLEntity {
    RoleID: string,
    RoleCode: string,
    RoleName: string,

    Employees: Employee[],
    Permissions: Permission[],

    PermissionCodes: string[]
}

export default Role;