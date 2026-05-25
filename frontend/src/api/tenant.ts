import { api } from './client';
import type { Tenant, CreateTenantInput, UpdateTenantInput } from '../types/models';
import type { PageResult } from '../types/api';

export const tenantApi = {
  // 获取租户列表
  getList: (keyword?: string, pageIndex = 1, pageSize = 20) => {
    const params = new URLSearchParams();
    if (keyword) params.append('keyword', keyword);
    params.append('pageIndex', pageIndex.toString());
    params.append('pageSize', pageSize.toString());
    return api.get<PageResult<Tenant>>(`/api/Tenant/list?${params.toString()}`);
  },

  // 获取租户详情
  getById: (id: number) => {
    return api.get<Tenant>(`/api/Tenant/${id}`);
  },

  // 创建租户
  create: (input: CreateTenantInput) => {
    return api.post<number>('/api/Tenant', input);
  },

  // 更新租户
  update: (input: UpdateTenantInput) => {
    return api.put<boolean>('/api/Tenant', input);
  },

  // 删除租户
  delete: (id: number) => {
    return api.delete<boolean>(`/api/Tenant/${id}`);
  },
};
