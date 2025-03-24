import CommonFunction from "@/common/CommonFunction";
import Config from "@/common/Config";
import EventBus from "@/common/EventBus";
import EventName from "@/common/EventName";
import LocalStorageKey from "@/common/LocalStorageKey";
import { MLActionResult } from "@/models";
import PagingData from "@/models/PagingData";
import axios, { AxiosInstance } from "axios";

abstract class BaseService<IMLEntity> {
  protected baseURL: string = '';
  protected api: AxiosInstance = axios.create();
  protected entityName: string = '';

  protected configApi() {
    this.baseURL = `${import.meta.env.VITE_API_URL}/api/${this.entityName}`;
    
    const api = axios.create({
      baseURL: this.baseURL,
      withCredentials: true
    });

    if (!Config.UseCookies) {
      api.interceptors.request.use((config) => {
        const token = localStorage.getItem(LocalStorageKey.AuthToken);
        config.headers.Authorization = `Bearer ${token ?? ''}`;
        return config;
      });
    }
    
    api.interceptors.response.use(
      response => {
        const actionResult = response.data as MLActionResult;
        if (actionResult?.Success === false && actionResult?.ErrorMsg) {
          CommonFunction.showToastMessage(actionResult.ErrorMsg, 'error');
        }

        return Promise.resolve(response);
      },
      error => {
          if (error.response?.status === 401) {
            EventBus.emit(EventName.RedirectToLogin);
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
  public async getAll() : Promise<IMLEntity[]> {
    var result = [] as IMLEntity[];

    try {
      const response = await this.api.get('/GetAll');
      const actionResult = response.data as MLActionResult;

      if (actionResult?.Success) {
        result = actionResult.Data as IMLEntity[];
      }
    } catch (e) {
      CommonFunction.handleException(e);
    }

    return result;
  }

  /**
   * Lấy dữ liệu phân trang
   * @param page Số trang
   * @param itemsPerPage Kích thước trang
   */
  public async getDataPaging(page: number, itemsPerPage: number) : Promise<PagingData<IMLEntity>> {
    var result:PagingData<IMLEntity> = {
      Data: [],
      TotalCount: 0
    };

    try {
      const response = await this.api.get('/GetDataPaging', {
        params: {
          page: page,
          itemsPerPage: itemsPerPage
        }
      });
      const actionResult = response.data as MLActionResult;

      if (actionResult?.Success) {
        result = actionResult.Data as PagingData<IMLEntity>;
      } else if (actionResult?.ErrorMsg) {
        CommonFunction.showToastMessage(actionResult.ErrorMsg, 'error');
      }
    } catch (e) {
      CommonFunction.handleException(e);
    }

    return result;
  }

  /**
   * Lấy dữ liệu theo ID
   */
  public async getByID(ID: string) : Promise<MLActionResult> {
    const result = await this.api.get('/GetByID', {
      params: {
        ID: ID
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