import { api } from './client';
import { Supplier } from '../types/models';
import { PageResult, PageParams } from '../types/api';

export const supplierApi = {
  getList: (params?: { keyword?: string } & PageParams) =>
    api.get<PageResult<Supplier>>('/api/Supplier/list', params),

  getById: (id: number) => api.get<Supplier>(`/api/Supplier/${id}`),

  create: (data: Omit<Supplier, 'id' | 'createdAt' | 'updatedAt'>) =>
    api.post<number>('/api/Supplier', data),

  update: (data: Supplier) => api.put<boolean>('/api/Supplier', data),

  delete: (id: number) => api.delete<boolean>(`/api/Supplier/${id}`),
};
