import { EnumEmployeeWorkStatus } from "@/common/Enumeration";
import UserLogin from "./UserLogin";
import MLEntity from "./MLEntity";
import Role from "./Role";

interface Employee extends MLEntity {
    EmployeeID: string,
    EmployeeCode: string,
    EmployeeName: string,
    PhoneNumber: string,
    Email: string,
    WorkStatus: EnumEmployeeWorkStatus,
    ImageUrl: string,
    Schedule?: string,

    UserLogin?: UserLogin,
    Roles: Role[]
};

export default Employee;