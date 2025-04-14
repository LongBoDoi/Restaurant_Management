import { Table } from "@/models";
import MLBaseService from "./baseService";

class TableService extends MLBaseService<Table> {
    protected entityName: string = 'Table';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default TableService;

const tableService = new TableService();
export {tableService};