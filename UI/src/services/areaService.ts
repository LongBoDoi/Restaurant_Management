import { Area } from "@/models";
import BaseService from "./baseService";

class AreaService extends BaseService<Area> {
    protected entityName: string = 'Area';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default AreaService;

const areaService = new AreaService();
export {areaService};