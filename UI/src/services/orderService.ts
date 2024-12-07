import { MLActionResult, Order } from "@/models";
import BaseService from "./baseService";
import { EnumOrderStatus } from "@/common/Enumeration";

class OrderService extends BaseService<Order> {
    protected entityName: string = 'Order';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu phân trang
     * @param page Số trang
     * @param itemsPerPage Kích thước trang
     */
    public async getOrdersByStatus(status: EnumOrderStatus, page: number, itemsPerPage: number) : Promise<MLActionResult> {
        const result = await this.api.get('/GetOrdersByStatus', {
        params: {
            status: status,
            page: page,
            itemsPerPage: itemsPerPage
        }
        });
        return result.data as MLActionResult;
    }
}

export default OrderService;

const orderService = new OrderService();
export {orderService};