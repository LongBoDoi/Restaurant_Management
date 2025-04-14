import MLEntity from "./MLEntity";
import Table from "./Table";

interface Area extends MLEntity {
    AreaID: string,
    AreaName: string,
    TableCount: number,

    Tables?: Table[]
}

export default Area;