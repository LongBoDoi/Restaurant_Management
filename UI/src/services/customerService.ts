import { Customer } from "@/models";
import BaseService from "./baseService";

class CustomerService extends BaseService<Customer> {
    protected entityName: string = 'Customer';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default CustomerService;

const customerService = new CustomerService();
export {customerService};