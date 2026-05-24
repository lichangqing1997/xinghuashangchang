import dayjs from 'dayjs';
import type { OutboundOrder, OutboundOrderItem, Product, Location } from '../../../types/models';

interface Props {
  order: OutboundOrder;
  items: OutboundOrderItem[];
  products: Product[];
  locations: Location[];
}

export default function PrintableOutboundOrder({ order, items, products, locations }: Props) {
  const getProductName = (productId: number) => {
    const p = products.find((x) => x.id === productId);
    return p?.productName || `#${productId}`;
  };

  const getProductCode = (productId: number) => {
    const p = products.find((x) => x.id === productId);
    return p?.productCode || '';
  };

  const getLocationCode = (locationId?: number) => {
    if (!locationId) return '-';
    const loc = locations.find((x) => x.id === locationId);
    return loc?.locationCode || `#${locationId}`;
  };

  const totalAmount = items.reduce((s, i) => s + i.amount, 0);

  return (
    <div>
      <div className="title">出 库 单</div>
      <div className="info">
        <span>出库单号: {order.orderNo}</span>
        <span>日期: {dayjs(order.createdAt).format('YYYY-MM-DD')}</span>
        <span>收货公司: {order.companyName || '-'}</span>
        <span>制单人: {order.operator || '-'}</span>
      </div>
      <table>
        <thead>
          <tr>
            <th>序号</th>
            <th>商品编码</th>
            <th>商品名</th>
            <th>规格</th>
            <th>销售单价</th>
            <th>出库数量</th>
            <th>出库总价</th>
            <th>生产厂家</th>
            <th>有效期</th>
            <th>生产日期</th>
            <th>批号</th>
            <th>注册证号</th>
            <th>库位</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item, i) => (
            <tr key={item.id}>
              <td>{i + 1}</td>
              <td>{getProductCode(item.productId)}</td>
              <td>{getProductName(item.productId)}</td>
              <td>{item.specification || '-'}</td>
              <td>¥{item.unitPrice.toFixed(2)}</td>
              <td>{item.quantity}</td>
              <td>¥{item.amount.toFixed(2)}</td>
              <td>{item.manufacturer || '-'}</td>
              <td>{item.expiryDate ? dayjs(item.expiryDate).format('YYYY-MM-DD') : '-'}</td>
              <td>{item.productionDate ? dayjs(item.productionDate).format('YYYY-MM-DD') : '-'}</td>
              <td>{item.batchNo || '-'}</td>
              <td>{item.registrationNo || '-'}</td>
              <td>{getLocationCode(item.locationId)}</td>
            </tr>
          ))}
          <tr>
            <td colSpan={6} style={{ textAlign: 'right', fontWeight: 'bold' }}>合计金额:</td>
            <td style={{ fontWeight: 'bold' }}>¥{totalAmount.toFixed(2)}</td>
            <td colSpan={6}></td>
          </tr>
        </tbody>
      </table>
      <div className="signatures">
        <div>制单人签字</div>
        <div>仓库管理员签字</div>
        <div>收货人签字</div>
      </div>
    </div>
  );
}
