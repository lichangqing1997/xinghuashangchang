import { api } from './client';
import { User, LoginInput } from '../types/models';

// 认证 API
export const authApi = {
  // 登录
  login: (data: LoginInput) => api.post<User>('/api/auth/login', data),

  // 获取当前用户信息
  getCurrentUser: () => api.get<User>('/api/auth/me'),

  // 退出登录
  logout: () => api.post<void>('/api/auth/logout'),
};
