import { api } from './client';
import { Product } from '../types/models';
import { PageResult, PageParams } from '../types/api';

export const productApi = {
  getList: (params?: { keyword?: string } & PageParams) =>
    api.get<PageResult<Product>>('/api/Product/list', params),

  getById: (id: number) => api.get<Product>(`/api/Product/${id}`),

  getByBarcode: (barcode: string) => api.get<Product>(`/api/Product/barcode/${barcode}`),

  create: (data: Omit<Product, 'id' | 'createdAt' | 'updatedAt'>) =>
    api.post<number>('/api/Product', data),

  update: (data: Product) => api.put<boolean>('/api/Product', data),

  delete: (id: number) => api.delete<boolean>(`/api/Product/${id}`),
};
