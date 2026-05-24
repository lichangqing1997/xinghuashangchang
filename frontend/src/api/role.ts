import { api } from './client';
import { RoleDetail, RoleOutput, CreateRoleInput, UpdateRoleInput } from '../types/models';
import { PageResult, PageParams } from '../types/api';
import { Role } from '../types/models';

export const roleApi = {
  // 获取角色列表
  getList: (params?: { keyword?: string; status?: number } & PageParams) =>
    api.get<PageResult<RoleDetail>>('/api/Role/list', params),

  // 获取所有启用的角色
  getAll: () =>
    api.get<Role[]>('/api/Role/all'),

  // 获取角色详情
  getById: (id: number) =>
    api.get<RoleDetail>(`/api/Role/${id}`),

  // 创建角色
  create: (data: CreateRoleInput) =>
    api.post<number>('/api/Role', data),

  // 更新角色
  update: (data: UpdateRoleInput) =>
    api.put<boolean>('/api/Role', data),

  // 删除角色
  delete: (id: number) =>
    api.delete<boolean>(`/api/Role/${id}`),
};
