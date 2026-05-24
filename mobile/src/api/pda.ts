import request from './request'

interface ScanInParams {
  条形码: string
  库位编码: string
  数量: number
  进货单号?: string
  操作人?: string
}

interface ScanOutParams {
  条形码: string
  数量: number
  销售单号?: string
  操作人?: string
}

interface ScanRecord {
  id: number
  条形码: string
  商品Id: number
  操作类型: string
  库位Id?: number
  数量: number
  关联单号?: string
  操作人?: string
  操作时间: string
}

interface PageResult<T> {
  items: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

export function scanIn(data: ScanInParams): Promise<string> {
  return request<string>({ url: '/Pda/scan-in', method: 'POST', data })
}

export function scanOut(data: ScanOutParams): Promise<string> {
  return request<string>({ url: '/Pda/scan-out', method: 'POST', data })
}

export function getRecords(params: {
  条形码?: string
  操作类型?: string
  pageIndex?: number
  pageSize?: number
}): Promise<PageResult<ScanRecord>> {
  return request<PageResult<ScanRecord>>({
    url: '/Pda/records',
    method: 'GET',
    data: params,
    showLoading: false
  })
}

export type { ScanInParams, ScanOutParams, ScanRecord, PageResult }
