import { api } from './client';
import type {
  InboundOrder,
  InboundOrderItem,
  CreateInboundOrderInput,
  UpdateInboundOrderInput,
  InboundOrderQueryInput,
  AuditInboundOrderInput
} from '../types/models';
import type { PageResult } from '../types/api';

export const inboundOrderApi = {
  // 获取入库单列表
  getList: (params?: InboundOrderQueryInput) =>
    api.get<PageResult<InboundOrder>>('/api/InboundOrder/list', params),

  // 根据ID获取入库单
  getById: (id: number) =>
    api.get<InboundOrder>(`/api/InboundOrder/${id}`),

  // 获取入库单明细
  getItems: (orderId: number) =>
    api.get<InboundOrderItem[]>(`/api/InboundOrder/${orderId}/items`),

  // 创建入库单
  create: (data: CreateInboundOrderInput) =>
    api.post<number>('/api/InboundOrder', data),

  // 更新入库单
  update: (data: UpdateInboundOrderInput) =>
    api.put<boolean>('/api/InboundOrder', data),

  // 删除入库单
  delete: (id: number) =>
    api.delete<boolean>(`/api/InboundOrder/${id}`),

  // 审核入库单
  audit: (id: number, data: AuditInboundOrderInput) =>
    api.post<boolean>(`/api/InboundOrder/${id}/audit`, data),

  // 变更入库单状态
  changeStatus: (id: number, newStatus: string, operatorName?: string) =>
    api.post<boolean>(`/api/InboundOrder/${id}/change-status?newStatus=${encodeURIComponent(newStatus)}${operatorName ? `&operatorName=${encodeURIComponent(operatorName)}` : ''}`)
};
