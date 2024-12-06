import { Employee } from "@/models";
import BaseService from "./baseService";

class EmployeeService extends BaseService<Employee> {
    protected entityName: string = 'Employee';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default EmployeeService;

const employeeService = new EmployeeService();
export {employeeService};