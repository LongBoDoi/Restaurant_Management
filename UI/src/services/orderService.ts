import { MLActionResult, Order } from "@/models";
import BaseService from "./baseService";
import CommonFunction from "@/common/CommonFunction";

class OrderService extends BaseService<Order> {
    protected entityName: string = 'Order';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu chi tiết order
     * @param page Số trang
     * @param itemsPerPage Kích thước trang
     */
    public async getOrderDetail(orderID: string) : Promise<Order|undefined> {
        var order:Order|undefined = undefined;

        try {
            const response = await this.api.get('/GetOrderDetail', {
                params: {
                    orderID: orderID
                }
            });
            const actionResult:MLActionResult = response.data;
            if (actionResult?.Success) {
                order = actionResult.Data as Order;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return order;
    }

    /**
     * Lấy order đang hoạt động của khách hàng
     */
    public async getActiveOrderByCustomerID(customerID: string) : Promise<Order|undefined> {
        var order:Order|undefined = undefined;

        try {
            const response = await this.api.get('/GetActiveOrderByCustomerID', {
                params: {
                    customerID: customerID
                }
            });
            const actionResult:MLActionResult = response.data;
            if (actionResult?.Success) {
                order = actionResult.Data as Order;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return order;
    }
}

export default OrderService;

const orderService = new OrderService();
export {orderService};