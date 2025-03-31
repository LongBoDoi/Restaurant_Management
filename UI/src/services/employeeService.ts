import { Employee, MLActionResult } from "@/models";
import BaseService from "./baseService";

class EmployeeService extends BaseService<Employee> {
    protected entityName: string = 'Employee';
    
    constructor() {
        super();
        this.configApi();
    }

    async updateEmployee(employee: Employee, image:File|undefined) : Promise<MLActionResult> {
        const formData = new FormData();
        formData.append("employee", JSON.stringify(employee));
        if (image) {
            formData.append("image", image);
        }
        
        const response = await this.api.post('/UpdateEmployee', formData, {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        });
        return response.data as MLActionResult;
    }
}

export default EmployeeService;

const employeeService = new EmployeeService();
export {employeeService};