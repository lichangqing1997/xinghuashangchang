import { api, apiClient } from './client';
import { UserDetail, CreateUserInput, UpdateUserInput, ChangePasswordInput } from '../types/models';
import { PageResult, PageParams, Result } from '../types/api';

export const userApi = {
  // 获取用户列表
  getList: (params?: { keyword?: string; status?: number } & PageParams) =>
    api.get<PageResult<UserDetail>>('/api/User/list', params),

  // 获取用户详情
  getById: (id: number) =>
    api.get<UserDetail>(`/api/User/${id}`),

  // 创建用户
  create: (data: CreateUserInput) =>
    api.post<number>('/api/User', data),

  // 更新用户
  update: (data: UpdateUserInput) =>
    api.put<boolean>('/api/User', data),

  // 删除用户
  delete: (id: number) =>
    api.delete<boolean>(`/api/User/${id}`),

  // 修改密码
  changePassword: (data: ChangePasswordInput) =>
    api.post<boolean>('/api/User/change-password', data),

  // 上传头像
  uploadAvatar: (file: File, userId: number) => {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post<Result<string>>(`/api/User/avatar?userId=${userId}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    }).then((res) => res.data);
  },
};
