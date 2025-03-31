import { EnumEmployeeWorkStatus } from "@/common/Enumeration";
import UserLogin from "./UserLogin";
import MLEntity from "./MLEntity";

interface Employee extends MLEntity {
    EmployeeID: string,
    EmployeeCode: string,
    EmployeeName: string,
    PhoneNumber: string,
    Email: string,
    WorkStatus: EnumEmployeeWorkStatus,
    ImageUrl: string,
    Schedule?: string,

    UserLogin?: UserLogin
};

export default Employee;