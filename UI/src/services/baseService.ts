import EventBus from "@/common/EventBus";
import axios, { AxiosInstance } from "axios";

class BaseService {
  protected baseURL: string = '';
  protected api: AxiosInstance = axios.create();
  protected entityName: string = '';

  configApi() {
    this.baseURL = `${import.meta.env.VITE_API_URL}/${this.entityName}`;
    
    const api = axios.create({
      baseURL: this.baseURL,
      withCredentials: true
    });

    api.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken');
      if (token) { 
        config.headers['Authorization'] = `Bearer ${token}`; 
      }
      return config; 
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
}

export default BaseService;