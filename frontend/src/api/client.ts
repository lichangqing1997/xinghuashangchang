import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { Result } from '../types/api';

// 创建 axios 实例
export const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5242',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // 从 localStorage 获取 token
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error: AxiosError<Result<unknown>>) => {
    // 处理 401 错误（未授权）
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// 封装 API 方法
export const api = {
  get: <T>(url: string, params?: any) =>
    apiClient.get<Result<T>>(url, { params }).then((res) => res.data),

  post: <T>(url: string, data?: unknown) =>
    apiClient.post<Result<T>>(url, data).then((res) => res.data),

  put: <T>(url: string, data?: unknown) =>
    apiClient.put<Result<T>>(url, data).then((res) => res.data),

  delete: <T>(url: string) =>
    apiClient.delete<Result<T>>(url).then((res) => res.data),
};

export default apiClient;
