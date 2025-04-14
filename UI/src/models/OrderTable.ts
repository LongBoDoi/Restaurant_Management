import MLEntity from "./MLEntity";
import Order from "./Order";
import Table from "./Table";

interface OrderTable extends MLEntity {
    OrderTableID: string,
    OrderID: string,
    TableID: string,

    Order?: Order,
    Table?: Table
};

export default OrderTable;