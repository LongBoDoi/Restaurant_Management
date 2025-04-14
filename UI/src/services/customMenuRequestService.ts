import MLBaseService from "./baseService";
import CustomMenuRequest from "@/models/CustomMenuRequest";

class CustomMenuRequestService extends MLBaseService<CustomMenuRequest> {
  protected entityName: string = 'CustomMenuRequest';
  
  constructor() {
      super();
      this.configApi();
  }
}

export default CustomMenuRequestService;

const customMenuRequestService = new CustomMenuRequestService();
export {customMenuRequestService};