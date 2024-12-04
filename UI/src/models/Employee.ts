import { EnumRole } from "@/common/Enumeration";
import UserLogin from "./UserLogin";

interface Employee {
    EmployeeID: string,
    EmployeeCode: string,
    EmployeeName: string,
    PhoneNumber?: string,
    Role: EnumRole,
    Schedule?: string,

    UserLogin?: UserLogin
};

export default Employee;