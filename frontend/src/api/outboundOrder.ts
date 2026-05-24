import { api } from './client';
import type {
  OutboundOrder,
  OutboundOrderItem,
  OutboundFlow,
  CreateOutboundOrderInput,
  UpdateOutboundOrderInput,
  OutboundOrderQueryInput,
} from '../types/models';
import type { PageResult } from '../types/api';

export const outboundOrderApi = {
  getList: (params?: OutboundOrderQueryInput) =>
    api.get<PageResult<OutboundOrder>>('/api/OutboundOrder/list', params),

  getById: (id: number) =>
    api.get<OutboundOrder>(`/api/OutboundOrder/${id}`),

  getItems: (orderId: number) =>
    api.get<OutboundOrderItem[]>(`/api/OutboundOrder/${orderId}/items`),

  create: (data: CreateOutboundOrderInput) =>
    api.post<number>('/api/OutboundOrder', data),

  update: (data: UpdateOutboundOrderInput) =>
    api.put<boolean>('/api/OutboundOrder', data),

  delete: (id: number) =>
    api.delete<boolean>(`/api/OutboundOrder/${id}`),

  confirmOutbound: (id: number, operatorName?: string) => {
    const url = operatorName
      ? `/api/OutboundOrder/${id}/confirm?operatorName=${encodeURIComponent(operatorName)}`
      : `/api/OutboundOrder/${id}/confirm`;
    return api.post<boolean>(url);
  },

  getFlows: (params?: { orderNo?: string; keyword?: string; pageIndex?: number; pageSize?: number }) =>
    api.get<PageResult<OutboundFlow>>('/api/OutboundOrder/flows', params),
};
