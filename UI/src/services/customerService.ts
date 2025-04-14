import { Customer } from "@/models";
import MLBaseService from "./baseService";

class CustomerService extends MLBaseService<Customer> {
    protected entityName: string = 'Customer';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default CustomerService;

const customerService = new CustomerService();
export {customerService};