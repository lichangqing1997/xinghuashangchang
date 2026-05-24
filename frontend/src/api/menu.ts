import { api } from './client';
import { Menu, MenuTree, CreateMenuInput, UpdateMenuInput } from '../types/models';

export const menuApi = {
  // 获取菜单列表（平铺）
  getList: (params?: { keyword?: string; status?: number }) =>
    api.get<Menu[]>('/api/Menu/list', params),

  // 获取菜单树
  getTree: (params?: { keyword?: string; status?: number }) =>
    api.get<MenuTree[]>('/api/Menu/tree', params),

  // 获取当前用户的菜单树
  getUserTree: (userId: number) =>
    api.get<MenuTree[]>(`/api/Menu/user-tree/${userId}`),

  // 获取菜单详情
  getById: (id: number) =>
    api.get<Menu>(`/api/Menu/${id}`),

  // 创建菜单
  create: (data: CreateMenuInput) =>
    api.post<number>('/api/Menu', data),

  // 更新菜单
  update: (data: UpdateMenuInput) =>
    api.put<boolean>('/api/Menu', data),

  // 删除菜单
  delete: (id: number) =>
    api.delete<boolean>(`/api/Menu/${id}`),
};
