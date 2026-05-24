import { api } from './client';
import { Inventory, Location } from '../types/models';
import { PageResult, PageParams } from '../types/api';

export const inventoryApi = {
  getList: (params?: { productId?: number; locationId?: number } & PageParams) =>
    api.get<PageResult<Inventory>>('/api/Inventory/list', params),

  getByProductId: (productId: number) =>
    api.get<Inventory[]>(`/api/Inventory/product/${productId}`),

  getByLocationId: (locationId: number) =>
    api.get<Inventory[]>(`/api/Inventory/location/${locationId}`),
};

export const locationApi = {
  getList: (params?: { keyword?: string } & PageParams) =>
    api.get<PageResult<Location>>('/api/Location/list', params),

  getById: (id: number) => api.get<Location>(`/api/Location/${id}`),

  getByCode: (code: string) => api.get<Location>(`/api/Location/code/${code}`),

  create: (data: Omit<Location, 'id' | 'createdAt'>) =>
    api.post<number>('/api/Location', data),

  update: (data: Location) => api.put<boolean>('/api/Location', data),

  delete: (id: number) => api.delete<boolean>(`/api/Location/${id}`),
};
