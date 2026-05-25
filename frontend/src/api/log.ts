import { api } from './client';
import type { DebugLog, ApiLog, OperationLog } from '../types/models';
import type { PageResult } from '../types/api';

export const logApi = {
  // 调试日志
  getDebugLogs: (params?: {
    level?: string;
    keyword?: string;
    startTime?: string;
    endTime?: string;
    pageIndex?: number;
    pageSize?: number;
  }) => {
    const searchParams = new URLSearchParams();
    if (params?.level) searchParams.append('level', params.level);
    if (params?.keyword) searchParams.append('keyword', params.keyword);
    if (params?.startTime) searchParams.append('startTime', params.startTime);
    if (params?.endTime) searchParams.append('endTime', params.endTime);
    searchParams.append('pageIndex', (params?.pageIndex || 1).toString());
    searchParams.append('pageSize', (params?.pageSize || 20).toString());
    return api.get<PageResult<DebugLog>>(`/api/Log/debug?${searchParams.toString()}`);
  },

  clearDebugLogs: (before?: string) => {
    const params = before ? `?before=${before}` : '';
    return api.delete<boolean>(`/api/Log/debug/clear${params}`);
  },

  // 接口日志
  getApiLogs: (params?: {
    method?: string;
    keyword?: string;
    statusCode?: number;
    startTime?: string;
    endTime?: string;
    pageIndex?: number;
    pageSize?: number;
  }) => {
    const searchParams = new URLSearchParams();
    if (params?.method) searchParams.append('method', params.method);
    if (params?.keyword) searchParams.append('keyword', params.keyword);
    if (params?.statusCode) searchParams.append('statusCode', params.statusCode.toString());
    if (params?.startTime) searchParams.append('startTime', params.startTime);
    if (params?.endTime) searchParams.append('endTime', params.endTime);
    searchParams.append('pageIndex', (params?.pageIndex || 1).toString());
    searchParams.append('pageSize', (params?.pageSize || 20).toString());
    return api.get<PageResult<ApiLog>>(`/api/Log/api?${searchParams.toString()}`);
  },

  clearApiLogs: (before?: string) => {
    const params = before ? `?before=${before}` : '';
    return api.delete<boolean>(`/api/Log/api/clear${params}`);
  },

  // 操作日志
  getOperationLogs: (params?: {
    module?: string;
    action?: string;
    keyword?: string;
    startTime?: string;
    endTime?: string;
    pageIndex?: number;
    pageSize?: number;
  }) => {
    const searchParams = new URLSearchParams();
    if (params?.module) searchParams.append('module', params.module);
    if (params?.action) searchParams.append('action', params.action);
    if (params?.keyword) searchParams.append('keyword', params.keyword);
    if (params?.startTime) searchParams.append('startTime', params.startTime);
    if (params?.endTime) searchParams.append('endTime', params.endTime);
    searchParams.append('pageIndex', (params?.pageIndex || 1).toString());
    searchParams.append('pageSize', (params?.pageSize || 20).toString());
    return api.get<PageResult<OperationLog>>(`/api/Log/operation?${searchParams.toString()}`);
  },

  clearOperationLogs: (before?: string) => {
    const params = before ? `?before=${before}` : '';
    return api.delete<boolean>(`/api/Log/operation/clear${params}`);
  },
};
