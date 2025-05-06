import MLEntity from "./MLEntity";

interface Permission extends MLEntity {
    PermissionID: string,
    PermissionCode: string,
    PermissionName: string
}

export default Permission