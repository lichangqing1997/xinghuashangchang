import * as XLSX from 'xlsx';
import dayjs from 'dayjs';
import type { OutboundOrder, OutboundOrderItem, Product, Location } from '../../../types/models';

export function exportOutboundOrderToExcel(
  order: OutboundOrder,
  items: OutboundOrderItem[],
  products: Product[],
  locations: Location[]
) {
  const getProductName = (productId: number) => {
    const p = products.find((x) => x.id === productId);
    return p?.productName || '';
  };

  const getProductCode = (productId: number) => {
    const p = products.find((x) => x.id === productId);
    return p?.productCode || '';
  };

  const getLocationCode = (locationId?: number) => {
    if (!locationId) return '';
    const loc = locations.find((x) => x.id === locationId);
    return loc?.locationCode || '';
  };

  const totalAmount = items.reduce((s, i) => s + i.amount, 0);

  const headerRows = [
    ['出库单'],
    [],
    ['出库单号', order.orderNo, '收货公司', order.companyName || ''],
    ['日期', dayjs(order.createdAt).format('YYYY-MM-DD'), '制单人', order.operator || ''],
    ['状态', order.status, '总金额', `¥${totalAmount.toFixed(2)}`],
    [],
  ];

  const itemHeaders = ['序号', '商品编码', '商品名', '规格', '销售单价', '出库数量', '出库总价', '生产厂家', '有效期', '生产日期', '批号', '注册证号', '库位'];
  const itemRows = items.map((item, i) => [
    i + 1,
    getProductCode(item.productId),
    getProductName(item.productId),
    item.specification || '',
    item.unitPrice,
    item.quantity,
    item.amount,
    item.manufacturer || '',
    item.expiryDate ? dayjs(item.expiryDate).format('YYYY-MM-DD') : '',
    item.productionDate ? dayjs(item.productionDate).format('YYYY-MM-DD') : '',
    item.batchNo || '',
    item.registrationNo || '',
    getLocationCode(item.locationId),
  ]);

  const ws = XLSX.utils.aoa_to_sheet([...headerRows, itemHeaders, ...itemRows]);

  ws['!cols'] = [
    { wch: 6 }, { wch: 12 }, { wch: 16 }, { wch: 12 },
    { wch: 10 }, { wch: 10 }, { wch: 10 }, { wch: 14 },
    { wch: 12 }, { wch: 12 }, { wch: 12 }, { wch: 14 },
    { wch: 12 },
  ];

  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, '出库单');
  XLSX.writeFile(wb, `出库单_${order.orderNo}.xlsx`);
}
