import { EnumRole } from "@/common/Enumeration";
import UserLogin from "./UserLogin";
import MLEntity from "./MLEntity";

interface Employee extends MLEntity {
    EmployeeID: string,
    EmployeeCode: string,
    EmployeeName: string,
    PhoneNumber?: string,
    Role: EnumRole,
    Schedule?: string,

    UserLogin?: UserLogin
};

export default Employee;