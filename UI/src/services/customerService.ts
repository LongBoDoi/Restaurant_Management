import { Customer, MLActionResult } from "@/models";
import MLBaseService from "./baseService";
import CommonFunction from "@/common/CommonFunction";

class CustomerService extends MLBaseService<Customer> {
    protected entityName: string = 'Customer';
    
    constructor() {
        super();
        this.configApi();
    }

    async updatePersonalInfo(customer: Customer, image:File|undefined) : Promise<boolean> {
        var result = false;
        
        try {
            const formData = new FormData();
            formData.append("customerJson", JSON.stringify(customer));
            if (image) {
                formData.append("image", image);
            }
            
            const response = await this.api.post('/UpdatePersonalInfo', formData, {
                headers: {
                    "Content-Type": "multipart/form-data"
                }
            });
            const actionResult = response.data as MLActionResult;
            
            if (!actionResult.Success && actionResult.ErrorMsg) {
                CommonFunction.showToastMessage(actionResult.ErrorMsg, 'error');
            }

            result = actionResult.Success;
        } catch (error) {
            CommonFunction.handleException(error);
        }
        return result;
    }
}

export default CustomerService;

const customerService = new CustomerService();
export {customerService};