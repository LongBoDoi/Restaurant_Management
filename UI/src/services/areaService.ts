import { Area } from "@/models";
import MLBaseService from "./baseService";

class AreaService extends MLBaseService<Area> {
    protected entityName: string = 'Area';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default AreaService;

const areaService = new AreaService();
export {areaService};