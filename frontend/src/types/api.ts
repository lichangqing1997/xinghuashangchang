// API 响应类型
export interface Result<T> {
  success: boolean;
  errorCode?: string;
  message: string;
  data?: T;
  timestamp: number;
}

// 分页响应类型
export interface PageResult<T> {
  items: T[];
  total: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
}

// 分页请求参数
export interface PageParams {
  pageIndex?: number;
  pageSize?: number;
}
