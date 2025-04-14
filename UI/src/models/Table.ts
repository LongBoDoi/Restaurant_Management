import { EnumTableStatus } from "@/common/Enumeration";
import MLEntity from "./MLEntity";
import Area from "./Area";

interface Table extends MLEntity {
    TableID: string,
    TableName: string,
    SeatCount: number,
    Status: EnumTableStatus,
    AreaID: string,

    Area?: Area
}

export default Table;