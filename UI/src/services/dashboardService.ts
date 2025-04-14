import { MLActionResult } from "@/models";
import { BaseService } from "./baseService";
import CommonFunction from "@/common/CommonFunction";
import moment from "moment";

class DashboardService extends BaseService {
    protected entityName: string = 'Dashboard';
    
    constructor() {
        super();
        this.configApi();
    }

    /**
     * Lấy dữ liệu doanh thu hôm nay
     */
    public async getTodayRevenue() : Promise<any> {
        var result = undefined;

        try {
            const minDate = moment().startOf('day').utc().format();

            const response = await this.api.get('/GetTodayRevenue', {
                params: {
                    minDate: minDate
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
}

export default DashboardService;

const dashboardService = new DashboardService();
export {dashboardService};