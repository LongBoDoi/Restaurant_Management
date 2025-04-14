import BaseService from "./baseService";
import CustomMenuRequest from "@/models/CustomMenuRequest";

class CustomMenuRequestService extends BaseService<CustomMenuRequest> {
  protected entityName: string = 'CustomMenuRequest';
  
  constructor() {
      super();
      this.configApi();
  }
}

export default CustomMenuRequestService;

const customMenuRequestService = new CustomMenuRequestService();
export {customMenuRequestService};