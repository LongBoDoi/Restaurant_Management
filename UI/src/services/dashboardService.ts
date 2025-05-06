import { MLActionResult, Order, Reservation } from "@/models";
import { BaseService } from "./baseService";
import CommonFunction from "@/common/CommonFunction";
import moment from "moment";
import { EnumTimeFilter } from "@/common/Enumeration";

class DashboardService extends BaseService {
    protected entityName: string = 'Dashboard';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu doanh thu hôm nay
     */
    public async getTotalRevenue(fromDate: string, toDate: string, timeFilter: EnumTimeFilter) : Promise<any> {
        var result = undefined;

        try {
            const response = await this.api.get('/GetTotalRevenue', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    timeFilter: timeFilter,
                    timeZone: CommonFunction.getTimeZone()
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu số order hoàn thành
     */
    public async getTotalOrders(fromDate: string, toDate: string, timeFilter: EnumTimeFilter) : Promise<any> {
        var result = undefined;

        try {
            const response = await this.api.get('/GetTotalOrders', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    timeFilter: timeFilter,
                    timeZone: CommonFunction.getTimeZone()
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Giá trị order trung bình
     */
    public async getAverageOrderValue(fromDate: string, toDate: string, timeFilter: EnumTimeFilter) : Promise<any> {
        var result = undefined;

        try {
            const response = await this.api.get('/GetAverageOrderValue', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    timeFilter: timeFilter,
                    timeZone: CommonFunction.getTimeZone()
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Số lượng bàn hoạt động
     */
    public async getActiveTables() : Promise<any> {
        var result = undefined;

        try {
            const response = await this.api.get('/GetActiveTables');
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Xu hướng doanh thu
     */
    public async getRevenueTrend(fromDate: string, toDate: string, timeFilter: EnumTimeFilter) : Promise<any> {
        var result = [];

        try {
            const response = await this.api.get('/GetRevenueTrend', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    timeFilter: timeFilter,
                    timeZone: CommonFunction.getTimeZone()
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Món phổ biến
     */
    public async getPopularMenuItems(fromDate: string, toDate: string, timeFilter: EnumTimeFilter) : Promise<any> {
        var result = [];

        try {
            const response = await this.api.get('/GetPopularMenuItems', {
                params: {
                    fromDate: fromDate,
                    toDate: toDate,
                    timeFilter: timeFilter,
                    timeZone: CommonFunction.getTimeZone()
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data;
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Order gần đây
     */
    public async getRecentOrders() : Promise<Order[]> {
        var result:Order[] = [];

        try {
            const minDate = moment().startOf('day').utc().format();

            const response = await this.api.get('/GetRecentOrders', {
                params: {
                    minDate: minDate
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data as Order[];
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }

    /**
     * Lấy dữ liệu Đặt bàn hôm nay
     */
    public async getTodayReservations() : Promise<Reservation[]> {
        var result:Reservation[] = [];

        try {
            const minDate = moment().startOf('day').utc().format();

            const response = await this.api.get('/GetTodayReservations', {
                params: {
                    minDate: minDate
                }
            });
            const actionResult:MLActionResult = response.data as MLActionResult;
            if (actionResult.Success) {
                result = actionResult.Data as Reservation[];
            }
        } catch (e) {
            CommonFunction.handleException(e);
        }
        
        return result;
    }
}

export default DashboardService;

const dashboardService = new DashboardService();
export {dashboardService};