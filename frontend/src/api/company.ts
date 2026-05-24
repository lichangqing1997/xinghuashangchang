import { api } from './client';
import type { Company, CompanyDetail, CreateCompanyInput, UpdateCompanyInput } from '../types/models';
import type { PageResult } from '../types/api';

export const companyApi = {
  // 获取公司列表
  getList: (keyword?: string, pageIndex = 1, pageSize = 20) => {
    const params = new URLSearchParams();
    if (keyword) params.append('keyword', keyword);
    params.append('pageIndex', pageIndex.toString());
    params.append('pageSize', pageSize.toString());
    return api.get<PageResult<Company>>(`/api/Company/list?${params.toString()}`);
  },

  // 获取公司详情
  getById: (id: number) => {
    return api.get<Company>(`/api/Company/${id}`);
  },

  // 获取公司扩展信息
  getDetails: (companyId: number) => {
    return api.get<CompanyDetail[]>(`/api/Company/${companyId}/details`);
  },

  // 创建公司
  create: (input: CreateCompanyInput) => {
    return api.post<number>('/api/Company', input);
  },

  // 更新公司
  update: (input: UpdateCompanyInput) => {
    return api.put<boolean>('/api/Company', input);
  },

  // 删除公司
  delete: (id: number) => {
    return api.delete<boolean>(`/api/Company/${id}`);
  },
};
