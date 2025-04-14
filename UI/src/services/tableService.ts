import { Table } from "@/models";
import BaseService from "./baseService";

class TableService extends BaseService<Table> {
    protected entityName: string = 'Table';
    
    constructor() {
        super();
        this.configApi();
    }
}

export default TableService;

const tableService = new TableService();
export {tableService};