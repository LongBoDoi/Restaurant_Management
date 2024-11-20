import BaseService from "./baseService";
import { UserLogin } from "../models";
import MLActionResult from "../models/MLActionResult";

class UserLoginService extends BaseService {
    protected override entityName: string = 'UserLogin';

    constructor() {
        super();
        this.configApi();
    }

    // /**
    //  * Tạo tài khoản mới
    //  */
    // async createAccount(users: Users) : Promise<MLActionResult> {
    //     const response = await axios.post(`${this.url}/CreateAccount`, users);
    //     return response.data;
    // }

    /**
     * Thực hiện đăng nhập
     */
    async login(userLogin: UserLogin) : Promise<MLActionResult|undefined> {
        const response = await this.api.post(`/Login`, userLogin);
        return response.data;
    }

    async getUserData():Promise<MLActionResult|undefined> {
        const response = await this.api.get('/GetUserData');
        return response.data;
        // try {
        //     const response = await axios.get(`${this.url}/GetUserData/${userID}`);
        //     return response.data;
        // } catch (error) {
        //     console.error('Error fetching data:', error);
        //     return undefined;
        // }
    }
}

export default UserLoginService;