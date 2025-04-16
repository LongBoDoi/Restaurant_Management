import { EnumOrderStatus, EnumPaymentMethod } from "@/common/Enumeration";
import OrderDetail from "./OrderDetail";
import MLEntity from "./MLEntity";
import Customer from "./Customer";
import OrderTable from "./OrderTable";

interface Order extends MLEntity {
    OrderID: string,
    OrderName: string,
    CustomerID: string|undefined,
    CustomerName: string,
    OrderDate: Date,
    TotalAmount: number,
    NetAmount: number,
    TipAmount: number,
    Status: EnumOrderStatus,
    SpecialRequest: string,
    PaidAmount: number,
    PaymentMethod: EnumPaymentMethod,
    TableName: string,

    OrderDetails: OrderDetail[],
    Customer: Customer|undefined,
    OrderTables?: OrderTable[]
};

export default Order;