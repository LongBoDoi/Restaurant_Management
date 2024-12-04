import EventBus from "@/common/EventBus";
import { MLActionResult } from "@/models";
import axios, { AxiosInstance } from "axios";

abstract class BaseService<IMLEntity> {
  protected baseURL: string = '';
  protected api: AxiosInstance = axios.create();
  protected entityName: string = '';

  protected configApi() {
    this.baseURL = `${import.meta.env.VITE_API_URL}/${this.entityName}`;
    
    const api = axios.create({
      baseURL: this.baseURL,
      withCredentials: true
    });
    
    api.interceptors.response.use(
      response => response,
      error => {
          if (error.response?.status === 401) {
            EventBus.emit('RedirectToLogin');
          }
          return Promise.reject(error);
      }
    );

    this.api = api;
  }

  /**
   * Lấy dữ liệu phân trang
   * @param page Số trang
   * @param itemsPerPage Kích thước trang
   */
  public async getDataPaging(page: number, itemsPerPage: number) : Promise<MLActionResult> {
    const result = await this.api.get('/GetDataPaging', {
      params: {
        page: page,
        itemsPerPage: itemsPerPage
      }
    });
    return result.data as MLActionResult;
  }

  /**
   * Cập nhật bản ghi
   * @param entity Bản ghi
   */
  public async saveChanges(entity: IMLEntity) : Promise<MLActionResult> {
    const result = await this.api.post('/SaveChanges', entity);
    return result.data as MLActionResult;
  }
}

export default BaseService;