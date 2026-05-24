import { useState, useEffect, useRef } from 'react';
import { Modal, Descriptions, Table, Tag, Button, Space, Spin, message } from 'antd';
import { PrinterOutlined, ExportOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { outboundOrderApi, productApi, locationApi } from '../../../api';
import type { OutboundOrder, OutboundOrderItem, Product, Location } from '../../../types/models';
import PrintableOutboundOrder from './PrintableOutboundOrder';
import { exportOutboundOrderToExcel } from '../utils/exportExcel';

const statusColorMap: Record<string, string> = {
  '待出库': 'blue',
  '已出库': 'green',
  '已取消': 'red',
};

interface Props {
  open: boolean;
  orderId: number;
  onClose: () => void;
}

export default function OutboundOrderDetail({ open, orderId, onClose }: Props) {
  const [order, setOrder] = useState<OutboundOrder | null>(null);
  const [items, setItems] = useState<OutboundOrderItem[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [loading, setLoading] = useState(false);
  const printRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (open && orderId) {
      loadData();
    }
  }, [open, orderId]);

  const loadData = async () => {
    setLoading(true);
    try {
      const [orderRes, itemsRes, prodRes, locRes] = await Promise.all([
        outboundOrderApi.getById(orderId),
        outboundOrderApi.getItems(orderId),
        productApi.getList({ pageIndex: 1, pageSize: 500 }),
        locationApi.getList({ pageIndex: 1, pageSize: 100 }),
      ]);
      if (orderRes.success && orderRes.data) setOrder(orderRes.data);
      if (itemsRes.success && itemsRes.data) setItems(itemsRes.data);
      if (prodRes.success && prodRes.data) setProducts(prodRes.data.items);
      if (locRes.success && locRes.data) setLocations(locRes.data.items);
    } finally {
      setLoading(false);
    }
  };

  const getProductName = (productId: number) => {
    const p = products.find((x) => x.id === productId);
    return p ? `${p.productCode} - ${p.productName}` : `#${productId}`;
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

  const handlePrint = () => {
    if (!order || !printRef.current) return;
    const printWindow = window.open('', '_blank');
    if (!printWindow) return;
    printWindow.document.write(`
      <html><head><title>出库单_${order.orderNo}</title>
      <style>
        body { font-family: SimSun, serif; font-size: 12px; padding: 20px; }
        table { width: 100%; border-collapse: collapse; margin: 10px 0; }
        th, td { border: 1px solid #000; padding: 6px 8px; text-align: left; }
        th { background: #f5f5f5; }
        .title { text-align: center; font-size: 20px; font-weight: bold; margin: 20px 0; }
        .info { margin: 10px 0; }
        .info span { margin-right: 30px; }
        .signatures { display: flex; justify-content: space-between; margin-top: 40px; }
        .signatures div { width: 30%; border-top: 1px solid #000; padding-top: 5px; text-align: center; }
      </style></head><body>${printRef.current.innerHTML}</body></html>
    `);
    printWindow.document.close();
    printWindow.print();
  };

  const handleExport = () => {
    if (!order) return;
    exportOutboundOrderToExcel(order, items, products, locations);
    message.success('导出成功');
  };

  const columns: ColumnsType<OutboundOrderItem> = [
    { title: '序号', width: 60, render: (_, __, i) => i + 1 },
    { title: '商品编码', width: 100, render: (_, r) => getProductCode(r.productId) },
    { title: '商品名', width: 120, render: (_, r) => getProductName(r.productId).split(' - ')[1] || '' },
    { title: '规格', dataIndex: 'specification', width: 100, render: (v) => v || '-' },
    { title: '销售单价', dataIndex: 'unitPrice', width: 100, render: (v) => `¥${v.toFixed(2)}` },
    { title: '出库数量', dataIndex: 'quantity', width: 80 },
    { title: '出库总价', dataIndex: 'amount', width: 100, render: (v) => `¥${v.toFixed(2)}` },
    { title: '生产厂家', dataIndex: 'manufacturer', width: 120, render: (v) => v || '-' },
    { title: '有效期', dataIndex: 'expiryDate', width: 110, render: (v) => v ? dayjs(v).format('YYYY-MM-DD') : '-' },
    { title: '生产日期', dataIndex: 'productionDate', width: 110, render: (v) => v ? dayjs(v).format('YYYY-MM-DD') : '-' },
    { title: '批号', dataIndex: 'batchNo', width: 100, render: (v) => v || '-' },
    { title: '注册证号', dataIndex: 'registrationNo', width: 120, render: (v) => v || '-' },
    { title: '库位', width: 100, render: (_, r) => getLocationCode(r.locationId) },
  ];

  if (!order) return null;

  return (
    <Modal
      title="出库单详情"
      open={open}
      onCancel={onClose}
      width={1400}
      destroyOnClose
      footer={
        <Space>
          <Button onClick={onClose}>关闭</Button>
          <Button icon={<PrinterOutlined />} onClick={handlePrint}>打印</Button>
          <Button icon={<ExportOutlined />} onClick={handleExport}>导出Excel</Button>
        </Space>
      }
    >
      <Spin spinning={loading}>
        <Descriptions bordered column={4} size="small" style={{ marginBottom: 16 }}>
          <Descriptions.Item label="出库单号">{order.orderNo}</Descriptions.Item>
          <Descriptions.Item label="收货公司">{order.companyName || '-'}</Descriptions.Item>
          <Descriptions.Item label="总金额">¥{order.totalAmount.toFixed(2)}</Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag color={statusColorMap[order.status] || 'default'}>{order.status}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="制单人">{order.operator || '-'}</Descriptions.Item>
          <Descriptions.Item label="备注" span={2}>{order.remark || '-'}</Descriptions.Item>
          <Descriptions.Item label="创建时间">{order.createdAt ? dayjs(order.createdAt).format('YYYY-MM-DD HH:mm') : '-'}</Descriptions.Item>
          <Descriptions.Item label="出库时间">{order.outboundAt ? dayjs(order.outboundAt).format('YYYY-MM-DD HH:mm') : '-'}</Descriptions.Item>
        </Descriptions>

        <Table
          columns={columns}
          dataSource={items}
          rowKey="id"
          pagination={false}
          scroll={{ x: 1400 }}
          size="small"
          footer={() => (
            <div style={{ textAlign: 'right', fontWeight: 'bold' }}>
              合计金额: ¥{items.reduce((s, i) => s + i.amount, 0).toFixed(2)}
            </div>
          )}
        />

        <div style={{ position: 'absolute', left: '-9999px', top: 0 }}>
          <div ref={printRef}>
            <PrintableOutboundOrder order={order} items={items} products={products} locations={locations} />
          </div>
        </div>
      </Spin>
    </Modal>
  );
}
