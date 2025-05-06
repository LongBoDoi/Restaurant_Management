import MLBaseService from "./baseService";
import { Customer, UserLogin } from "../models";
import MLActionResult from "../models/MLActionResult";
import CommonFunction from "@/common/CommonFunction";

class UserLoginService extends MLBaseService<UserLogin> {
    protected override entityName: string = 'UserLogin';

    constructor() {
        super();
        this.configApi();
    }

    /**
     * Tạo tài khoản mới
     */
    async registerNewCustomer(customer: Customer) : Promise<MLActionResult> {
        const response = await this.api.post(`/RegisterNewCustomer`, customer);
        return response.data;
    }

    /**
     * Thực hiện đăng nhập khách hàng
     */
    async loginCustomer(userLogin: UserLogin) : Promise<MLActionResult> {
        const response = await this.api.post(`/LoginCustomer`, userLogin);
        return response.data;
    }

    /**
     * Thực hiện đăng nhập nhân viên
     */
    async loginEmployee(userLogin: UserLogin) : Promise<MLActionResult> {
        const response = await this.api.post(`/LoginEmployee`, userLogin);
        return response.data;
    }

    /**
     * Thực hiện đăng xuất
     */
    async logOut() : Promise<MLActionResult> {
        const response = await this.api.post(`/Logout`);
        return response.data;
    }

    /**
     * Lấy dữ liệu đăng nhập của người dùng
     */
    async getUserData(userID: string):Promise<MLActionResult> {
        const response = await this.api.get('/GetUserData', {
            params: {
                userID: userID
            }
        });
        return response.data;
    }

    /**
     * Lấy dữ liệu đăng nhập của người dùng
     */
    async changePassword(userLogin: UserLogin):Promise<boolean> {
        try {
            const response = await this.api.post('/ChangePassword', userLogin);
            const actionResult = response.data as MLActionResult;

            if (!actionResult.Success && actionResult.ErrorMsg) {
                CommonFunction.showToastMessage(actionResult.ErrorMsg, 'error');
            }

            return actionResult.Success;
        } catch (e) {
            CommonFunction.handleException(e);
        }

        return false;
    }
}

export default UserLoginService;
export const userLoginService = new UserLoginService();