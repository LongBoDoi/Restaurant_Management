import { EnumOrderStatus } from "@/common/Enumeration";
import OrderDetail from "./OrderDetail";
import MLEntity from "./MLEntity";
import Customer from "./Customer";

interface Order extends MLEntity {
    OrderID: string,
    OrderName: string,
    CustomerID: string|undefined,
    CustomerName: string,
    OrderDate: Date,
    TotalAmount: number,
    Status: EnumOrderStatus,
    SpecialRequest: string,

    OrderDetails: OrderDetail[],
    Customer: Customer|undefined
};

export default Order;