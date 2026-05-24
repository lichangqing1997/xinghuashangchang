import request from './request'

interface Product {
  id: number
  商品编码: string
  商品名称: string
  条形码: string
  SKU: string
  类别: string
  颜色?: string
  尺码?: string
  进货价: number
  销售价: number
  单位: string
  供应商Id?: number
  状态: number
  创建时间: string
  更新时间?: string
}

export function getProductByBarcode(barcode: string): Promise<Product> {
  return request<Product>({ url: `/商品/barcode/${barcode}`, method: 'GET' })
}

export type { Product }
