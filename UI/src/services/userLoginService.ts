import BaseService from "./baseService";
import { Customer, UserLogin } from "../models";
import MLActionResult from "../models/MLActionResult";

class UserLoginService extends BaseService<UserLogin> {
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
    async getUserData():Promise<MLActionResult> {
        const response = await this.api.get('/GetUserData');
        return response.data;
    }
}

export default UserLoginService;